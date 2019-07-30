using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using LUSSIS.Services.Interfaces;
using LUSSIS.Models;
using LUSSIS.Repositories;
using LUSSIS.Repositories.Interfaces;

namespace LUSSIS.Services
{
    public class StationeryService : IStationeryService
    {
        private IStationeryRepo stationeryRepo;

        public StationeryService(IStationeryRepo stationeryRepo)
        {
            this.stationeryRepo = stationeryRepo;
        }


        public IEnumerable<Stationery> GetStationeriesBySupplierIdAndYear(int supplierId, int year)
        {
            //return stationeryRepo.FindBy(x => x.CategoryId == 2 && x.Bin == "Hello");
            return stationeryRepo.GetStationeriesBySupplierAndYear(supplierId, year);
        }
    }
}