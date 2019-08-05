using LUSSIS.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LUSSIS.Models.DTOs
{
    public class StockMovementBalanceDTO
    {
        public StockMovementDTO StockMovement { get; set; }
        // hidden in View
        public int Balance { get; set; }
    }
}