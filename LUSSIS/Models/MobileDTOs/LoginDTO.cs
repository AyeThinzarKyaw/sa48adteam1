using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LUSSIS.Models.MobileDTOs
{
    public class LoginDTO
    {
        public int EmployeeId { get; set; }

        public int RoleId { get; set; }

        public string SessionGuid { get; set; }
        public string Name { get; set; }
    }
}