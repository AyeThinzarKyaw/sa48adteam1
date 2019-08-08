using LUSSIS.Models;
using LUSSIS.Models.DTOs;
using LUSSIS.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LUSSIS.Controllers
{
    public class PurchaseOrderController : Controller
    {

        // GET: All PurchaseOrder
        //By ATZK
        public ActionResult Index()
        {
            //EmailNotificationService.Instance.SendNotificationEmail(receipient:"athinzarkyaw@gmail.com",subject: "test test test AD", body: "successfully sent out");
            PurchaseOrderListDTO purchaseOrders = new PurchaseOrderListDTO();
            purchaseOrders.PurchaseOrders = PurchaseOrderService.Instance.getAllPurchaseOrders();

            return View(purchaseOrders);
        }

        // POST: Search PO between chosen dates
        //By ATZK
        [HttpPost]
        public ActionResult Index(PurchaseOrderListDTO purchaseOrderListDTO)
        {
            if (purchaseOrderListDTO.FromDate != null && purchaseOrderListDTO.ToDate != null)
            {
                purchaseOrderListDTO.PurchaseOrders = PurchaseOrderService.Instance.getAllPurchaseOrders()
                                                    .Where(x => x.OrderDateTime.Date >= purchaseOrderListDTO.FromDate.Date
                                                    && x.OrderDateTime <= purchaseOrderListDTO.ToDate.Date)
                                                    .OrderByDescending(x => x.OrderDateTime);
            }
            else
            {
                purchaseOrderListDTO.PurchaseOrders = PurchaseOrderService.Instance.getAllPurchaseOrders()
                                                    .OrderByDescending(x => x.OrderDateTime);
            }
            return View(purchaseOrderListDTO);
        }



        // cancelPOStatus as cancelled
        //By ATZK
        public JsonResult CancelPO(int poId)
        {
            PurchaseOrder purchaseOrder = PurchaseOrderService.Instance.getPurchaseOrderById(poId);
            if (purchaseOrder != null)
            {
                if (purchaseOrder.Status == Enum.GetName(typeof(Enums.POStatus), Enums.POStatus.OPEN))
                {
                    purchaseOrder.Status = Enum.GetName(typeof(Enums.POStatus), Enums.POStatus.CANCELLED);
                    PurchaseOrderService.Instance.UpdatePO(purchaseOrder);
                    return Json(true, JsonRequestBehavior.AllowGet);
                }
                return Json(false, JsonRequestBehavior.AllowGet);
            }
            return Json(false, JsonRequestBehavior.AllowGet);
        }


        // View PO Details
        //By ATZK
        public ActionResult Detail(int poId)
        {
            PurchaseOrder purchaseOrder = PurchaseOrderService.Instance.getPurchaseOrderById(poId);

            return View(purchaseOrder);
        }

        // Approve/Reject PurchaseOrder
        //By ATZK
        public JsonResult ConfirmPO(int poId, string reply, string remark)
        {
            PurchaseOrder purchaseOrder = PurchaseOrderService.Instance.getPurchaseOrderById(poId);


            if (purchaseOrder != null)
            {
                if (purchaseOrder.Status == Enum.GetName(typeof(Enums.POStatus), Enums.POStatus.OPEN))
                {
                    purchaseOrder.Status = reply;
                    purchaseOrder.Remark = remark;
                    PurchaseOrderService.Instance.UpdatePO(purchaseOrder);
                    return Json(new object[] { true, "" }, JsonRequestBehavior.AllowGet);
                }
                return Json(new object[] { false, "This purchase order is not in OPEN status." }, JsonRequestBehavior.AllowGet);
            }
            return Json(new object[] { false, "This purchase order does not exist." }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ReceiveDO(int poId)
        {
            ReceiveDoDTO receiveDO = new ReceiveDoDTO();
            receiveDO.purchaseOrder = PurchaseOrderService.Instance.getPurchaseOrderById(poId);


            if (receiveDO.purchaseOrder.PurchaseOrderDetails.Count > 0)
            {
                receiveDO.DOReceivedList = new int[receiveDO.purchaseOrder.PurchaseOrderDetails.Count];
                int i = 0;
                foreach (var item in receiveDO.purchaseOrder.PurchaseOrderDetails)
                {
                    receiveDO.DOReceivedList[i] = item.QuantityDelivered == null ? 0 : (int)item.QuantityDelivered;
                    i++;
                }
            }
            return View(receiveDO);
        }

        [HttpPost]
        public ActionResult ReceiveDO(ReceiveDoDTO receiveDO)
        {
            if (ModelState.IsValid)
            {
                if (receiveDO.DO.ContentLength > 10 * 1024 * 1024 || receiveDO.Invoice.ContentLength > 10 * 1024 * 1024)
                {
                    receiveDO.Error.HasError = true;
                    receiveDO.Error.Message = "Each attachment cannot be bigger than 10MB.";

                }
                else
                {
                    if (TempData["DOReceivedQty"] != null)
                    {
                        PurchaseOrder updatedPO = PurchaseOrderService.Instance.getPurchaseOrderById(receiveDO.purchaseOrder.Id);
                        PurchaseOrder receivedQtyDTO = (PurchaseOrder)TempData["DOReceivedQty"];
                        TempData.Keep("DOReceivedQty");

                        //save attachments
                        //var filename = Path.GetFileName(receiveDO.DO.FileName);

                        var filename = "DO_" + receiveDO.purchaseOrder.Id + "_" + DateTime.Now.ToString("ddMMyyyy_hhmm") + Path.GetExtension(receiveDO.DO.FileName);
                        var path = Path.Combine(Server.MapPath("~/Images/DeliveryOrders/"), filename);

                        receiveDO.DO.SaveAs(path);
                        receiveDO.purchaseOrder.DO = filename;
                        updatedPO.DO = filename;


                        filename = "Invoice_" + receiveDO.purchaseOrder.Id + "_" + DateTime.Now.ToString("ddMMyyyy_hhmm") + Path.GetExtension(receiveDO.Invoice.FileName);
                        path = Path.Combine(Server.MapPath("~/Images/DeliveryOrders/"), filename);
                        receiveDO.Invoice.SaveAs(path);
                        receiveDO.purchaseOrder.Invoice = filename;
                        updatedPO.Invoice = filename;

                        bool poComplete = true;
                        //update POdetails deliveredQty
                        foreach (var detail in receivedQtyDTO.PurchaseOrderDetails)
                        {

                            PurchaseOrderDetail oldPODetail = updatedPO.PurchaseOrderDetails.Single(x => x.Id == detail.Id);
                            int qtyDeliveredNow = detail.QuantityDelivered == null ? 0 : (int)detail.QuantityDelivered;
                            //int oldQty = oldPODetail.QuantityDelivered == null ? 0 : (int)oldPODetail.QuantityDelivered;
                            //int newQty= oldQty + qtyDeliveredNow;//qtyInDB+nowReceivedQty
                            //oldPODetail.QuantityDelivered = oldQty + qtyDeliveredNow;//qtyInDB+nowReceivedQty
                            //check to complete PO
                            if (oldPODetail.QuantityOrdered > oldPODetail.QuantityDelivered)
                            {
                                poComplete = false;
                            }


                            //PurchaseOrderService.Instance.UpdatePODetail(oldPODetail);

                            //update stationery qty
                            Stationery s = StationeryService.Instance.GetStationeryById(detail.StationeryId);
                            s.Quantity = s.Quantity + qtyDeliveredNow;
                            StationeryService.Instance.UpdateStationery(s);

                            //check whether to raise AdjVoucher (eg: gift, extra)
                            if (oldPODetail.QuantityOrdered < oldPODetail.QuantityDelivered)
                            {
                                //raise adjustment voucher
                                AdjustmentVoucherDetail adjustmentItem = new AdjustmentVoucherDetail();
                                adjustmentItem.DateTime = DateTime.Now;
                                adjustmentItem.Quantity = (int)(oldPODetail.QuantityDelivered - oldPODetail.QuantityOrdered);
                                adjustmentItem.Reason = "Received extra (eg: gift) on Delivery Order Receive";
                            }

                            //Check to move waitlistApproved to Preparing
                        }
                        if (poComplete == true)
                        {
                            updatedPO.Status = Enum.GetName(typeof(Enums.POStatus), Enums.POStatus.CLOSED);
                        }

                        //save updatedPO
                        updatedPO.DeliveryDateTime = DateTime.Now;
                        updatedPO.Remark = receivedQtyDTO.Remark;
                        updatedPO.DeliveryOrderNo = receivedQtyDTO.DeliveryOrderNo;
                        PurchaseOrderService.Instance.UpdatePO(updatedPO);
                        //EmailNotificationService.Instance.SendNotificationEmail("e0395895@u.nus.edu", "test test test AD", "successfully sent out", new string[] { Path.Combine(Server.MapPath("~/Images/DeliveryOrders/"), updatedPO.DO) , Path.Combine(Server.MapPath("~/Images/DeliveryOrders/"), updatedPO.Invoice) });
                        TempData["DOReceivedQty"] = null;
                        return RedirectToAction("Index");

                    }

                }
            }

            PurchaseOrder po = PurchaseOrderService.Instance.getPurchaseOrderById(receiveDO.purchaseOrder.Id);
            po.Remark = receiveDO.purchaseOrder.Remark;
            po.DO = receiveDO.purchaseOrder.DO;
            receiveDO.purchaseOrder = po;
            return View(receiveDO);
        }

        public JsonResult UpdateReceivedQty(int podId, int qty, int poId)
        {
            PurchaseOrder po = new PurchaseOrder();
            if (TempData["DOReceivedQty"] == null)
            {
                po = PurchaseOrderService.Instance.getPurchaseOrderById(poId);
            }
            else if (TempData["DOReceivedQty"] != null)
            {
                po = (PurchaseOrder)TempData["DOReceivedQty"];
                TempData.Keep("DOReceivedQty");
            }
            if (po.Id == poId)
            {
                po.PurchaseOrderDetails.Single(x => x.Id == podId).QuantityDelivered += qty;

                TempData["DOReceivedQty"] = po;
            }

            return Json(true, JsonRequestBehavior.AllowGet);
        }



        // GET: View PurchaseOrder Catalogue to select items
        //By ATZK
        public ActionResult Catalogue()
        {
            POCreateDTO poCreateDTO = new POCreateDTO();
            if (TempData["PO"] != null)
            {
                poCreateDTO = (POCreateDTO)TempData["PO"];
                TempData.Keep("PO");
            }
            else
            {
                poCreateDTO.Catalogue = PurchaseOrderService.Instance.RetrievePurchaseOrderCatalogue().ToList();
            }

            return View(poCreateDTO);
        }

        public JsonResult updateSelectList(int stationeryId, int qty, bool selectOrNot, bool selectAllSelected)
        {
            POCreateDTO poCreateDTO = new POCreateDTO();
            if (TempData["PO"] != null)
            {
                poCreateDTO = (POCreateDTO)TempData["PO"];
                TempData.Keep("PO");
            }
            else
            {
                poCreateDTO = new POCreateDTO();
                poCreateDTO.Catalogue = PurchaseOrderService.Instance.RetrievePurchaseOrderCatalogue().ToList();
            }
            if (selectAllSelected == false)//if show all selected is not selected
            {
                if (stationeryId == 0 && qty == 0 && selectOrNot == false)
                {
                    TempData["PO"] = poCreateDTO;
                    return Json(this.RenderRazorViewToString("~/Views/PurchaseOrder/Catalogue_Partial.cshtml", poCreateDTO), JsonRequestBehavior.AllowGet);
                }
                else
                {
                    poCreateDTO.Catalogue.Single(x => x.Id == stationeryId).Unsubmitted = qty;
                    if (selectOrNot)
                    {
                        if (poCreateDTO.SelectedItems == null)
                        {
                            poCreateDTO.SelectedItems = new List<Stationery>();
                        }
                        if (poCreateDTO.SelectedItems.Where(x => x.Id == stationeryId).Count() <= 0)
                        {
                            Stationery item = StationeryService.Instance.GetStationeryById(stationeryId);
                            item.CategoryId = 0;
                            poCreateDTO.SelectedItems.Add(item);
                        }

                    }
                    else
                    {
                        if (poCreateDTO != null && poCreateDTO.SelectedItems.Where(x => x.Id == stationeryId).Count() > 0)
                        {
                            poCreateDTO.SelectedItems.Remove(poCreateDTO.SelectedItems.Single(x => x.Id == stationeryId));
                        }

                    }
                    TempData["PO"] = poCreateDTO;

                    return Json(true, JsonRequestBehavior.AllowGet);
                }

            }
            else
            {
                TempData["PO"] = poCreateDTO;
                List<PO_getPOCatalogue_Result> catalogueSelected = new List<PO_getPOCatalogue_Result>();
                foreach (var item in poCreateDTO.SelectedItems)
                {
                    catalogueSelected.Add(poCreateDTO.Catalogue.Single(x => x.Id == item.Id));
                }
                POCreateDTO selectedCatalogueDTO = new POCreateDTO();
                selectedCatalogueDTO.Catalogue = catalogueSelected;
                return Json(this.RenderRazorViewToString("~/Views/PurchaseOrder/Catalogue_Partial.cshtml", selectedCatalogueDTO), JsonRequestBehavior.AllowGet);
            }

        }

        public string RenderRazorViewToString(string viewName, object model)
        {
            ViewData.Model = model;
            using (var sw = new StringWriter())
            {
                var viewResult = ViewEngines.Engines.FindPartialView(ControllerContext, viewName);
                var viewContext = new ViewContext(ControllerContext, viewResult.View, ViewData, TempData, sw);
                viewResult.View.Render(viewContext, sw);
                viewResult.ViewEngine.ReleaseView(ControllerContext, viewResult.View);
                return sw.GetStringBuilder().ToString();
            }
        }

        public ActionResult SelectSupplier()
        {
            if (TempData["PO"] == null)
            {
                return RedirectToAction("Catalogue");
            }
            POCreateDTO poCreateDTO = (POCreateDTO)TempData["PO"];
            TempData.Keep("PO");
            return View(poCreateDTO);
        }

        [HttpPost]
        public JsonResult RaisePO(bool removeZero)
        {
            POCreateDTO poCreateDTO = (POCreateDTO)TempData["PO"];
            TempData.Keep("PO");
            var ids = poCreateDTO.SelectedItems.Select(x => x.Id).ToList();
            int zeroCount = poCreateDTO.Catalogue.Where(x =>ids.Contains(x.Id)).Where(o=>o.Unsubmitted == 0).Count();
            if (zeroCount>0 && removeZero==false)
            {
               return Json(new object[] { true, "Currently, there are some stationery of Order Quantity fills as 0. Stationery with Order Quantity 0 will not raise PO. Click OK to continue raising PO for other stationeries. Click CANCEL to edit." }, JsonRequestBehavior.AllowGet);
            }

            if (poCreateDTO.SelectedItems.Where(s => s.CategoryId != 0 && s.Status == "confirmed").Count() > 0)
            {
                PurchaseOrderService.Instance.RaisePO(poCreateDTO);
                TempData["PO"] = null;
                return Json(new object[] { true, null }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new object[] { false, "Currently, there is no confirmed stationery to raise PO. Please confirm first." }, JsonRequestBehavior.AllowGet);
            }



        }

        [HttpPost]
        public JsonResult UpdateSelectedSupplier(int stationeryId, int supplierId)
        {

            POCreateDTO poCreateDTO = (POCreateDTO)TempData["PO"];
            TempData.Keep("PO");
            poCreateDTO.SelectedItems.Single(x => x.Id == stationeryId).CategoryId = supplierId;
            TempData["PO"] = poCreateDTO;
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult UpdateEstimatedDate(DateTime estDate, int supplierId)
        {
            POCreateDTO poCreateDTO = (POCreateDTO)TempData["PO"];
            TempData.Keep("PO");
            if (poCreateDTO.EstimatedDates.Where(x => x.Key == supplierId).Count() == 1)
            {
                poCreateDTO.EstimatedDates.Remove(poCreateDTO.EstimatedDates.Single(x => x.Key == supplierId));
            }
            poCreateDTO.EstimatedDates.Add(new KeyValuePair<int, DateTime>(supplierId, estDate));

            TempData["PO"] = poCreateDTO;

            return Json(true, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult RemoveItem(int stationeryId)
        {
            POCreateDTO poCreateDTO = (POCreateDTO)TempData["PO"];
            TempData.Keep("PO");
            int supplierId = poCreateDTO.SelectedItems.Single(x => x.Id == stationeryId).CategoryId;
            poCreateDTO.SelectedItems.Remove(poCreateDTO.SelectedItems.Single(x => x.Id == stationeryId));
            if (supplierId != 0)
            {
                if (poCreateDTO.SelectedItems.Where(x => x.CategoryId == supplierId).Count() <= 0)
                {
                    poCreateDTO.EstimatedDates.Remove(poCreateDTO.EstimatedDates.Single(x => x.Key == supplierId));
                }
            }
            TempData["PO"] = poCreateDTO;

            return Json(true, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult ConfirmItemToPO(int stationeryId, string confirmStatus)
        {
            if (TempData["PO"] != null)
            {
                POCreateDTO poCreateDTO = (POCreateDTO)TempData["PO"];
                TempData.Keep("PO");
                poCreateDTO.SelectedItems.Single(x => x.Id == stationeryId).Status = confirmStatus;
                TempData["PO"] = poCreateDTO;
                return Json(true, JsonRequestBehavior.AllowGet);
            }
            return Json(false, JsonRequestBehavior.AllowGet);

        }

    }
}