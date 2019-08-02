using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LUSSIS.Models.DTOs
{
    //can use this VM for My Requisition History also and My Requisition Detail
    public class DeptRequisitionListDTO
    {
        public LoginDTO LoginDTO { get; set; }

        public List<Requisition> Requisitions { get; set; }

    }
}