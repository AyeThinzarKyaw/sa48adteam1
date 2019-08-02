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

namespace LUSSIS.Services
{
    public class RequisitionCatalogueService : IRequisitionCatalogueService
    {
        private ICartDetailRepo cartDetailRepo;
        private IStationeryRepo stationeryRepo;
        private IRequisitionRepo requisitionRepo;
        private IRequisitionDetailRepo requisitionDetailRepo;
        private IAdjustmentVoucherRepo adjustmentVoucherRepo;
        private static RequisitionCatalogueService instance = new RequisitionCatalogueService();

        private RequisitionCatalogueService()
        {
            cartDetailRepo = CartDetailRepo.Instance;
            stationeryRepo = StationeryRepo.Instance;
            requisitionRepo = RequisitionRepo.Instance;
            requisitionDetailRepo = RequisitionDetailRepo.Instance;
            adjustmentVoucherRepo = AdjustmentVoucherRepo.Instance;
        }

        //returns single instance
        public static IRequisitionCatalogueService Instance
        {
            get { return instance; }
        }

        public List<CatalogueItemDTO> GetCatalogueItems(int employeeId)
        {
            List<CartDetail> cartDetails = GetAnyExistingCartDetails(employeeId);
            List<Stationery> stationeries = (List<Stationery>)stationeryRepo.FindAll();
            List<CatalogueItemDTO> catalogueItems = new List<CatalogueItemDTO>();

            //for each stationery in stationeries create a catalogueItemDTO
            foreach(Stationery s in stationeries)
            {
                CatalogueItemDTO catalogueItemDTO = new CatalogueItemDTO()
                {
                    Item = s.Description,
                    UnitOfMeasure = s.UnitOfMeasure,
                    StationeryId = s.Id
                };
                getCatalogueItemAvailability(catalogueItemDTO, s);               
                catalogueItems.Add(catalogueItemDTO);
            }

            if (cartDetails != null)
            {
                foreach(CartDetail cd in cartDetails)
                {
                    CatalogueItemDTO catItemDTO = catalogueItems.Find(x => x.StationeryId == cd.StationeryId);
                    catItemDTO.OrderQtyInput = cd.Quantity;
                    catItemDTO.ReservedCount = getReservedBalanceForExistingCartItem(cd);
                    catItemDTO.WaitlistCount = cd.Quantity - catItemDTO.ReservedCount;
                    catItemDTO.Confirmation = true;
                }
            }
            return catalogueItems;
        }

        private StockAvailabilityEnum getStockAvailabilityStatus(int currBalance, int? reorderLevel)
        {
            if (currBalance <= 0)
            {
                return StockAvailabilityEnum.OutOfStock;
            }
            else if (reorderLevel != null && currBalance < reorderLevel)
            {
                return StockAvailabilityEnum.LowStock;
                
            }
            else 
            {
                return StockAvailabilityEnum.InStock;
            }
        }

        private int getReservedBalanceForExistingCartItem(CartDetail cd)
        {
            //Q - requisition reserved - adjustment open - total in front of queue
            int reservedCount = requisitionDetailRepo.GetReservedCountForStationery(cd.StationeryId);
            int foqCartCount = cartDetailRepo.GetFrontOfQueueCartCountForStationery(cd.StationeryId, cd.DateTime);
            int openAdjustmentCount = adjustmentVoucherRepo.GetOpenAdjustmentVoucherCountForStationery(cd.StationeryId);
            int totalCount = stationeryRepo.FindById(cd.StationeryId).Quantity;
            int netCount = totalCount - reservedCount - foqCartCount - openAdjustmentCount;

            if (netCount <= 0)
            {
                return 0;
            }
            else if (netCount >= cd.Quantity)
            {
                return cd.Quantity;
            }
            else
            {
                return netCount;
            }
        }

        private int getCurrentBalance(Stationery s)
        {
            int reservedCount = requisitionDetailRepo.GetReservedCountForStationery(s.Id);
            int cartCount = cartDetailRepo.GetCountOnHoldForStationery(s.Id); //could be 0
            int openAdjustmentCount = adjustmentVoucherRepo.GetOpenAdjustmentVoucherCountForStationery(s.Id);
            int totalCount = stationeryRepo.FindById(s.Id).Quantity;
            int netCount = totalCount - reservedCount - cartCount - openAdjustmentCount;

            if(netCount <= 0)
            {
                return 0;
            }
            else
            {
                return netCount;
            }
          
        }


        //returns null if user's cart is empty
        private List<CartDetail> GetAnyExistingCartDetails(int employeeId)
        {
            
            List<CartDetail> cartDetails = (List<CartDetail>)cartDetailRepo.FindBy(x => x.Employee.Id == employeeId);
            if (cartDetails.Count == 0)
            {
                return null;
            }
            else
            {
                return cartDetails;
            }
            
        }     

