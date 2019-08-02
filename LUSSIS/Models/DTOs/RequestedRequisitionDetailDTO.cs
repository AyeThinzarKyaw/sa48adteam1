using LUSSIS.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LUSSIS.Models.DTOs
{
    public class RequestedRequisitionDetailDTO
    {
        public string RequisitionDetailId { get; set; }
        public string Item { get; set; }
        public string UnitOfMeasure { get; set; }
        public int OrderQty { get; set; }
        public RequisitionDetailStatusEnum Status { get; set; }
    }
}