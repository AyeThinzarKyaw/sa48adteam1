using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


namespace LUSSIS.Models.DTOs
{
    public class AssignDeptRepDTO
    {
        public Employee DeptRep { get; set; }
        public int NewDeptRepId { get; set; }
        public IEnumerable<Employee> StaffAndDeptRep { get; set; }
        
    }
}