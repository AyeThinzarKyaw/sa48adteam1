using LUSSIS.Models;
using LUSSIS.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace LUSSIS.Repositories
{
    public class RequisitionDetailRepo : GenericRepo<RequisitionDetail, int> , IRequisitionDetailRepo
    {
        private RequisitionDetailRepo() { }

        private static RequisitionDetailRepo instance = new RequisitionDetailRepo();
        public static IRequisitionDetailRepo Instance
        {
            get { return instance; }
        }

        public int GetReservedCountForStationery(int stationeryId)
        {
            if (Context.RequisitionDetails.Any(x => x.StationeryId == stationeryId && x.Status.StartsWith("Reserved")))
            {
                return (int)(from rd in Context.RequisitionDetails
                             where rd.StationeryId == stationeryId
                             select rd.QuantityOrdered).Sum();
            }
            else
            {
                return 0;
            }
            
        }

        public List<RequisitionDetail> RequisitionDetailsEagerLoadStationeryNameAndUOM(int requisitionId)
        {
            return Context.RequisitionDetails.Include(x => x.Stationery.Description)
                .Include(x => x.Stationery.UnitOfMeasure).Where(x => x.RequisitionId == requisitionId)
                .ToList();
        }
    }
}