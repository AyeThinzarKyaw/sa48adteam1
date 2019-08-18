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
    }

    public class ReceivedQtyDTO
    {
        public List<KeyValuePair<int, int>> DOReceivedList { get; set; }

        public int PurchaseOrderID { get; set; }
    }
}