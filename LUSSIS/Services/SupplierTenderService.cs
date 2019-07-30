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
        private ISupplierTenderRepo supplierTenderRepo;

        public SupplierTenderService(ISupplierTenderRepo supplierTenderRepo)
        {
            this.supplierTenderRepo = supplierTenderRepo;
        }


        public IEnumerable<SupplierTender> GetSupplierTendersOfCurrentYearByStationeryId(int stationeryId)
        {
            //return supplierTenderRepo.FindBy(x => x.Year == DateTime.Now.Year && x.StationeryId == stationeryId);

            return supplierTenderRepo.GetSupplierTendersOfCurrentYearByStationeryId(stationeryId);
        }

        public IEnumerable<SupplierTender> GetAllSupplierTendersOfCurrentYear()
        {
            return supplierTenderRepo.GetAllSupplierTendersOfCurrentYear();
        }
    }
}