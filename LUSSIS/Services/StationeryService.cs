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
            Stationery stationery=StationeryRepo.Instance.FindById(id);
            stationery.SupplierTenders = stationery.SupplierTenders.OrderBy(x => x.Rank).ToList();
            return stationery;
        }

        public IEnumerable<Stationery> GetStationeriesByCategory(int id)
        {
            return StationeryRepo.Instance.getStationeriesByCategoryId(id);
        }

        public void CreateStationery(Stationery stationery)
        {
            StationeryRepo.Instance.Create(stationery);
        }

        public void UpdateStationery(Stationery stationery)
        {
            StationeryRepo.Instance.Update(stationery);
        }
        public IEnumerable<Category> GetAllCategories()
        {
            return CategoryRepo.Instance.FindAll().OrderBy(c=>c.Type).ToList();
        }

        public IEnumerable<Supplier> GetAllSuppliers()
        {
            return SupplierRepo.Instance.FindAll().OrderBy(s => s.Name).ToList();
        }
        public void CreateCategory(Category category)
        {
            CategoryRepo.Instance.Create(category);
        }

        public void UpdateCategory(Category category)
        {
            CategoryRepo.Instance.Update(category);
        }
    }
}