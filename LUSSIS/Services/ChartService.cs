using LUSSIS.Models;
using LUSSIS.Models.DTOs;
using LUSSIS.Repositories;
using LUSSIS.Repositories.Interfaces;
using LUSSIS.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LUSSIS.Services
{
    public class ChartService : IChartService
    {
        private IDisbursementRepo disbursementRepo;
        private IStationeryRepo stationeryRepo;
        private ICategoryRepo categoryRepo;
        private ISupplierTenderRepo supplierTenderRepo;
        private ISupplierRepo supplierRepo;
        private IAdjustmentVoucherRepo adjustmentVoucherRepo;
        private IAdjustmentVoucherDetailRepo adjustmentVoucherDetailRepo;
        private IRequisitionRepo requisitionRepo;
        private IRequisitionDetailRepo requisitionDetailRepo;
        private IEmployeeRepo employeeRepo;
        private IDepartmentRepo departmentRepo;
        private IPurchaseOrderRepo purchaseOrderRepo;
        private IPurchaseOrderDetailRepo purchaseOrderDetailRepo;
        public ChartService()
        {
            stationeryRepo = StationeryRepo.Instance;
            categoryRepo = CategoryRepo.Instance;
            supplierTenderRepo = SupplierTenderRepo.Instance;
            supplierRepo = SupplierRepo.Instance;
            adjustmentVoucherRepo = AdjustmentVoucherRepo.Instance;
            adjustmentVoucherDetailRepo = AdjustmentVoucherDetailRepo.Instance;
            requisitionRepo = RequisitionRepo.Instance;
            requisitionDetailRepo = RequisitionDetailRepo.Instance;
            disbursementRepo = DisbursementRepo.Instance;
            employeeRepo = EmployeeRepo.Instance;
            departmentRepo = DepartmentRepo.Instance;
            purchaseOrderRepo = PurchaseOrderRepo.Instance;
            purchaseOrderDetailRepo = PurchaseOrderDetailRepo.Instance;
        }

        private static ChartService instance = new ChartService();

        public static IChartService Instance
        {
            get { return instance; }
        }

        public List<ChartDTO> TrendChartInfoForSupplier(int SupplierId, int CategoryId, int StationeryId)
        {
            List<PurchaseOrderDetail> PurchaseOrderDetailsList = purchaseOrderDetailRepo.GetPurchaseOrderDetailsBySupplierId(SupplierId);
            IEnumerable<SupplierTender> SupplierTendersList = supplierTenderRepo.FindBy(x => x.SupplierId == SupplierId);

            List<ChartDTO> chartDTOs = new List<ChartDTO>();

            foreach (PurchaseOrderDetail pod in PurchaseOrderDetailsList)
            {
                foreach (SupplierTender st in SupplierTendersList)
                {
                    if (pod.StationeryId == st.StationeryId && pod.PurchaseOrder.SupplierId == st.SupplierId && pod.Stationery.CategoryId == CategoryId && pod.StationeryId == StationeryId)
                    {

                        ChartDTO chartDTO = new ChartDTO()
                        {
                            PubchaseOrderDetailId = pod.Id,
                            PurchaseOrderId = pod.PurchaseOrderId,
                            SupplierChartStationeryId = pod.StationeryId,
                            SupplierChartCategoryId = pod.Stationery.Category.Id,
                            QuantityOrdered = pod.QuantityOrdered,
                            QuantityDelivered = pod.QuantityDelivered == null ? 0 : (int)pod.QuantityDelivered,
                            ItemName = pod.Stationery.Description,
                            ItemType = pod.Stationery.Category.Type,
                            UnitOfMeasure = pod.Stationery.UnitOfMeasure,
                            OrderDateTime = pod.PurchaseOrder.OrderDateTime,
                            SupplierName = pod.PurchaseOrder.Supplier.Name,
                            ItemUnitPrice = st.Price,
                            EmployeeId = pod.PurchaseOrder.Employee.Id,
                            EmployeeName = pod.PurchaseOrder.Employee.Name,
                            SupplierId = pod.PurchaseOrder.SupplierId,

                            PurchaseOrderDetailForChart = pod,
                            EmployeeForChart = pod.PurchaseOrder.Employee,
                            SupplierForChart = pod.PurchaseOrder.Supplier,
                            StationeryForChart = pod.Stationery,
                            PurchaseOrderForChart = pod.PurchaseOrder,
                            SupplierTenderForChart = st,
                            CategoryForChart = pod.Stationery.Category,
                        };
                        chartDTOs.Add(chartDTO);

                    }

                }
            }
            return chartDTOs;
        }
        public ChartFilteringDTO FilteringByAttributes()
        {
            List<Supplier> suppliers = (List<Supplier>)supplierRepo.FindAll();
            List<Stationery> stationeries = (List<Stationery>)stationeryRepo.FindAll();
            List<Category> categories = (List<Category>)categoryRepo.FindAll();
            List<Department> departments = (List<Department>)departmentRepo.FindAll();

            ChartFilteringDTO FilteringDetails = new ChartFilteringDTO()
            {
                SupplierForChartList = suppliers,
                StationeryForChartList = stationeries,
                CategoryForChartList = categories,
                DepartmentForChartList = departments,
            };
            return FilteringDetails;
        }

        public List<ChartDTO> TrendChartInfoForDepartment(int DepartmentId, int CategoryId, int StationeryId)
        {
            List<RequisitionDetail> RequisitionDetailsList = requisitionDetailRepo.GetRequisitionDetailsByDepartmentIdByCategoryIdByStationeryId(DepartmentId, CategoryId, StationeryId);
            IEnumerable<SupplierTender> SupplierTendersList = supplierTenderRepo.FindBy(x => x.StationeryId == StationeryId);
            List<ChartDTO> chartDTOs = new List<ChartDTO>();
            foreach (RequisitionDetail rdl in RequisitionDetailsList)
            {
                foreach (SupplierTender st in SupplierTendersList)
                {
                    if (rdl.StationeryId == st.StationeryId && rdl.Stationery.CategoryId == CategoryId && rdl.StationeryId == StationeryId && rdl.Requisition.Employee.DepartmentId == DepartmentId)
                    {
                        ChartDTO chartDTO = new ChartDTO()
                        {
                            RequisitionDetailId = rdl.Id,
                            RequisitionId = rdl.RequisitionId,
                            RequisitionStationeryId = rdl.StationeryId,
                            RequisitionQuantityOrdered = rdl.QuantityOrdered,
                            RequisitionQuantityDelivered = rdl.QuantityDelivered == null ? 0 : (int)rdl.QuantityDelivered,
                            RequisitionDateTime = rdl.Requisition.DateTime,
                            RequisitionEmployeeDepartmentId = rdl.Requisition.Employee.DepartmentId,
                            RequisitionEmployeeDepartmentName = rdl.Requisition.Employee.Department.DepartmentName,
                            RequisitionEmployeeId = rdl.Requisition.EmployeeId,
                            RequisitionEmployeeName = rdl.Requisition.Employee.Name,
                            RequisitionStationeryCategoryId = rdl.Stationery.CategoryId,
                            RequisitionStationeryCategoryType = rdl.Stationery.Category.Type,
                            RequisitionStationeryName = rdl.Stationery.Description,
                            RequisitionStationeryItemPrice = st.Price,
                        };
                        chartDTOs.Add(chartDTO);
                    }
                }
            }
            return chartDTOs;
        }
    }
}