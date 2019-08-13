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

        public List<AdHocDeptAndRetrievalDTO> departmentAndRetrieval { get; set; }

        public int requisitionId { get; set; }
    }
}