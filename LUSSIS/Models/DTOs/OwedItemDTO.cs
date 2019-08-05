using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LUSSIS.Models.DTOs
{
    public class OwedItemDTO
    {
        public Stationery Stationery{get; set;}

        public int QtyOwed { get; set; }

        //public int InStock { get; set; }

        //public int Waitlist { get; set; }

    }
}