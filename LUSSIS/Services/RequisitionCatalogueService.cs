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
                int currBalance =  getCurrentBalance(s);
                int? lowStockCount = null;
                StockAvailabilityEnum stockAvailEnum = getStockAvailabilityStatus(currBalance, s.ReorderLevel);
                if(stockAvailEnum == StockAvailabilityEnum.LowStock)
                {
                    lowStockCount = currBalance;
                }
 
                catalogueItems.Add(new CatalogueItemDTO { Item = s.Description,
                    UnitOfMeasure = s.UnitOfMeasure, StockAvailability =  stockAvailEnum,
                    LowStockAvailability = lowStockCount, StationeryId = s.Id});
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
            int newStockBalance = getCurrentBalance(s);
            int? lowStockCount = null;
            StockAvailabilityEnum stockAvailEnum = getStockAvailabilityStatus(currBalance, s.ReorderLevel);
            if (stockAvailEnum == StockAvailabilityEnum.LowStock)
            {
                lowStockCount = currBalance;
            }

            return new CatalogueItemDTO { StockAvailability = stockAvailEnum, LowStockAvailability = lowStockCount, ReservedCount = reserved, WaitlistCount = waitlist };

        }

        public void RemoveCartDetail(int employeeId, int stationeryId)
        {
            throw new NotImplementedException();
        }

        public CatalogueItemDTO UpdateCartDetail(int employeeId, int stationeryId, int inputQty)
        {
            throw new NotImplementedException();
        }
    }
}