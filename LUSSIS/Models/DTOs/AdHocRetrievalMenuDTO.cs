using LUSSIS.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LUSSIS.Models.DTOs
{
    public class AdHocRetrievalMenuDTO
    {
        //hidden

        public List<AdHocDeptAndRetrievalDTO> DepartmentAndRetrieval { get; set; }

        public int RequisitionId { get; set; }
    }
}