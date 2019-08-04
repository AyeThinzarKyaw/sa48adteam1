using LUSSIS.Enums;
using LUSSIS.Models;
using LUSSIS.Models.DTOs;
using LUSSIS.Repositories;
using LUSSIS.Repositories.Interfaces;
using LUSSIS.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

// written by Edwin
namespace LUSSIS.Services
{
    public class RetrievalService : IRetrievalService
    {
        private IStationeryRepo stationeryRepo;
        private ICategoryRepo categoryRepo;
        private IRequisitionRepo requisitionRepo;
        private IRequisitionDetailRepo requisitionDetailRepo;
        private IDisbursementRepo disbursementRepo;
        private IEmployeeRepo employeeRepo;
        private IDepartmentRepo departmentRepo;
        private IPurchaseOrderRepo purchaseOrderRepo;
        private IPurchaseOrderDetailRepo purchaseOrderDetailRepo;
        private ICollectionPointRepo collectionPointRepo;
        private static RetrievalService instance = new RetrievalService();

        private RetrievalService()
        {
            stationeryRepo = StationeryRepo.Instance;
            categoryRepo = CategoryRepo.Instance;
            requisitionRepo = RequisitionRepo.Instance;
            requisitionDetailRepo = RequisitionDetailRepo.Instance;
            disbursementRepo = DisbursementRepo.Instance;
            employeeRepo = EmployeeRepo.Instance;
            departmentRepo = DepartmentRepo.Instance;
            purchaseOrderRepo = PurchaseOrderRepo.Instance;
            collectionPointRepo = CollectionPointRepo.Instance;
            purchaseOrderDetailRepo = PurchaseOrderDetailRepo.Instance;
        }

        //returns single instance
        public static IRetrievalService Instance
        {
            get { return instance; }
        }

        // method to get Collection Point Ids that Clerk is in charge of
        public List<CollectionPoint> RetrieveAssignedCollectionPoints(int employeeId)
        {
            List<CollectionPoint> assignedCollectionPointList = (List<CollectionPoint>) collectionPointRepo.FindBy(x => x.EmployeeId == employeeId);

            return assignedCollectionPointList;
        }

        // method to get departments in collection point assigned
        public List<Department> RetrieveDepartmentsInCollectionPointList(List<CollectionPoint> collectionPointList)
        {
            List<Department> assignedDepartmentList = new List<Department>();

            foreach (CollectionPoint cp in collectionPointList)
            {
                List<Department> deptList = (List<Department>)departmentRepo.FindBy(x => x.CollectionPointId == cp.Id);
                foreach (Department dp in deptList)
                {
                    Department dept = dp;
                    assignedDepartmentList.Add(dept);
                }
            }

            return assignedDepartmentList;
        }

        // method to get all employees in Departments
        public List<Employee> RetrieveAllEmployeesInAssignedDepartmentList(List<Department> departmentList)
        {
            List<Employee> employeesInAssignedDepartmentList = new List<Employee>();

            foreach (Department dp in departmentList)
            {
                List<Employee> employeeList = (List<Employee>)employeeRepo.FindBy(x => x.DepartmentId == dp.Id);
                foreach (Employee empl in employeeList)
                {
                    Employee em = empl;
                    employeesInAssignedDepartmentList.Add(em);
                }
            }

            return employeesInAssignedDepartmentList;
        }

        // method to get all Status == Approved Requisitions from employees 
        public List<Requisition> RetrieveAllApprovedRequisitionsByEmployeeList(List<Employee> employeeList)
        {
            List<Requisition> approvedRequisitionList = new List<Requisition>();

            foreach (Employee em in employeeList)
            {
                List<Requisition> requisitionList = (List<Requisition>)requisitionRepo.FindBy(x => x.EmployeeId == em.Id && x.Status == "Approved");
                foreach (Requisition req in requisitionList)
                {
                    Requisition rq = req;
                    approvedRequisitionList.Add(rq);
                }
            }

            return approvedRequisitionList;
        }

        // method to get all Status == Preparing Requisition Details from Status == Approved Requisitions 
        public List<RequisitionDetail> RetrieveAllPreparingRequisitionDetailsByRequisitionList(List<Requisition> requisitionList)
        {
            List<RequisitionDetail> preparingRequisitionDetailList = new List<RequisitionDetail>();

            foreach (Requisition req in requisitionList)
            {
                List<RequisitionDetail> requisitionDetailList = (List<RequisitionDetail>)requisitionDetailRepo.FindBy(x => x.RequisitionId == req.Id && x.Status == "Preparing");
                foreach (RequisitionDetail reqDet in requisitionDetailList)
                {
                    RequisitionDetail rqDet = reqDet;
                    preparingRequisitionDetailList.Add(rqDet);
                }
            }

            return preparingRequisitionDetailList;
        }

