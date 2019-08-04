using LUSSIS.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LUSSIS.Models.DTOs
{
    public class StockMovementDTO
    {

        public DateTime MovementDate { get; set; }
        public String DepartmentOrSupplier { get; set; }
        public int Quantity { get; set; }

    }
}