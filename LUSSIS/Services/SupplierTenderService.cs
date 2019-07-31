using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using LUSSIS.Repositories;
using LUSSIS.Repositories.Interfaces;
using LUSSIS.Services.Interfaces;
using LUSSIS.Models;

namespace LUSSIS.Services
{
    public class SupplierTenderService : ISupplierTenderService
    {
        private SupplierTenderService() { }

        private static SupplierTenderService instance = new SupplierTenderService();
        public static ISupplierTenderService Instance
        {
            get { return instance; }
        }

        public IEnumerable<SupplierTender> GetSupplierTendersOfCurrentYearByStationeryId(int stationeryId)
        {
            //return supplierTenderRepo.FindBy(x => x.Year == DateTime.Now.Year && x.StationeryId == stationeryId);

            return SupplierTenderRepo.Instance.GetSupplierTendersOfCurrentYearByStationeryId(stationeryId);
        }

        public IEnumerable<SupplierTender> GetAllSupplierTendersOfCurrentYear()
        {
            return SupplierTenderRepo.Instance.GetAllSupplierTendersOfCurrentYear();
        }

        public void CreateSupplierTender(SupplierTender supplierTender)
        {
            SupplierTenderRepo.Instance.Create(supplierTender);
        }
        public void UpdateSupplierTender(SupplierTender supplierTender)
        {
            SupplierTenderRepo.Instance.Update(supplierTender);
        }

    }
}