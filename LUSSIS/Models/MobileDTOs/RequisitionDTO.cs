using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LUSSIS.Models.MobileDTOs
{
    public class RequisitionDTO
    {
        public RequisitionDTO()
        {
            this.RequisitionDetails = new HashSet<RequisitionDetailDTO>();
        }
    
        public int Id { get; set; }
        public int EmployeeId { get; set; }
        public System.DateTime DateTime { get; set; }
        public string Status { get; set; }
        public string Remarks { get; set; }

        public virtual EmployeeDTO Employee { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<RequisitionDetailDTO> RequisitionDetails { get; set; }

    }
}