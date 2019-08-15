using LUSSIS.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LUSSIS.Models.DTOs
{
    public class AdHocDeptAndRetrievalDTO
    {
        //hidden

        public Department Department { get; set; }
        public List<Requisition> Requisitions { get; set; }

    }
}