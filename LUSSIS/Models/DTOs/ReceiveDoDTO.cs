using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using LUSSIS.Services;

namespace LUSSIS.Models.DTOs
{
    public class ReceiveDoDTO
    {
        public ErrorDTO Error { get; set; }
        public PurchaseOrder purchaseOrder { get; set; }

        [Required(ErrorMessage = "Please upload Delivery Order.")]
        public HttpPostedFileBase DO { get; set; }

        [Required(ErrorMessage = "Please upload Invoice.")]
        public HttpPostedFileBase Invoice { get; set; }

        public int[] DOReceivedList { get; set; }

        //public ReceiveDoDTO(int poId)
        //{
        //    this.purchaseOrder = PurchaseOrderService.Instance.getPurchaseOrderById(poId);
        //    if (this.purchaseOrder.PurchaseOrderDetails.Count > 0)
        //    {
        //        this.DOReceivedList = new int[this.purchaseOrder.PurchaseOrderDetails.Count];
        //        int i = 0;
        //        foreach (var item in this.purchaseOrder.PurchaseOrderDetails)
        //        {
        //            this.DOReceivedList[i] = item.QuantityDelivered == null ? 0 : (int)item.QuantityDelivered;
        //            i++;
        //        }
        //    }
        //}
    }
    public class ReceivedQtyDTO
    {
        public List<KeyValuePair<int, int>> DOReceivedList { get; set; }
        public int PurchaseOrderID { get; set; }
    }
}