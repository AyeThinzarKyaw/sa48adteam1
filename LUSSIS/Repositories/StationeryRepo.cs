using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using LUSSIS.Models;

namespace LUSSIS.Repositories
{
    public class StationeryRepo
    {
        private LUSSISContext db = new LUSSISContext();

        private StationeryRepo() { }

        private static StationeryRepo instance = new StationeryRepo();
        public static StationeryRepo Instance
        {
            get { return instance; }
        }

        public IEnumerable<Stationery> getStationairesBySupplierAndYear(int supplierId, int year)
        {
            return db.Stationeries.Where(s => s.Id == s.SupplierTenders.Single(t => t.SupplierId == supplierId && t.Year == year).StationeryId).Distinct().ToList();

        }
    }
}