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

        IEnumerable<Stationery> IStationeryRepo.GetStationeriesBySupplierIdAndYear(int supplierId, int year)
        {
            return Context.Stationeries.Where(s => s.Id == s.SupplierTenders.Single(t => t.SupplierId == supplierId && t.Year == year).StationeryId).Distinct().ToList();
        }


    }
}