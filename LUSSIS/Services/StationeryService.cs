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


        public IEnumerable<Stationery> GetStationeriesBySupplierAndYear(int supplierId, int year)
        {
            return stationeryRepo.GetStationeriesBySupplierAndYear(supplierId, year);
        }

        public IEnumerable<Stationery> GetAllStationeries()
        {
            return stationeryRepo.FindAll();
        }

        public Stationery GetStationeryById(int id)
        {
            return stationeryRepo.FindById(id);
        }

        public void CreateStationery(Stationery stationery)
        {
            stationeryRepo.Create(stationery);
        }

        public void UpdateStationery(Stationery stationery)
        {
            stationeryRepo.Update(stationery);
        }
    }
}