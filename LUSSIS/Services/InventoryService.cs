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
    public class InventoryService : IInventoryService
    {
        private IStationeryRepo stationeryRepo;
        private ICategoryRepo categoryRepo;
        private ISupplierTenderRepo supplierTenderRepo;
        private ISupplierRepo supplierRepo;
        private IAdjustmentVoucherRepo adjustmentVoucherRepo;
        private IAdjustmentVoucherDetailRepo adjustmentVoucherDetailRepo;
        private IRequisitionRepo requisitionRepo;
        private IRequisitionDetailRepo requisitionDetailRepo;
        private IDisbursementRepo disbursementRepo;
        private IEmployeeRepo employeeRepo;
        private IDepartmentRepo departmentRepo;
        private IPurchaseOrderRepo purchaseOrderRepo;
        private IPurchaseOrderDetailRepo purchaseOrderDetailRepo;
        private static InventoryService instance = new InventoryService();

        private InventoryService()
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

        //returns single instance
        public static IInventoryService Instance
        {
            get { return instance; }
        }

        public List<InventoryListDTO> RetrieveStationeryAndCategory()
        {
            List<Stationery> stationeries = stationeryRepo.getAllStationeries();
            List<InventoryListDTO> inventory = new List<InventoryListDTO>();

            foreach (Stationery st in stationeries)
            {
                Stationery s = StationeryRepo.Instance.FindById(st.Id);
                InventoryListDTO inventoryList = new InventoryListDTO();
                {
                    inventoryList.StationeryId = s.Id;
                    inventoryList.ItemNumber = s.Code;
                    inventoryList.Category = s.Category.Type;
                    inventoryList.Description = s.Description;
                    inventoryList.Location = s.Bin;
                    inventoryList.UnitOfMeasure = s.UnitOfMeasure;
                    inventoryList.QuantityInStock = s.Quantity;
                }
                inventory.Add(inventoryList);
            }

            return inventory;
        }

        public StockAndSupplierDTO RetrieveStockMovement(int stationeryId)
        {
            List<StockMovementDTO> stockMovement = new List<StockMovementDTO>();
            List<StockMovementBalanceDTO> stockMovementBalance = new List<StockMovementBalanceDTO>();
            List<SupplierStockRankDTO> supplierStockRank = new List<SupplierStockRankDTO>();

            Stationery s = stationeryRepo.FindById(stationeryId);
            Category c = categoryRepo.FindById(s.CategoryId);
            List<SupplierTender> st = (List<SupplierTender>)supplierTenderRepo.FindBy(x => x.StationeryId == stationeryId);

            List<Supplier> sp = new List<Supplier>();
            foreach (SupplierTender rankedsupplier in st)
            {
                Supplier supplier = (Supplier)supplierRepo.FindById(rankedsupplier.SupplierId);
                sp.Add(supplier);
            }

            //add Suppliers To SupplierStockRankDTO
            int limit = 3;
            for (int i = 1; i <= limit; i++)
            {
                SupplierStockRankDTO supstockrank = new SupplierStockRankDTO();
                {
                    supstockrank.Rank = i;

                    SupplierTender rankingsupplier = (SupplierTender)supplierTenderRepo.FindOneBy(x => x.StationeryId == stationeryId && x.Rank == i);
                    supstockrank.SupplierCode = rankingsupplier.Supplier.Code;
                    supstockrank.SupplierName = rankingsupplier.Supplier.Name;
                    supstockrank.ContactPerson = rankingsupplier.Supplier.ContactName;
                    supstockrank.ContactNumber = rankingsupplier.Supplier.PhoneNo;
                    supstockrank.Price = rankingsupplier.Price;
                }
                supplierStockRank.Add(supstockrank);
            }


            //retrieve all adjustment voucher Ids that are acknowledged
            List<int> avId = adjustmentVoucherRepo.getAdjustmentVoucherIdsWithAcknowledgedStatus();

            //retrieve all adjustment voucher details with adjustment voucher Ids that are acknowledged and stationeryId
            List<AdjustmentVoucherDetail> avDet = new List<AdjustmentVoucherDetail>();
            foreach (int adjvouch in avId)
            {
                List<AdjustmentVoucherDetail> adjvouchDetail = (List<AdjustmentVoucherDetail>)adjustmentVoucherDetailRepo.FindBy(x => x.AdjustmentVoucherId == adjvouch && x.StationeryId == stationeryId);
                foreach (AdjustmentVoucherDetail aVD in adjvouchDetail)
                {
                    avDet.Add(aVD);
                }

            }

            // set retrieved adjustmentvouchers into StockMovementDTO
            foreach (AdjustmentVoucherDetail adjV in avDet)
            {
                StockMovementDTO stockMovList = new StockMovementDTO();
                {
                    stockMovList.MovementDate = adjV.DateTime;
                    stockMovList.DepartmentOrSupplier = "Adjustment Voucher - " + adjV.AdjustmentVoucherId;
                    stockMovList.Quantity = adjV.Quantity;
                }
                stockMovement.Add(stockMovList);
            }

            //retrieve all purchase Order Ids that are closed
            List<int> poId = purchaseOrderRepo.getPurchaseOrderIdsWithClosedStatus();

            //retrieve all PO details with PO Ids that are closed and stationeryId
            List<PurchaseOrderDetail> purchaseOrderDet = new List<PurchaseOrderDetail>();
            foreach (int a in poId)
            {
                PurchaseOrderDetail purOrderDetail = (PurchaseOrderDetail)purchaseOrderDetailRepo.FindOneBy(x => x.PurchaseOrderId == a && x.StationeryId == stationeryId);
                purchaseOrderDet.Add(purOrderDetail);
            }

            // set retrieved PODetails into StockMovementDTO
            foreach (PurchaseOrderDetail poDetail in purchaseOrderDet)
            {
                if (poDetail != null)
                {
                    StockMovementDTO stockMovList = new StockMovementDTO();
                    {
                        stockMovList.MovementDate = (DateTime) purchaseOrderRepo.FindById(poDetail.PurchaseOrderId).DeliveryDateTime;
                        stockMovList.DepartmentOrSupplier = "Supplier - " + supplierRepo.FindById(purchaseOrderRepo.FindById(poDetail.PurchaseOrderId).SupplierId).Name;
                        stockMovList.Quantity = (int)poDetail.QuantityDelivered;
                    }
                    stockMovement.Add(stockMovList);
                }
            }

            //retrieve all requisitiondetails that are delivered and are of the input stationeryId
            List<RequisitionDetail> reqDet = (List<RequisitionDetail>)requisitionDetailRepo.FindBy(x => x.Status == "Collected" && x.StationeryId == stationeryId);

            // set retrieved PODetails into StockMovementDTO
            foreach (RequisitionDetail reqDetails in reqDet)
            {
                StockMovementDTO stockMovList = new StockMovementDTO();

                stockMovList.MovementDate = (DateTime)reqDetails.Disbursement.DeliveryDateTime;

                int rcdEmployeeId = (int)disbursementRepo.FindOneBy(x => x.Id == reqDetails.DisbursementId).ReceivedEmployeeId;

                stockMovList.DepartmentOrSupplier = employeeRepo.FindById(rcdEmployeeId).Department.DepartmentName; ;

                stockMovList.Quantity = (int)reqDetails.QuantityDelivered*-1;

                stockMovement.Add(stockMovList);
            }

            // order the list by date & alphabetically
            stockMovement = stockMovement.OrderBy(x => x.MovementDate).ToList();

            int runningBal = 0;

            // set StockMovementDTO into StockMovementBalanceDTO
            foreach (StockMovementDTO stkMovDTO in stockMovement)
            {

                StockMovementBalanceDTO stockMovBalList = new StockMovementBalanceDTO();

                stockMovBalList.StockMovement = stkMovDTO;
                runningBal = runningBal + stkMovDTO.Quantity;
                stockMovBalList.Balance = runningBal;

                stockMovementBalance.Add(stockMovBalList);
            }

            stockMovementBalance.Reverse();

            // set StockMovementBalanceDTO into StockAndSupplierDTO

            StockAndSupplierDTO stockAndSuppliers = new StockAndSupplierDTO();

            stockAndSuppliers.StationeryId = s.Id;
            stockAndSuppliers.ItemNumber = s.Code;
            stockAndSuppliers.Category = (String)categoryRepo.getCategoryType(s.CategoryId);
            stockAndSuppliers.Description = s.Description;
            stockAndSuppliers.Location = s.Bin;
            stockAndSuppliers.UnitOfMeasure = s.UnitOfMeasure;

            stockAndSuppliers.SupplierStockRank = supplierStockRank;

            foreach (StockMovementBalanceDTO stockMovementBalanceDTO in stockMovementBalance)
            {
                stockMovementBalanceDTO.Balance += s.Quantity - stockMovementBalance.Last().Balance;
            }

            stockAndSuppliers.StockMovementBalance = stockMovementBalance;

            return stockAndSuppliers;
        }

    }
}