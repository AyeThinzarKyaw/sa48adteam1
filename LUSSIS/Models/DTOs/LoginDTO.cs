﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LUSSIS.Models.DTOs
{
    public class LoginDTO
    {
        public int EmployeeId { get; set; }

        public int RoleId { get; set; }

        public string SessionGuid { get; set; }
        public string Department { get; set; }

    }
}