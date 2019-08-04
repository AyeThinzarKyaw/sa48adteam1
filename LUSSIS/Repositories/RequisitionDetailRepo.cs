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
            return (from rd in Context.RequisitionDetails
                    where rd.StationeryId == stationeryId
                    where (rd.Status.Equals("RESERVED_PENDING") || rd.Status.Equals("PREPARING") || rd.Status.Equals("PENDING_COLLECTION"))
                    select (int?)rd.QuantityOrdered).Sum() ?? 0;
        }

        public int GetRequisitionCountForUnfulfilledStationery(int requisitionDetailId)
        {
            Requisition r = FindById(requisitionDetailId).Requisition;

            return (from rd in Context.RequisitionDetails
                    where rd.Id == requisitionDetailId
                    where rd.Requisition.DateTime < r.DateTime
                    where (rd.Status.Equals("RESERVED_PENDING") || rd.Status.Equals("PREPARING") || rd.Status.Equals("PENDING_COLLECTION"))
                    select (int?)rd.QuantityOrdered).Sum() ?? 0;
        }

        public List<RequisitionDetail> RequisitionDetailsEagerLoadStationery(int requisitionId)
        {
            return Context.RequisitionDetails.Include(x => x.Stationery)
                .Where(x => x.RequisitionId == requisitionId)
                .ToList();
        }

        public List<RequisitionDetail> RequisitionDetailsEagerLoadStationeryIncCategory(int requisitionId)
        {
            return Context.RequisitionDetails.Include(x => x.Stationery.Category)
                .Where(x => x.RequisitionId == requisitionId)
                .ToList();
        }

        public List<Requisition> GetUniqueRequisitionsForDisbursement(int disbursementId)
        {
            return Context.RequisitionDetails.Select(x => x.Requisition).Distinct().ToList();

        }
    }
}