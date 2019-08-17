using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LUSSIS.Models.DTOs
{
    public class ChartDTO
    {
        public List<ChartDTO> SupplierChartDTOs { get; set; }
        public int PubchaseOrderDetailId { get; set; }
        public int PurchaseOrderId { get; set; }
        public int SupplierChartStationeryId { get; set; }
        public int SupplierChartCategoryId { get; set; }
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
        public int SupplierId { get; set; }

        public PurchaseOrderDetail PurchaseOrderDetailForChart { get; set; }
        public PurchaseOrder PurchaseOrderForChart { get; set; }
        public SupplierTender SupplierTenderForChart { get; set; }
        public Employee EmployeeForChart { get; set; }
        public Supplier SupplierForChart { get; set; }
        public Stationery StationeryForChart { get; set; }
        public Category CategoryForChart { get; set; }


        public int RequisitionDetailId { get; set; }
        public int RequisitionId { get; set; }
        public int RequisitionStationeryId { get; set; }
        public int RequisitionQuantityOrdered { get; set; }
        public int RequisitionQuantityDelivered { get; set; }
        public int RequisitionEmployeeId { get; set; }
        public DateTime RequisitionDateTime { get; set; }
        public string RequisitionEmployeeName { get; set; }
        public int RequisitionEmployeeDepartmentId { get; set; }
        public string RequisitionEmployeeDepartmentName { get; set; }
        public int RequisitionStationeryCategoryId { get; set; }
        public string RequisitionStationeryName { get; set; }
        public string RequisitionStationeryCategoryType { get; set; }
        public decimal RequisitionStationeryItemPrice { get; set; }



    }
}