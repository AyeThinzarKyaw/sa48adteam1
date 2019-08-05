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

        // GET: View PurchaseOrder Catalogue to select items
        //By ATZK
        public ActionResult Catalogue()
        {
            return View(PurchaseOrderService.Instance.RetrievePurchaseOrderCatalogue());
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
            //if (receiveDO.DO==null || receiveDO.Invoice==null )
            //{
            //    //receiveDO.Error = new ErrorDTO();
            //    receiveDO.Error.HasError = true;
            //    receiveDO.Error.Message = "Please attach both Delivery Order and Invoice.";
            //    return View(receiveDO);
            //}
            //else
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
                        PurchaseOrder receivedQtyDTO = (PurchaseOrder)TempData["DOReceivedQty"];
                        TempData.Keep("DOReceivedQty");


                        PurchaseOrder updatedPO = PurchaseOrderService.Instance.getPurchaseOrderById(receiveDO.purchaseOrder.Id);

                        //save attachments
                        //var filename = Path.GetFileName(receiveDO.DO.FileName);

                        var filename = "DO_" + receiveDO.purchaseOrder.Id+"_"+DateTime.Now.ToString("ddMMyyyy_hhmm")+ Path.GetExtension(receiveDO.DO.FileName);
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
                            int oldQty = oldPODetail.QuantityDelivered == null ? 0 : (int)oldPODetail.QuantityDelivered;
                            oldPODetail.QuantityDelivered = oldQty+qtyDeliveredNow;//qtyInDB+nowReceivedQty
                            //check to complete PO
                            if (oldPODetail.QuantityOrdered > oldPODetail.QuantityDelivered)
                            {
                                poComplete = false;
                            }
                            

                            PurchaseOrderService.Instance.UpdatePODetail(oldPODetail);

                            //update stationery qty
                            Stationery s = StationeryService.Instance.GetStationeryById(detail.StationeryId);
                            s.Quantity = s.Quantity+qtyDeliveredNow;
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
                        if (poComplete==true)
                        {
                            updatedPO.Status = Enum.GetName(typeof(Enums.POStatus), Enums.POStatus.CLOSED);
                        }

                        //save updatedPO
                        updatedPO.DeliveryDateTime = DateTime.Now;
                        PurchaseOrderService.Instance.UpdatePO(updatedPO);

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
                po.PurchaseOrderDetails.Single(x => x.Id == podId).QuantityDelivered = qty;

                TempData["DOReceivedQty"] = po;
            }

            return Json(true, JsonRequestBehavior.AllowGet);


            //ReceivedQtyDTO receivedQtyDTO=new ReceivedQtyDTO();
            //if (TempData["DOReceivedQty"]!=null)
            //{
            //    receivedQtyDTO = (ReceivedQtyDTO)TempData["DOReceivedQty"];
            //    TempData.Keep("DOReceivedQty");
            //    if (receivedQtyDTO.DOReceivedList.Where(x => x.Key == podId).Count() == 1)
            //    {
            //        if (receivedQtyDTO.DOReceivedList.SingleOrDefault(x => x.Key == podId).Value!=qty)
            //        {
            //            receivedQtyDTO.DOReceivedList.Remove(receivedQtyDTO.DOReceivedList.SingleOrDefault(x => x.Key == podId));
            //            receivedQtyDTO.DOReceivedList.Add(new KeyValuePair<int, int>(podId, qty));
            //        }
            //    }
            //    else
            //    {
            //       receivedQtyDTO.DOReceivedList.Add(new KeyValuePair<int, int>(podId, qty));
            //    }
            //}
            //else
            //{
            //    receivedQtyDTO.PurchaseOrderID = poId;
            //    receivedQtyDTO.DOReceivedList = new List<KeyValuePair<int, int>>();
            //    receivedQtyDTO.DOReceivedList.Add(new KeyValuePair<int, int>(podId, qty));
            //}


            //TempData["DOReceivedQty"] = receivedQtyDTO;
            //return Json(true, JsonRequestBehavior.AllowGet);



            //if (receivedQtyDTO.PurchaseOrderID==poId)
            //{
            //    if (receivedQtyDTO.DOReceivedList.Where(x=>x.Key==podId).Count()==1)
            //    {
            //        receivedQtyDTO.DOReceivedList.Remove(receivedQtyDTO.DOReceivedList.SingleOrDefault(x => x.Key == podId));

            //    }
            //    receivedQtyDTO.DOReceivedList.Add(new KeyValuePair<int, int>(podId, qty));
            //    TempData["DOReceivedQty"] = receivedQtyDTO;
            //    return Json(true, JsonRequestBehavior.AllowGet);
            //}
            //else
            //{
            //    this.receivedQtyDTO.PurchaseOrderID = poId;
            //    this.receivedQtyDTO.DOReceivedList.Add(new KeyValuePair<int, int>(podId, qty));
            //    return Json(true, JsonRequestBehavior.AllowGet);
            //}
            //DOReceivedList.Add(new KeyValuePair<int, int>(0, 20));
            //int a=DOReceivedList[0].Value;

        }

        public ActionResult SelectSupplier()
        {
            return View();
        }

    }
}