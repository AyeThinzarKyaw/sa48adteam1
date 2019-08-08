using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LUSSIS.Models.MobileDTOs
{
    public class DisbursementDTO
    {
        public DisbursementDTO()
        {
            this.RequisitionDetails = new HashSet<RequisitionDetailDTO>();
        }

        public int Id { get; set; }
        public int DeliveredEmployeeId { get; set; }
        public int ReceivedEmployeeId { get; set; }
        public bool AdHoc { get; set; }
        public int DeliveryDateTime { get; set; }
        public string CollectionPoint { get; set; }
        public byte[] Signature { get; set; }

        public virtual EmployeeDTO Employee { get; set; }
        public virtual EmployeeDTO Employee1 { get; set; }
        public virtual ICollection<RequisitionDetailDTO> RequisitionDetails { get; set; }
    }
}