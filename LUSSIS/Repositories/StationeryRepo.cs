using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using LUSSIS.Models;
using LUSSIS.Repositories.Interfaces;

namespace LUSSIS.Repositories
{
    public class StationeryRepo : GenericRepo<Stationery, int>, IStationeryRepo
    {

        IEnumerable<Stationery> IStationeryRepo.GetStationeriesBySupplierAndYear(Supplier supplier, int year)
        {
            return Context.Stationeries.Where(s => s.Id == s.SupplierTenders.Single(t => t.SupplierId == supplier.Id && t.Year == year).StationeryId).Distinct().ToList();
        }


    }
}