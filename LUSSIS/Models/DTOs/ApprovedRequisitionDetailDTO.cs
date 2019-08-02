using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LUSSIS.Models.DTOs
{
    public class ApprovedRequisitionDetailDTO
    {
        public LoginDTO LoginDTO {get; set;}

        public int RequisitionDetailId { get; set; }
    }
}