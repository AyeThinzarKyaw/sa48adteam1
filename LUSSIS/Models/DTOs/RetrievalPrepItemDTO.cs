using LUSSIS.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LUSSIS.Models.DTOs
{
    public class RetrievalPrepItemDTO
    {
        public Stationery ReqStationery { get; set; }

        public RequisitionDetail ReqDetail { get; set; }

        public Requisition Req { get; set; }

        public Employee ReqOwner { get; set; }

        public Employee ReqDepartmentRep { get; set; }

        public Department ReqDepartment { get; set; }

        public CollectionPoint ReqCollectionPoint { get; set; }
    }
}