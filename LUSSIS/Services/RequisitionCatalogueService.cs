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
                getCurrentBalance(s);
                catalogueItems.Add(new CatalogueItemDTO { Item = s.Description, UnitOfMeasure = s.UnitOfMeasure,  })
            }

            if (cartDetails != null)
            {

            }

            throw new NotImplementedException();
        }

        private void getCurrentBalance(Stationery s)
        {
            int reservedCount = ;
            int cartCount = 0;


            throw new NotImplementedException();
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