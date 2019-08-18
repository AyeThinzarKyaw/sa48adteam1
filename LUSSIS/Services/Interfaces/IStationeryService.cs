using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using LUSSIS.Models;

namespace LUSSIS.Services.Interfaces
{
    public interface IStationeryService
    {
        IEnumerable<Stationery> GetStationeriesBySupplierIdAndYear(int supplierId, int year);

        IEnumerable<Stationery> GetAllStationeries();

        Stationery GetStationeryById(int id);

        void CreateStationery(Stationery stationery);

        void UpdateStationery(Stationery stationery);

        IEnumerable<Category> GetAllCategories();

        IEnumerable<Supplier> GetAllSuppliers();

        IEnumerable<Stationery> GetStationeriesByCategory(int id);

        void CreateCategory(Category category);

        void UpdateCategory(Category category);
    }
}