        // method to get all stationery Details from Status == Preparing Requisition Details
        public List<Stationery> RetrieveStationeryDetailsByRequisitionDetailsList(List<RequisitionDetail> requisitionDetailList)
        {
            List<Stationery> stationeryDetailList = new List<Stationery>();

            foreach (RequisitionDetail reqDet in requisitionDetailList)
            {
                Stationery st = (Stationery) stationeryRepo.FindById(reqDet.StationeryId);
                stationeryDetailList.Add(st);

            }

            return stationeryDetailList.GroupBy(x => x.Id).Select(g => g.First()).ToList();
        }

        public RetrievalDTO constructRetrievalDTO(LoginDTO loginDTO)
        {
            List<RetrievalPrepItemDTO> retrievalPrepList = new List<RetrievalPrepItemDTO>();
            List<RetrievalItemDTO> retrievalList = new List<RetrievalItemDTO>();

            List<CollectionPoint> assignedCollectionPoint = RetrieveAssignedCollectionPoints(loginDTO.EmployeeId);
            List<Department> assignedDepartment = RetrieveDepartmentsInCollectionPointList(assignedCollectionPoint);
            List<Employee> employeesInAssignedDepartments = RetrieveAllEmployeesInAssignedDepartmentList(assignedDepartment);
            List<Requisition> approvedRequisitionsFromEmployeesInAssignedDepartments = RetrieveAllApprovedRequisitionsByEmployeeList(employeesInAssignedDepartments);
            List<RequisitionDetail> preparingRequisitionDetailsFromApprovedRequisitions = RetrieveAllPreparingRequisitionDetailsByRequisitionList(approvedRequisitionsFromEmployeesInAssignedDepartments);
            List<Stationery> stationeriesInPreparingRequisitionDetails = RetrieveStationeryDetailsByRequisitionDetailsList(preparingRequisitionDetailsFromApprovedRequisitions);

            // .GroupBy(x => x.Id).Select(g => g.First()).ToList()
            // create retrievalItemDTO
            foreach (Stationery s in stationeriesInPreparingRequisitionDetails)
            {
                RetrievalItemDTO rID = new RetrievalItemDTO();

                rID.StationeryId = s.Id;
                rID.Description = s.Description;
                rID.Location = s.Bin;
                rID.RetrievalPrepItemList = new List<RetrievalPrepItemDTO>();

                retrievalList.Add(rID);
            }

            // add all preparing transactions to the retrievalDTO (groupedby stationeryid)
            foreach (RetrievalItemDTO rID in retrievalList)
            {
                List<RequisitionDetail> reqDetailList = (List<RequisitionDetail>)preparingRequisitionDetailsFromApprovedRequisitions.FindAll(x => x.StationeryId == rID.StationeryId);
                foreach (RequisitionDetail reDList in reqDetailList)
                {
                    RetrievalPrepItemDTO rPID = new RetrievalPrepItemDTO();
                    rPID.ReqStationery = stationeriesInPreparingRequisitionDetails.Find(x => x.Id == reDList.StationeryId);
                    rPID.ReqDetail = preparingRequisitionDetailsFromApprovedRequisitions.Find(x => x.StationeryId == reDList.StationeryId);
                    rPID.Req = approvedRequisitionsFromEmployeesInAssignedDepartments.Find(x => x.Id == rPID.ReqDetail.RequisitionId);
                    rPID.ReqOwner = employeesInAssignedDepartments.Find(x => x.Id == rPID.Req.EmployeeId);
                    rPID.ReqDepartmentRep = employeesInAssignedDepartments.Find(x => x.RoleId == 3);
                    rPID.ReqDepartment = assignedDepartment.Find(x => x.Id == rPID.ReqDepartmentRep.DepartmentId);
                    rPID.ReqCollectionPoint = assignedCollectionPoint.Find(x => x.Id == rPID.ReqDepartment.CollectionPointId);

                    rID.RetrievalPrepItemList.Add(rPID);
                }
            }

            //complete RetrievalItemDTO
            foreach (RetrievalItemDTO rID in retrievalList)
            {

                foreach (RetrievalPrepItemDTO rPID in rID.RetrievalPrepItemList)
                {
                    int x = rPID.ReqDetail.QuantityOrdered;
                    rID.NeededQuantity = rID.NeededQuantity + x;
                }

                rID.RetrievedQty = rID.NeededQuantity;
            }

            RetrievalDTO retrieval = new RetrievalDTO();

            retrieval.RetrievalDate = System.DateTime.Now.ToString("MM/dd/yyyy");
            Employee clerk = (Employee) employeeRepo.FindOneBy(x => x.Id == loginDTO.EmployeeId);

            if (clerk.Name == null)
            {
                retrieval.GeneratedBy = null;
            }
            else
            {
                retrieval.GeneratedBy = clerk.Name;
            }

            retrieval.RetrievalItem = retrievalList;

            return retrieval;
        }


