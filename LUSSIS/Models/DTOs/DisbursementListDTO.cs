using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using LUSSIS.Enums;

namespace LUSSIS.Models.DTOs
{
    public class DisbursementListDTO
    {
        public List<DisbursementListDTO> disbursementDTOList { get; set; }
        public int Id { get; set; }
        public int DisbursementId { get; set; }
        public int DeliveredEmployeeId { get; set; }
        public int ReceivedEmployeeId { get; set; }
        public DateTime DeliveryDateTime { get; set; }
        public string CollectionPoint { get; set; }
        public int QuantityOrdered { get; set; }
        public int QuantityDelivered { get; set; }
        public string Status { get; set; }
        public string UnitOfMeasure { get; set; }
        public string DepartmentName { get; set; }
        public int EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public int DepartmentId { get; set; }
        public string DepartmentCode { get; set; }
        public string DepartmentContactName { get; set; }
        public string DepartmentContactNumber { get; set; }
        public string Category { get; set; }
        public int StationeryId { get; set; }
        public string ItemName { get; set; }
        public string ItemCode { get; set; }
        public int SumOfRequestedQuantity { get; set; }
        public int SumOfDisbursedQuantity { get; set; }
        public int OutstandingQuantity { get; set; }
        public int RequisitionID { get; set; }
        public DateTime RequisitionDateTime { get; set; }
        public List<RequisitionDetail> RequisitionDetails { get; set; }

    }
}