        public CatalogueItemDTO AddCartDetail(int employeeId, int stationeryId, int inputQty)
        {
            Stationery s = stationeryRepo.FindById(stationeryId);
            int currBalance = getCurrentBalance(s);
            int reserved = 0;
            int waitlist = 0;
            if (inputQty > currBalance)
            {
                reserved = currBalance;
                waitlist = inputQty - currBalance;
            }
            else{
                reserved = inputQty;
            }
            CartDetail cd = new CartDetail { DateTime = DateTime.Now, EmployeeId = employeeId, Quantity = inputQty, StationeryId = stationeryId };
            cartDetailRepo.Create(cd); //persist new cart detail

            //can abstract into a method and share with above bbut must instantiate a new DTO first
            CatalogueItemDTO catalogueItemDTO = new CatalogueItemDTO() { ReservedCount = reserved, WaitlistCount = waitlist };
            getCatalogueItemAvailability(catalogueItemDTO, s);

            return catalogueItemDTO;

        }

        public CatalogueItemDTO RemoveCartDetail(int employeeId, int stationeryId)
        {
            Stationery s = stationeryRepo.FindById(stationeryId);
            //remove cd from db first
            CartDetail cd = cartDetailRepo.FindOneBy(x => x.EmployeeId == employeeId && x.StationeryId == stationeryId);
            if(cd != null)
            {
                cartDetailRepo.Delete(cd);
                CatalogueItemDTO catalogueItemDTO = new CatalogueItemDTO();
                getCatalogueItemAvailability(catalogueItemDTO, s);
                return catalogueItemDTO;
            }

            return null;
        }

        private void getCatalogueItemAvailability(CatalogueItemDTO catalogueItemDTO, Stationery s)
        {
            int currBalance = getCurrentBalance(s);
            int? lowStockCount = null;
            StockAvailabilityEnum stockAvailEnum = getStockAvailabilityStatus(currBalance, s.ReorderLevel);
            if (stockAvailEnum == StockAvailabilityEnum.LowStock)
            {
                lowStockCount = currBalance;
            }

            catalogueItemDTO.StockAvailability = stockAvailEnum;
            catalogueItemDTO.LowStockAvailability = lowStockCount;
        }

        public CatalogueItemDTO UpdateCartDetail(int employeeId, int stationeryId, int inputQty)
        {
            throw new NotImplementedException();
        }

        public List<Requisition> GetPersonalRequisitionHistory(int employeeId)
        {
            return (List<Requisition>)requisitionRepo.FindBy(x=> x.EmployeeId == employeeId);  
        }

        public List<RequisitionDetail> GetRequisitionDetailsForPersonalRequisition(int requisitionId, int employeeId)
        {
            return (List<RequisitionDetail>)requisitionDetailRepo.FindBy(x=> x.RequisitionId == requisitionId);
        }

        public Requisition ConvertCartDetailsToRequisitionDetails(int employeeId)
        {
            //erase records from cartDetails
            List<CartDetail> cartDetails = (List<CartDetail>)cartDetailRepo.FindBy(x => x.EmployeeId == employeeId);
            Requisition newRequisition = new Requisition() {DateTime = DateTime.Now, EmployeeId = employeeId,
                Status = RequisitionStatusEnum.PENDING.ToString() };
            newRequisition = requisitionRepo.Create(newRequisition);
            List<RequisitionDetail> requisitionDetails = new List<RequisitionDetail>();

            foreach(CartDetail cd in cartDetails)
            {
                cartDetailRepo.Delete(cd);

                int reservedCount = getReservedBalanceForExistingCartItem(cd);
                if(reservedCount < cd.Quantity)
                {
                    int waitllistCount = cd.Quantity - reservedCount;
                    //has waitlist, create 2 requisition details
                    RequisitionDetail waitlistRequisitionDetail = new RequisitionDetail()
                    {
                        QuantityOrdered = waitllistCount,
                        RequisitionId = newRequisition.Id,
                        Status = RequisitionDetailStatusEnum.WAITLIST_PENDING.ToString()
                    };
                    requisitionDetailRepo.Create(waitlistRequisitionDetail);
                    if(reservedCount > 0)
                    {
                        createReservedPendingRequisitionDetail(reservedCount, newRequisition.Id);
                    }                    
                }
                else
                {
                    createReservedPendingRequisitionDetail(cd.Quantity, newRequisition.Id);
                }

            }
            return newRequisition;
        }

        private void createReservedPendingRequisitionDetail(int reservedCount, int requisitionId)
        {
            RequisitionDetail reservedRequisitionDetail = new RequisitionDetail()
            {
                QuantityOrdered = reservedCount,
                RequisitionId = requisitionId,
                Status = RequisitionDetailStatusEnum.RESERVED_PENDING.ToString()
            };
            requisitionDetailRepo.Create(reservedRequisitionDetail);
        }

        RequisitionDetailsDTO IRequisitionCatalogueService.GetRequisitionDetailsForPersonalRequisition(int requisitionId, int employeeId)
        {
            throw new NotImplementedException();
        }
    }
}