using LUSSIS.Filters;
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
        [Authorizer]
        public ActionResult Index()
        {
            if (Session["existinguser"] != null)
            {
                LoginDTO currentUser = (LoginDTO)Session["existinguser"];
                if (currentUser.RoleId != (int)Enums.Roles.StoreClerk && currentUser.RoleId != (int)Enums.Roles.StoreSupervisor)
                {
                    return RedirectToAction("RedirectToClerkOrDepartmentView", currentUser);
                }
                PurchaseOrderListDTO purchaseOrders = new PurchaseOrderListDTO();
                purchaseOrders.PurchaseOrders = PurchaseOrderService.Instance.getAllPurchaseOrders();

                return View(purchaseOrders);
            }
            return RedirectToAction("Index", "Login");
        }

        // POST: Search PO between chosen dates
        //By ATZK
        [Authorizer]
        [HttpPost]
        public ActionResult Index(PurchaseOrderListDTO purchaseOrderListDTO)
        {
            if (Session["existinguser"] != null)
            {
                LoginDTO currentUser = (LoginDTO)Session["existinguser"];
                if (currentUser.RoleId != (int)Enums.Roles.StoreClerk && currentUser.RoleId != (int)Enums.Roles.StoreSupervisor)
                {
                    return RedirectToAction("RedirectToClerkOrDepartmentView", currentUser);
                }

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
            return RedirectToAction("Index", "Login");

        }



        // cancelPOStatus as cancelled
        //By ATZK
        [Authorizer]
        public JsonResult CancelPO(int poId)
        {

            if (Session["existinguser"] != null)
            {
                LoginDTO currentUser = (LoginDTO)Session["existinguser"];
                if (currentUser.RoleId == (int)Enums.Roles.StoreClerk)
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
                return Json(false, JsonRequestBehavior.AllowGet);
            }
            return Json(false, JsonRequestBehavior.AllowGet);
        }

        // View PO Details
        //By ATZK
        [Authorizer]
        public ActionResult Detail(int poId)
        {
            if (Session["existinguser"] != null)
            {
                LoginDTO currentUser = (LoginDTO)Session["existinguser"];
                if (currentUser.RoleId != (int)Enums.Roles.StoreClerk && currentUser.RoleId != (int)Enums.Roles.StoreSupervisor)
                {
                    return RedirectToAction("RedirectToClerkOrDepartmentView", currentUser);
                }
                PurchaseOrder purchaseOrder = PurchaseOrderService.Instance.getPurchaseOrderById(poId);

                return View(purchaseOrder);
            }
            return RedirectToAction("Index", "Login");
        }

        // Approve/Reject PurchaseOrder
        //By ATZK
        [Authorizer]
        public JsonResult ConfirmPO(int poId, string reply, string remark)
        {
            if (Session["existinguser"] != null)
            {
                LoginDTO currentUser = (LoginDTO)Session["existinguser"];
                if (currentUser.RoleId == (int)Enums.Roles.StoreSupervisor)
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
            }
            return Json(new object[] { false, "You don't have permission." }, JsonRequestBehavior.AllowGet);
        }

        [Authorizer]
        public ActionResult ReceiveDO(int poId)
        {
            if (Session["existinguser"] != null)
            {
                LoginDTO currentUser = (LoginDTO)Session["existinguser"];
                if (currentUser.RoleId != (int)Enums.Roles.StoreClerk)
                {
                    return RedirectToAction("RedirectToClerkOrDepartmentView", currentUser);
                }
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
            return RedirectToAction("Index", "Login");
        }

        [Authorizer]
        [HttpPost]
        public ActionResult ReceiveDO(ReceiveDoDTO receiveDO)
        {
            if (Session["existinguser"] != null)
            {
                LoginDTO currentUser = (LoginDTO)Session["existinguser"];
                if (currentUser.RoleId != (int)Enums.Roles.StoreClerk)
                {
                    return RedirectToAction("RedirectToClerkOrDepartmentView", currentUser);
                }
                if (ModelState.IsValid)
                {
                    if (receiveDO.DO.ContentLength > 10 * 1024 * 1024 || receiveDO.Invoice.ContentLength > 10 * 1024 * 1024)
                    {
                        receiveDO.Error.HasError = true;
                        receiveDO.Error.Message = "Each attachment cannot be bigger than 10MB.";

                    }
                    else
                    {
                        PurchaseOrder receivedQtyDTO = new PurchaseOrder();
                        if (TempData["DOReceivedQty"] != null)
                        {
                            receivedQtyDTO = (PurchaseOrder)TempData["DOReceivedQty"];
                            TempData.Keep("DOReceivedQty");
                        }
                        else
                        {
                            receivedQtyDTO = PurchaseOrderService.Instance.getPurchaseOrderById(receiveDO.purchaseOrder.Id);
                        }
                        //save attachments
                        List<string> attachments = new List<string>();
                        var filename = "DO_" + receiveDO.purchaseOrder.Id + "_" + DateTime.Now.ToString("ddMMyyyy_hhmm") + Path.GetExtension(receiveDO.DO.FileName);
                        var path = Path.Combine(Server.MapPath("~/Images/DeliveryOrders/"), filename);

                        receiveDO.DO.SaveAs(path);
                        receiveDO.purchaseOrder.DO = filename;
                        receivedQtyDTO.DO = filename;
                        attachments.Add(path);

                        filename = "Invoice_" + receiveDO.purchaseOrder.Id + "_" + DateTime.Now.ToString("ddMMyyyy_hhmm") + Path.GetExtension(receiveDO.Invoice.FileName);
                        path = Path.Combine(Server.MapPath("~/Images/DeliveryOrders/"), filename);
                        receiveDO.Invoice.SaveAs(path);
                        receiveDO.purchaseOrder.Invoice = filename;
                        receivedQtyDTO.Invoice = filename;
                        attachments.Add(path);

                        receivedQtyDTO.Status = Enum.GetName(typeof(Enums.POStatus), Enums.POStatus.CLOSED);

                        //save updatedPO
                        receivedQtyDTO.DeliveryDateTime = DateTime.Now;
                        receivedQtyDTO.DORemark = receiveDO.purchaseOrder.DORemark;
                        receivedQtyDTO.DeliveryOrderNo = receiveDO.purchaseOrder.DeliveryOrderNo;
                        PurchaseOrderService.Instance.UpdatePO(receivedQtyDTO);

                        foreach (var detail in receivedQtyDTO.PurchaseOrderDetails)
                        {

                            //update stationery qty
                            Stationery s = StationeryService.Instance.GetStationeryById(detail.StationeryId);
                            s.Quantity += detail.QuantityDelivered == null ? 0 : (int)detail.QuantityDelivered;
                            StationeryService.Instance.UpdateStationery(s);

                            //check whether to raise AdjVoucher (eg: gift, extra)
                            if (detail.QuantityOrdered < detail.QuantityDelivered)
                            {
                                //raise adjustment voucher
                                AdjustmentVoucherDetail adjustmentItem = new AdjustmentVoucherDetail();
                                adjustmentItem.DateTime = DateTime.Now;
                                adjustmentItem.Quantity = (int)(detail.QuantityDelivered - detail.QuantityOrdered);
                                adjustmentItem.Reason = "Received extra (eg: gift) on Delivery Order Receive";
                                //AdjustmentVoucherService.Instance.CreateAdjustmentVoucher(adjustmentItem);
                            }


                        }
                        //Check to move waitlistApproved to Preparing
                        RequisitionCatalogueService.Instance.CheckStockAndUpdateStatusForWaitlistApprovedRequisitionDetails(receivedQtyDTO.Id);

                        EmailNotificationService.Instance.SendNotificationEmail(receipient: "sa48team1@gmail.com", subject: "(Stationery Store) Delivery Order and Invoice for " + DateTime.Now.ToString("dd/MM/yyyy"), body: "Delivery Order and Invoices for Purchase Orders of Stationery Store are attached.", attachments: attachments.AsEnumerable());
                        TempData["DOReceivedQty"] = null;
                        return RedirectToAction("Index");

                    }

                }

                PurchaseOrder po = PurchaseOrderService.Instance.getPurchaseOrderById(receiveDO.purchaseOrder.Id);
                po.Remark = receiveDO.purchaseOrder.Remark;
                po.DO = receiveDO.purchaseOrder.DO;
                receiveDO.purchaseOrder = po;
                return View(receiveDO);
            }
            return RedirectToAction("Index", "Login");
        }
        // GET: update received qty in DO
        //By ATZK
        [Authorizer]
        public JsonResult UpdateReceivedQty(int podId, int qty, int poId)
        {
            if (Session["existinguser"] != null)
            {
                LoginDTO currentUser = (LoginDTO)Session["existinguser"];
                if (currentUser.RoleId == (int)Enums.Roles.StoreClerk)
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
                }
            }
            return Json(false, JsonRequestBehavior.AllowGet);
        }



        // GET: View PurchaseOrder Catalogue to select items
        //By ATZK
        [Authorizer]
        public ActionResult Catalogue()
        {
            if (Session["existinguser"] != null)
            {
                LoginDTO currentUser = (LoginDTO)Session["existinguser"];
                if (currentUser.RoleId != (int)Enums.Roles.StoreClerk)
                {
                    return RedirectToAction("RedirectToClerkOrDepartmentView", currentUser);
                }
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
            return RedirectToAction("Index", "Login");
        }

        // GET: update select category on PO Catalogue
        //By ATZK
        [Authorizer]
        public JsonResult updateSelectList(int stationeryId, int qty, bool selectOrNot, bool selectAllSelected)
        {
            if (Session["existinguser"] != null)
            {
                LoginDTO currentUser = (LoginDTO)Session["existinguser"];
                if (currentUser.RoleId == (int)Enums.Roles.StoreClerk)
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
            }
            return Json(false, JsonRequestBehavior.AllowGet);
        }

        // render partial view as string html
        //By ATZK
        [Authorizer]
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

        //By ATZK
        [Authorizer]
        public ActionResult SelectSupplier()
        {
            if (Session["existinguser"] != null)
            {
                LoginDTO currentUser = (LoginDTO)Session["existinguser"];
                if (currentUser.RoleId != (int)Enums.Roles.StoreClerk)
                {
                    return RedirectToAction("RedirectToClerkOrDepartmentView", currentUser);
                }
                if (TempData["PO"] == null)
                {
                    return RedirectToAction("Catalogue");
                }
                POCreateDTO poCreateDTO = (POCreateDTO)TempData["PO"];
                TempData.Keep("PO");
                return View(poCreateDTO);
            }
            return RedirectToAction("Index", "Login");
        }



        //By ATZK
        [Authorizer]
        [HttpPost]
        public JsonResult RaisePO(bool removeZero)
        {
            if (Session["existinguser"] != null)
            {
                LoginDTO currentUser = (LoginDTO)Session["existinguser"];
                if (currentUser.RoleId == (int)Enums.Roles.StoreClerk)
                {
                    POCreateDTO poCreateDTO = (POCreateDTO)TempData["PO"];
                    TempData.Keep("PO");
                    var ids = poCreateDTO.SelectedItems.Select(x => x.Id).ToList();
                    int zeroCount = poCreateDTO.Catalogue.Where(x => ids.Contains(x.Id)).Where(o => o.Unsubmitted == 0).Count();
                    if (zeroCount > 0 && removeZero == false)
                    {
                        return Json(new object[] { true, "Currently, there are some stationery of Order Quantity fills as 0. Stationery with Order Quantity 0 will not raise PO. Click OK to continue raising PO for other stationeries. Click CANCEL to edit." }, JsonRequestBehavior.AllowGet);
                    }

                    if (poCreateDTO.SelectedItems.Where(s => s.CategoryId != 0 && s.Status == "confirmed").Count() > 0)
                    {
                        PurchaseOrderService.Instance.RaisePO(poCreateDTO, currentUser.EmployeeId);
                        TempData["PO"] = null;
                        return Json(new object[] { true, null }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json(new object[] { false, "Currently, there is no confirmed stationery to raise PO. Please confirm first." }, JsonRequestBehavior.AllowGet);
                    }
                }
            }
            return Json(new object[] { false, "You don't have permission." }, JsonRequestBehavior.AllowGet);


        }


        //By ATZK
        [Authorizer]
        [HttpPost]
        public JsonResult UpdateSelectedSupplier(int stationeryId, int supplierId)
        {
            if (Session["existinguser"] != null)
            {
                LoginDTO currentUser = (LoginDTO)Session["existinguser"];
                if (currentUser.RoleId == (int)Enums.Roles.StoreClerk)
                {
                    POCreateDTO poCreateDTO = (POCreateDTO)TempData["PO"];
                    TempData.Keep("PO");
                    poCreateDTO.SelectedItems.Single(x => x.Id == stationeryId).CategoryId = supplierId;
                    TempData["PO"] = poCreateDTO;
                    return Json(true, JsonRequestBehavior.AllowGet);
                }

            }
            return Json(false, JsonRequestBehavior.AllowGet);
        }


        //By ATZK
        [Authorizer]
        [HttpPost]
        public JsonResult UpdateEstimatedDate(DateTime estDate, int supplierId)
        {
            if (Session["existinguser"] != null)
            {
                LoginDTO currentUser = (LoginDTO)Session["existinguser"];
                if (currentUser.RoleId == (int)Enums.Roles.StoreClerk)
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
            }
            return Json(false, JsonRequestBehavior.AllowGet);
        }


        //By ATZK
        [Authorizer]
        [HttpPost]
        public JsonResult RemoveItem(int stationeryId)
        {
            if (Session["existinguser"] != null)
            {
                LoginDTO currentUser = (LoginDTO)Session["existinguser"];
                if (currentUser.RoleId == (int)Enums.Roles.StoreClerk)
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
            }
            return Json(false, JsonRequestBehavior.AllowGet);
        }


        //By ATZK
        [Authorizer]
        [HttpPost]
        public JsonResult ConfirmItemToPO(int stationeryId, string confirmStatus)
        {
            if (Session["existinguser"] != null)
            {
                LoginDTO currentUser = (LoginDTO)Session["existinguser"];
                if (currentUser.RoleId == (int)Enums.Roles.StoreClerk)
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
            return Json(false, JsonRequestBehavior.AllowGet);
        }

    }
}