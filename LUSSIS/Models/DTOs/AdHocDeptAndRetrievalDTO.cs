﻿using LUSSIS.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LUSSIS.Models.DTOs
{
    public class AdHocDeptAndRetrievalDTO
    {
        //hidden

        public Department department { get; set; }
        public List<Requisition> requisitions { get; set; }

    }
}