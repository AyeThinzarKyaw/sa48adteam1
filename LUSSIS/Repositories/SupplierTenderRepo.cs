using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using LUSSIS.Repositories;
using LUSSIS.Repositories.Interfaces;
using LUSSIS.Models;

namespace LUSSIS.Repositories
{
    public class SupplierTenderRepo : GenericRepo<SupplierTender, int>, ISupplierTenderRepo
    {
        public IEnumerable<SupplierTender> GetSupplierTendersOfCurrentYearByStationeryId(int stationeryId)
        {
            return Context.SupplierTenders.Where(s => s.StationeryId == stationeryId && s.Year == DateTime.Now.Year).ToList();
        }

        public IEnumerable<SupplierTender> GetAllSupplierTendersOfCurrentYear()
        {
            return Context.SupplierTenders.Where(x => x.Year == DateTime.Now.Year);
        }
    }
}