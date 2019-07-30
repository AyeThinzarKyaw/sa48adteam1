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
        private StationeryService() { }

        private static StationeryService instance = new StationeryService();
        public static IStationeryService Instance
        {
            get { return instance; }
        }

        public IEnumerable<Stationery> GetStationeriesBySupplierIdAndYear(int supplierId, int year)
        {
            return StationeryRepo.Instance.GetStationeriesBySupplierIdAndYear(supplierId, year);
        }

        public IEnumerable<Stationery> GetAllStationeries()
        {
            return StationeryRepo.Instance.FindAll();
        }

        public Stationery GetStationeryById(int id)
        {
            return StationeryRepo.Instance.FindById(id);
        }

        public void CreateStationery(Stationery stationery)
        {
            StationeryRepo.Instance.Create(stationery);
        }

        public void UpdateStationery(Stationery stationery)
        {
            StationeryRepo.Instance.Update(stationery);
        }
    }
}