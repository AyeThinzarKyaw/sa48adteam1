using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LUSSIS.Models.MobileDTOs
{
    public class RequisitionDetailDTO
    {
        public int Id { get; set; }
        public int RequisitionId { get; set; }
        public int? DisbursementId { get; set; }
        public int StationeryId { get; set; }
        public int QuantityOrdered { get; set; }
        public int QuantityRetrieved { get; set; }
        public int? QuantityDelivered { get; set; }
        public string Status { get; set; }
        public virtual DisbursementDTO Disbursement { get; set; }
        public virtual RequisitionDTO Requisition { get; set; }
        public virtual StationeryDTO Stationery { get; set; }
    }
}