            //public List<InventoryListDTO> RetrieveStationeryAndCategory()
            //{
            //    List<Stationery> stationeries = stationeryRepo.getAllStationeries();
            //    List<InventoryListDTO> inventory = new List<InventoryListDTO>();

            //    foreach (Stationery s in stationeries)
            //    {
            //        InventoryListDTO inventoryList = new InventoryListDTO();
            //        {
            //            inventoryList.StationeryId = s.Id;
            //            inventoryList.ItemNumber = s.Code;
            //            inventoryList.Category = (String) categoryRepo.getCategoryType(s.CategoryId);
            //            inventoryList.Description = s.Description;
            //            inventoryList.Location = s.Bin;
            //            inventoryList.UnitOfMeasure = s.UnitOfMeasure;
            //            inventoryList.QuantityInStock = s.Quantity;
            //        }
            //        inventory.Add(inventoryList);
            //    }

            //    return inventory;
            //}

            //public StockAndSupplierDTO RetrieveStockMovement(int stationeryId)
            //{
            //    List<StockMovementDTO> stockMovement = new List<StockMovementDTO>();
            //    List<StockMovementBalanceDTO> stockMovementBalance = new List<StockMovementBalanceDTO>();
            //    List<SupplierStockRankDTO> supplierStockRank = new List<SupplierStockRankDTO>();

            //    Stationery s = stationeryRepo.FindById(stationeryId);
            //    Category c = categoryRepo.FindById(s.CategoryId);
            //    List<SupplierTender> st = (List<SupplierTender>) supplierTenderRepo.FindBy(x => x.StationeryId == stationeryId);

            //    List<Supplier> sp = new List<Supplier>();
            //    foreach (SupplierTender rankedsupplier in st)
            //    {
            //        Supplier supplier = (Supplier) supplierRepo.FindById(rankedsupplier.SupplierId);
            //        sp.Add(supplier);
            //    }

            //    //add Suppliers To SupplierStockRankDTO
            //    int limit = 3;
            //    for (int i = 1; i <= limit; i++)
            //    {
            //        SupplierStockRankDTO supstockrank = new SupplierStockRankDTO();
            //        {
            //            supstockrank.Rank = i;

            //            SupplierTender rankingsupplier = (SupplierTender) supplierTenderRepo.FindOneBy(x => x.StationeryId == stationeryId && x.Rank == i);
            //            supstockrank.SupplierCode = rankingsupplier.Supplier.Code;
            //            supstockrank.SupplierName = rankingsupplier.Supplier.Name;
            //            supstockrank.ContactPerson = rankingsupplier.Supplier.ContactName;
            //            supstockrank.ContactNumber = rankingsupplier.Supplier.PhoneNo;
            //            supstockrank.Price = rankingsupplier.Price;
            //          }
            //        supplierStockRank.Add(supstockrank);
            //    }


            //    //retrieve all adjustment voucher Ids that are acknowledged
            //    List<int> avId = adjustmentVoucherRepo.getAdjustmentVoucherIdsWithAcknowledgedStatus();

            //    //retrieve all adjustment voucher details with adjustment voucher Ids that are acknowledged and stationeryId
            //    List<AdjustmentVoucherDetail> avDet = new List<AdjustmentVoucherDetail>();
            //    foreach (int adjvouch in avId)
            //    {
            //        List<AdjustmentVoucherDetail> adjvouchDetail = (List<AdjustmentVoucherDetail>)adjustmentVoucherDetailRepo.FindBy(x => x.AdjustmentVoucherId == adjvouch && x.StationeryId == stationeryId);
            //        foreach (AdjustmentVoucherDetail aVD in adjvouchDetail)
            //        {
            //            avDet.Add(aVD);
            //        }

            //    }

