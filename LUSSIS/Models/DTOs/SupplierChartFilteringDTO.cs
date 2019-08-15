using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LUSSIS.Models.DTOs
{
    public class SupplierChartFilteringDTO
    {
        public List<SupplierChartFilteringDTO> supplierChartFilteringDTOs { get; set; }
        public List<Supplier> SupplierForChartList { get; set; }
        public List<Stationery> StationeryForChartList { get; set; }
        public List<Category> CategoryForChartList { get; set; }

        public int trend { get; set; }
        public List<int> supplier { get; set; }
        public int category { get; set; }
        public int stationery { get; set; }
        public DateTime selectedDateTime { get; set; }
    }
}