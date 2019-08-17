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
    public class PurchaseOrderService :IPurchaseOrderService
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
        public PurchaseOrderService() {
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

        private static PurchaseOrderService instance = new PurchaseOrderService();
        public static IPurchaseOrderService Instance
        {
            get { return instance; }
        }

        public IEnumerable<PurchaseOrder> getAllPurchaseOrders()
        {
            return PurchaseOrderRepo.Instance.FindAll().OrderBy(x=>x.OrderDateTime).ToList();
        }

        public PurchaseOrder getPurchaseOrderById(int poId)
        {
            return PurchaseOrderRepo.Instance.FindById(poId);
        }

        public IEnumerable<PO_getPOCatalogue_Result> RetrievePurchaseOrderCatalogue()
        {
            return PurchaseOrderRepo.Instance.GetPOCatalogue();
        }
        public void CreatePO(PurchaseOrder po)
        {            
                po.OrderDateTime = DateTime.Now;                
                PurchaseOrderRepo.Instance.Create(po);           
        }
        public void UpdatePO(PurchaseOrder po)
        {
            PurchaseOrderRepo.Instance.Update(po);
        }
        public void CreatePODetail(PurchaseOrderDetail pod)
        {
            PurchaseOrderDetailRepo.Instance.Create(pod);
        }

        public void UpdatePODetail(PurchaseOrderDetail pod)
        {
            PurchaseOrderDetailRepo.Instance.Update(pod);
        }

        public void RaisePO(POCreateDTO poCreateDTO,int createdBy)
        {
            foreach (Stationery item in poCreateDTO.SelectedItems.Where(s=>s.CategoryId!=0 && s.Status=="confirmed"))
            {
                if (poCreateDTO.Catalogue.Single(c => c.Id == item.Id).Unsubmitted > 0)
                {

                    PurchaseOrder po = new PurchaseOrder();

                    PurchaseOrderDetail pod = new PurchaseOrderDetail();
                    pod.StationeryId = item.Id;
                    pod.QuantityOrdered = poCreateDTO.Catalogue.Single(c => c.Id == item.Id).Unsubmitted;

                    if (poCreateDTO.ConfirmedPOs.Count == 0 || poCreateDTO.ConfirmedPOs.Where(x => x.SupplierId == item.CategoryId).Count() <= 0)
                    {
                        //if no Confirm PO for this supplier yet, create new PO
                        po.SupplierId = item.CategoryId;
                        po.Status = Enum.GetName(typeof(Enums.POStatus), Enums.POStatus.PENDING);
                        po.EstDeliveryDate = poCreateDTO.EstimatedDates.Single(e => e.Key == po.SupplierId).Value;

                        pod.PurchaseOrderId = po.Id;
                        po.PurchaseOrderDetails.Add(pod);
                        poCreateDTO.ConfirmedPOs.Add(po);
                    }
                    else
                    {
                        //po = poCreateDTO.ConfirmedPOs.Single(x => x.SupplierId == item.CategoryId);
                        //pod.PurchaseOrderId = po.Id;

                        poCreateDTO.ConfirmedPOs.Single(x => x.SupplierId == item.CategoryId).PurchaseOrderDetails.Add(pod);
                    }
                }                
            }
            Employee employee = EmployeeRepo.Instance.FindById(createdBy);
            foreach (PurchaseOrder purchaseOrder in poCreateDTO.ConfirmedPOs)
            {
                purchaseOrder.EmployeeId = createdBy;
                PurchaseOrderService.Instance.CreatePO(purchaseOrder);
                
                EmailNotificationService.Instance.SendNotificationEmail(receipient: "sa48team1@gmail.com", subject: "Request for purchase order approval", body: "Dear Manager,\n\nStore clerk - "+ employee.Name+" raised a purchase order. Please check and approve." , cc: "sa48team1@gmail.com");
            }
        }
        public List<ChartDTO> TrendChartInfo(int SupplierId, int CategoryId, int StationeryId)
        {
            List<PurchaseOrderDetail> PurchaseOrderDetailsList = purchaseOrderDetailRepo.GetPurchaseOrderDetailsBySupplierId(SupplierId);
            IEnumerable<SupplierTender> SupplierTendersList = supplierTenderRepo.FindBy(x => x.SupplierId == SupplierId);

            List<ChartDTO> chartDTOs = new List<ChartDTO>();

            foreach (PurchaseOrderDetail pod in PurchaseOrderDetailsList)
            {
                foreach (SupplierTender st in SupplierTendersList)
                {
                    if (pod.StationeryId == st.StationeryId && supplierRepo.FindById(purchaseOrderRepo.FindById(pod.PurchaseOrderId).SupplierId).Id == st.SupplierId && categoryRepo.FindById(stationeryRepo.FindById(pod.StationeryId).CategoryId).Id == CategoryId && stationeryRepo.FindById(pod.StationeryId).Id == StationeryId)
                    {

                        ChartDTO chartDTO = new ChartDTO()
                        {
                            PurchaseOrderDetailId = pod.Id,
                            PurchaseOrderId = pod.PurchaseOrderId,
                            StationeryId = pod.StationeryId,
                            QuantityOrdered = pod.QuantityOrdered,
                            QuantityDelivered = pod.QuantityDelivered==null?0:(int)pod.QuantityDelivered,
                            ItemName = stationeryRepo.FindById(pod.StationeryId).Description,
                            ItemType = categoryRepo.FindById(stationeryRepo.FindById(pod.StationeryId).CategoryId).Type,
                            UnitOfMeasure = stationeryRepo.FindById(pod.StationeryId).UnitOfMeasure,
                            OrderDateTime = purchaseOrderRepo.FindById(pod.PurchaseOrderId).OrderDateTime,
                            SupplierName = supplierRepo.FindById(purchaseOrderRepo.FindById(pod.PurchaseOrderId).SupplierId).Name,
                            ItemUnitPrice = st.Price,
                            EmployeeId = purchaseOrderRepo.FindById(pod.PurchaseOrderId).EmployeeId,
                            EmployeeName = employeeRepo.FindById(purchaseOrderRepo.FindById(pod.PurchaseOrderId).EmployeeId).Name,


                            PurchaseOrderDetailForChart = purchaseOrderDetailRepo.FindById(pod.Id),
                            EmployeeForChart = employeeRepo.FindById(purchaseOrderRepo.FindById(pod.PurchaseOrderId).EmployeeId),
                            SupplierForChart = supplierRepo.FindById(purchaseOrderRepo.FindById(pod.PurchaseOrderId).SupplierId),
                            StationeryForChart = stationeryRepo.FindById(pod.StationeryId),
                            PurchaseOrderForChart = purchaseOrderRepo.FindById(pod.PurchaseOrderId),
                            SupplierTenderForChart = supplierTenderRepo.FindById(st.Id),
                            CategoryForChart = categoryRepo.FindById(stationeryRepo.FindById(pod.StationeryId).CategoryId),
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

            ChartFilteringDTO FilteringDetails = new ChartFilteringDTO()
            {
                SupplierForChartList = suppliers,
                StationeryForChartList = stationeries,
                CategoryForChartList = categories
            };
            return FilteringDetails;
        }
    }
}