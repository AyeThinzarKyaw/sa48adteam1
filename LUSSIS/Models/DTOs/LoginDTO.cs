using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LUSSIS.Models.DTOs
{
    public class LoginDTO
    {
        public int EmployeeId { get; set; }

        public Role EmployeeRole { get; set; }

        public int SessionId { get; set; }

        public List<CartDetail> CartDetails { get; set; }

    }
}