            //    // set retrieved adjustmentvouchers into StockMovementDTO
            //    foreach (AdjustmentVoucherDetail adjV in avDet)
            //    {
            //        StockMovementDTO stockMovList = new StockMovementDTO();
            //        {
            //            stockMovList.MovementDate = adjV.DateTime;
            //            stockMovList.DepartmentOrSupplier = "Adjustment Voucher - " + adjV.AdjustmentVoucherId;
            //            stockMovList.Quantity = adjV.Quantity;
            //        }
            //        stockMovement.Add(stockMovList);
            //    }

            //    //retrieve all purchase Order Ids that are closed
            //    List<int> poId = purchaseOrderRepo.getPurchaseOrderIdsWithClosedStatus();

            //    //retrieve all PO details with PO Ids that are closed and stationeryId
            //    List<PurchaseOrderDetail> purchaseOrderDet = new List<PurchaseOrderDetail>();
            //    foreach (int a in poId)
            //    {
            //        PurchaseOrderDetail purOrderDetail = (PurchaseOrderDetail) purchaseOrderDetailRepo.FindOneBy(x => x.PurchaseOrderId == a && x.StationeryId == stationeryId);
            //        purchaseOrderDet.Add(purOrderDetail);
            //    }

            //    // set retrieved PODetails into StockMovementDTO
            //    foreach (PurchaseOrderDetail poDetail in purchaseOrderDet)
            //    {
            //        StockMovementDTO stockMovList = new StockMovementDTO();
            //        {
            //            stockMovList.MovementDate = purchaseOrderRepo.FindById(poDetail.PurchaseOrderId).OrderDateTime;
            //            stockMovList.DepartmentOrSupplier = "Supplier - " + supplierRepo.FindById(purchaseOrderRepo.FindById(poDetail.PurchaseOrderId).SupplierId).Name;
            //            stockMovList.Quantity = (int) poDetail.QuantityDelivered;
            //        }
            //        stockMovement.Add(stockMovList);
            //    }

            //    //retrieve all requisitiondetails that are delivered and are of the input stationeryId
            //    List<RequisitionDetail> reqDet = (List<RequisitionDetail>) requisitionDetailRepo.FindBy(x => x.Status == "Collected" && x.StationeryId == stationeryId);

            //    // set retrieved PODetails into StockMovementDTO
            //    foreach (RequisitionDetail reqDetails in reqDet)
            //    {
            //        StockMovementDTO stockMovList = new StockMovementDTO();
            //        {
            //            stockMovList.MovementDate = (DateTime) reqDetails.Disbursement.DeliveryDateTime;

            //            int rcdEmployeeId = (int) disbursementRepo.FindOneBy(x => x.Id == reqDetails.DisbursementId).ReceivedEmployeeId;
            //            string deptName = departmentRepo.FindOneBy(x => x.Id == employeeRepo.FindOneBy(y => y.Id == rcdEmployeeId).DepartmentId).DepartmentName;
            //            stockMovList.DepartmentOrSupplier = deptName;

            //            stockMovList.Quantity = (int) reqDetails.QuantityDelivered;
            //        }
            //        stockMovement.Add(stockMovList);
            //    }

            //    // order the list by date & alphabetically
            //    stockMovement.OrderBy(x => x.MovementDate).OrderBy(x => x.DepartmentOrSupplier);

            //    // set StockMovementDTO into StockMovementBalanceDTO
            //    foreach (StockMovementDTO stkMovDTO in stockMovement)
            //    {
            //        int runningBal = 0;
            //        StockMovementBalanceDTO stockMovBalList = new StockMovementBalanceDTO();
            //        {
            //            stockMovBalList.StockMovement = stkMovDTO;
            //            runningBal = 0 + stkMovDTO.Quantity;
            //            stockMovBalList.Balance = runningBal;
            //        }
            //        stockMovementBalance.Add(stockMovBalList);
            //    }

            //    // set StockMovementBalanceDTO into StockAndSupplierDTO

            //    StockAndSupplierDTO stockAndSuppliers = new StockAndSupplierDTO();

            //    stockAndSuppliers.StationeryId = s.Id;
            //    stockAndSuppliers.ItemNumber = s.Code;
            //    stockAndSuppliers.Category = (String)categoryRepo.getCategoryType(s.CategoryId);
            //    stockAndSuppliers.Description = s.Description;
            //    stockAndSuppliers.Location = s.Bin;
            //    stockAndSuppliers.UnitOfMeasure = s.UnitOfMeasure;

            //    stockAndSuppliers.SupplierStockRank = supplierStockRank;

            //    stockAndSuppliers.StockMovementBalance = stockMovementBalance;

            //    return stockAndSuppliers;
            //}

        }
}