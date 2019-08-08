using LUSSIS.Models;
using LUSSIS.Models.DTOs;
using LUSSIS.Repositories;
using LUSSIS.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LUSSIS.Services
{
    public class PurchaseOrderService :IPurchaseOrderService
    {
        private PurchaseOrderService() { }

        private static PurchaseOrderService instance = new PurchaseOrderService();
        public static IPurchaseOrderService Instance
        {
            get { return instance; }
        }

        public IEnumerable<PurchaseOrder> getAllPurchaseOrders()
        {
            return PurchaseOrderRepo.Instance.FindAll().OrderBy(x=>x.OrderDateTime).ToList();
        }

        public PurchaseOrder getPurchaseOrderById(int poId)
        {
            return PurchaseOrderRepo.Instance.FindById(poId);
        }

        public IEnumerable<PO_getPOCatalogue_Result> RetrievePurchaseOrderCatalogue()
        {
            return PurchaseOrderRepo.Instance.GetPOCatalogue();
        }
        public void CreatePO(PurchaseOrder po)
        {
            po.OrderDateTime = DateTime.Now;
            po.EmployeeId = 1;//TODO: to change dynamic data
            PurchaseOrderRepo.Instance.Create(po);
        }
        public void UpdatePO(PurchaseOrder po)
        {
            PurchaseOrderRepo.Instance.Update(po);
        }
        public void CreatePODetail(PurchaseOrderDetail pod)
        {
            PurchaseOrderDetailRepo.Instance.Create(pod);
        }

        public void UpdatePODetail(PurchaseOrderDetail pod)
        {
            PurchaseOrderDetailRepo.Instance.Update(pod);
        }

        public void RaisePO(POCreateDTO poCreateDTO)
        {
            foreach (Stationery item in poCreateDTO.SelectedItems.Where(s=>s.CategoryId!=0 && s.Status=="confirmed"))
            {
                if (poCreateDTO.Catalogue.Single(c => c.Id == item.Id).Unsubmitted > 0)
                {

                    PurchaseOrder po = new PurchaseOrder();

                    PurchaseOrderDetail pod = new PurchaseOrderDetail();
                    pod.StationeryId = item.Id;
                    pod.QuantityOrdered = poCreateDTO.Catalogue.Single(c => c.Id == item.Id).Unsubmitted;

                    if (poCreateDTO.ConfirmedPOs.Count == 0 || poCreateDTO.ConfirmedPOs.Where(x => x.SupplierId == item.CategoryId).Count() <= 0)
                    {
                        //if no Confirm PO for this supplier yet, create new PO
                        po.SupplierId = item.CategoryId;
                        po.Status = Enum.GetName(typeof(Enums.POStatus), Enums.POStatus.PENDING);
                        po.EstDeliveryDate = poCreateDTO.EstimatedDates.Single(e => e.Key == po.SupplierId).Value;

                        pod.PurchaseOrderId = po.Id;
                        po.PurchaseOrderDetails.Add(pod);
                        poCreateDTO.ConfirmedPOs.Add(po);
                    }
                    else
                    {
                        po = poCreateDTO.ConfirmedPOs.Single(x => x.SupplierId == item.CategoryId);
                        pod.PurchaseOrderId = po.Id;

                        poCreateDTO.ConfirmedPOs.Single(x => x.Id == po.Id).PurchaseOrderDetails.Add(pod);
                    }
                }

                
            }

            foreach (PurchaseOrder purchaseOrder in poCreateDTO.ConfirmedPOs)
            {
                PurchaseOrderService.Instance.CreatePO(purchaseOrder);
                //if (purchaseOrder.Id != 0)
                //{
                //    foreach (PurchaseOrderDetail detail in purchaseOrder.PurchaseOrderDetails)
                //    {
                //        detail.PurchaseOrderId = purchaseOrder.Id;
                //        PurchaseOrderService.Instance.CreatePODetail(detail);
                //    }
                //}
            }
        }

        //public void RecordReceiveDO(ReceiveDoDTO receiveDO)
        //{
        //    var filename = "DO_" + receiveDO.purchaseOrder.Id + "_" + DateTime.Now.ToString("ddMMyyyy_hhmm") + Path.GetExtension(receiveDO.DO.FileName);
        //    var path = Path.Combine(Server.MapPath("~/Images/DeliveryOrders/"), filename);

        //    receiveDO.DO.SaveAs(path);
        //    receiveDO.purchaseOrder.DO = filename;
        //    updatedPO.DO = filename;


        //    filename = "Invoice_" + receiveDO.purchaseOrder.Id + "_" + DateTime.Now.ToString("ddMMyyyy_hhmm") + Path.GetExtension(receiveDO.Invoice.FileName);
        //    path = Path.Combine(Server.MapPath("~/Images/DeliveryOrders/"), filename);
        //    receiveDO.Invoice.SaveAs(path);
        //    receiveDO.purchaseOrder.Invoice = filename;
        //    updatedPO.Invoice = filename;

        //    bool poComplete = true;
        //    //update POdetails deliveredQty
        //    foreach (var detail in receivedQtyDTO.PurchaseOrderDetails)
        //    {

        //        PurchaseOrderDetail oldPODetail = updatedPO.PurchaseOrderDetails.Single(x => x.Id == detail.Id);
        //        int qtyDeliveredNow = detail.QuantityDelivered == null ? 0 : (int)detail.QuantityDelivered;
        //        //int oldQty = oldPODetail.QuantityDelivered == null ? 0 : (int)oldPODetail.QuantityDelivered;
        //        //int newQty= oldQty + qtyDeliveredNow;//qtyInDB+nowReceivedQty
        //        //oldPODetail.QuantityDelivered = oldQty + qtyDeliveredNow;//qtyInDB+nowReceivedQty
        //        //check to complete PO
        //        if (oldPODetail.QuantityOrdered > oldPODetail.QuantityDelivered)
        //        {
        //            poComplete = false;
        //        }


        //        //PurchaseOrderService.Instance.UpdatePODetail(oldPODetail);

        //        //update stationery qty
        //        Stationery s = StationeryService.Instance.GetStationeryById(detail.StationeryId);
        //        s.Quantity = s.Quantity + qtyDeliveredNow;
        //        StationeryService.Instance.UpdateStationery(s);

        //        //check whether to raise AdjVoucher (eg: gift, extra)
        //        if (oldPODetail.QuantityOrdered < oldPODetail.QuantityDelivered)
        //        {
        //            //raise adjustment voucher
        //            AdjustmentVoucherDetail adjustmentItem = new AdjustmentVoucherDetail();
        //            adjustmentItem.DateTime = DateTime.Now;
        //            adjustmentItem.Quantity = (int)(oldPODetail.QuantityDelivered - oldPODetail.QuantityOrdered);
        //            adjustmentItem.Reason = "Received extra (eg: gift) on Delivery Order Receive";
        //        }

        //        //Check to move waitlistApproved to Preparing
        //    }
        //    if (poComplete == true)
        //    {
        //        updatedPO.Status = Enum.GetName(typeof(Enums.POStatus), Enums.POStatus.CLOSED);
        //    }

        //    //save updatedPO
        //    updatedPO.DeliveryDateTime = DateTime.Now;
        //    updatedPO.Remark = receivedQtyDTO.Remark;
        //    updatedPO.DeliveryOrderNo = receivedQtyDTO.DeliveryOrderNo;
        //    PurchaseOrderService.Instance.UpdatePO(updatedPO);
        //    EmailNotificationService.Instance.SendNotificationEmail("e0395895@u.nus.edu", "test test test AD", "successfully sent out", new string[] { Path.Combine(Server.MapPath("~/Images/DeliveryOrders/"), updatedPO.DO), Path.Combine(Server.MapPath("~/Images/DeliveryOrders/"), updatedPO.Invoice) });
        //}
        
    }
}