using LUSSIS.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LUSSIS.Models.DTOs
{
    public class StaffRequisitionDTO
    {
        public int RequisitionID { get; set; }

        public string RequestedDate { get; set; }

        public RequisitionStatusEnum Status { get; set; }
    }

}