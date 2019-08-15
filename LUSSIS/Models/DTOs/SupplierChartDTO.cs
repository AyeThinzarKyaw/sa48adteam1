using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LUSSIS.Models.DTOs
{
    public class SupplierChartDTO
    {
        public List<SupplierChartDTO> SupplierChartDTOs { get; set; }
        public int Id { get; set; }
        public int PurchaseOrderId { get; set; }
        public int StationeryId { get; set; }
        public int QuantityOrdered { get; set; }
        public int QuantityDelivered { get; set; }
        public string ItemName { get; set; }
        public string ItemType { get; set; }
        public string UnitOfMeasure { get; set; }
        public DateTime OrderDateTime { get; set; }
        public string SupplierName { get; set; }
        public decimal ItemUnitPrice { get; set; }
        public int EmployeeId { get; set; }
        public string EmployeeName { get; set; }

        public PurchaseOrderDetail PurchaseOrderDetailForChart { get; set; }
        public PurchaseOrder PurchaseOrderForChart { get; set; }
        public SupplierTender SupplierTenderForChart { get; set; }
        public Employee EmployeeForChart { get; set; }
        public Supplier SupplierForChart { get; set; }
        public Stationery StationeryForChart { get; set; }
        public Category CategoryForChart { get; set; }

    }
}