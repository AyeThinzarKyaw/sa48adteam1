using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LUSSIS.Models.DTOs
{
    public class RequisitionsDTO
    {
        public LoginDTO LoginDTO { get; set; }

        public List<Requisition> Requisitions { get; set; }

    }
}