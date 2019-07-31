using LUSSIS.Models;
using LUSSIS.Repositories.Interfaces;
using System;
using System.Collections.Generic;
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
            return (from rd in Context.RequisitionDetails
                    where rd.StationeryId == stationeryId
                    select rd.QuantityOrdered).Sum();
        }
    }
}