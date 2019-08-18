using LUSSIS.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LUSSIS.Models.DTOs
{
    public class CatalogueItemDTO
    {
        public int StationeryId { get; set; }

        public string Item { get; set; }

        public string UnitOfMeasure { get; set; }

        public StockAvailabilityEnum StockAvailability { get; set; }

        public int? LowStockAvailability { get; set; }

        public int ReservedCount { get; set; }

        public int WaitlistCount { get; set; }

        public int? OrderQtyInput { get; set; } 

        public bool Confirmation { get; set; }
    }
}