using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LUSSIS.Models.DTOs
{
    public class RequestedRequisitionDetailsDTO
    {
        public string RequisitionFormId { get; set; }
        public DateTime RequestedDate { get; set; }
        public string Status { get; set; }
        public string EmployeeName { get; set; }
        public List<RequisitionDetail> RequisitionDetails { get; set; }
        public string Remarks { get; set; }

    }
}