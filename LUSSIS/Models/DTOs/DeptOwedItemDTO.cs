﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LUSSIS.Models.DTOs
{
    public class DeptOwedItemDTO
    {
        public Department Department { get; set; }

        public List<OwedItemDTO> OwedItems { get; set; }
    }
}