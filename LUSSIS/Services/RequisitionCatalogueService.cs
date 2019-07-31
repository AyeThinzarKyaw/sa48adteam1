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
                StockAvailabilityEnum stockAvailEnum;
                int? lowStockCount = null;

                if (currBalance <= 0)
                {
                    stockAvailEnum = StockAvailabilityEnum.OutOfStock;
                }
                else if (currBalance < s.ReorderLevel)
                {
                    stockAvailEnum = StockAvailabilityEnum.LowStock;
                    lowStockCount = currBalance;
                }
                else
                {
                    stockAvailEnum = StockAvailabilityEnum.InStock;
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
                    if (cd.Quantity <= getCurrentBalance(cd.Stationery))
                    {
                        catItemDTO.ReservedCount = cd.Quantity;
                    }
                    else
                    {
                        catItemDTO.ReservedCount = getCurrentBalance(cd.Stationery);
                        catItemDTO.WaitlistCount = cd.Quantity - getCurrentBalance(cd.Stationery);
                    }
                    catItemDTO.Confirmation = true;
                }
            }
            return catalogueItems;
        }

        private int getCurrentBalance(Stationery s)
        {
            int reservedCount = requisitionDetailRepo.GetReservedCountForStationery(s.Id);
            int cartCount = cartDetailRepo.GetCountOnHoldForStationery(s.Id); //could be 0
            int totalCount = stationeryRepo.FindById(s.Id).Quantity;

            return totalCount - reservedCount - cartCount;
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

        public CatalogueItemDTO AddCartDetail(CatalogueItemDTO catalogueItemDTO)
        {
            Stationery s = stationeryRepo.FindById(catalogueItemDTO.StationeryId);
            int? requestedBalance = catalogueItemDTO.OrderQtyInput;
            int currBalance = getCurrentBalance(s);


            throw new NotImplementedException();
        }

        public void RemoveCartDetail(CatalogueItemDTO catalogueItemDTO)
        {
            throw new NotImplementedException();
        }

        public CatalogueItemDTO UpdateCartDetail(CatalogueItemDTO catalogueItemDTO)
        {
            throw new NotImplementedException();
        }
    }
}