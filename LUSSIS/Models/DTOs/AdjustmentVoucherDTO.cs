using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace LUSSIS.Models.DTOs
{
    public class AdjustmentVoucherDTO
    {
        public ErrorDTO Error { get; set; }

        public int StationeryId { get; set; }

        public AdjustmentVoucher adjustmentVoucher { get; set; }

        public float TotalAmount { get; set; }
        
        public IEnumerable<Stationery> Stationeries { get; set; }

        [Required(ErrorMessage = "Adjusted Qty is required.")]
        
        public int Quantity { get; set; }

        public string Reason { get; set; }
    }
}