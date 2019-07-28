using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using LUSSIS.Services.Interfaces;
using LUSSIS.Models;
using LUSSIS.Repositories;

namespace LUSSIS.Services
{
    public class StationeryService:IStationeryService
    {
        private StationeryService() { }

        private static StationeryService instance = new StationeryService();
        public static IStationeryService Instance
        {
            get { return instance; }
        }

        public IEnumerable<Stationery> getStationairesBySupplierAndYear(int supplierId, int year)
        {
            return StationeryRepo.Instance.getStationairesBySupplierAndYear(supplierId, year);
        }
    }
}