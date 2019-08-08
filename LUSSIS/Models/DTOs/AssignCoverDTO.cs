using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LUSSIS.Models.DTOs
{
    public class AssignCoverDTO
    {
        public LoginDTO LoginDTO { get; set; }
        public ErrorDTO Error { get; set; }

        public IEnumerable<Employee> StaffAndCoverHead { get; set; }
        public IEnumerable<DepartmentCoverEmployee> ActiveCoverHeadDetails { get; set; }
        public IEnumerable<DepartmentCoverEmployee> PastCoverHeadDetails { get; set; }
        public int NewCoverHeadId { get; set; }
        //[Required(ErrorMessage = "FromDate required")]
        public System.DateTime FromDate { get; set; }
        // [Required(ErrorMessage = "ToDate required")]
        public System.DateTime ToDate { get; set; }

    }
}