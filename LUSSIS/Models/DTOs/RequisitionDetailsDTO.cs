using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LUSSIS.Models.DTOs
{
    public class RequisitionDetailsDTO
    {
        public LoginDTO LoginDTO {get; set;}

        public int RequisitionFormId { get; set; }

        public string RequestedDate { get; set; }

        public string RequisitionStatus { get; set; }

        public string EmployeeName { get; set; }

        public List<RequisitionDetail> RequisitionDetails { get; set; }

        public string Remarks { get; set; }

    }
}