using LUSSIS.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LUSSIS.Models.DTOs
{
    public class StockAndSupplierDTO
    {
        public int StationeryId { get; set; }

        public string ItemNumber { get; set; }

        public string Category { get; set; }

        public string Description { get; set; }

        public string Location { get; set; }

        public string UnitOfMeasure { get; set; }

        public List<SupplierStockRankDTO> SupplierStockRank { get; set; }

        public List<StockMovementBalanceDTO> StockMovementBalance { get; set; }
    }
}