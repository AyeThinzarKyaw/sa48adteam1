using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LUSSIS.Models.MobileDTOs
{
    public class DisbursementDTO
    {
        public int Id { get; set; }
        public int DeliveredEmployeeId { get; set; }
        public int ReceivedEmployeeId { get; set; }
        public string ReceivedEmployeeName { get; set; }
        public string DepartmentName { get; set; }
        public bool AdHoc { get; set; }
        public DateTime DeliveryDateTime { get; set; }
        public string CollectionPoint { get; set; }
        public string Signature { get; set; }
        public List<RequisitionDetailDTO> RequisitionDetails { get; set; }
    }
}