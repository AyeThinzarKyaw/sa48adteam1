using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LUSSIS.Models.MobileDTOs
{
    public class StationeryDTO
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public int CategoryId { get; set; }
        public string Description { get; set; }
        public string Bin { get; set; }
        public int Quantity { get; set; }
        public int ReorderLevel { get; set; }
        public int ReorderQuantity { get; set; }
        public string UnitOfMeasure { get; set; }
        public byte[] Image { get; set; }
        public string Status { get; set; }
    }
}