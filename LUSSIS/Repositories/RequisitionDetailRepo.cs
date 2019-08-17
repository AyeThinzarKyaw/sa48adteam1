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
            return Context.RequisitionDetails.Where(x=> x.DisbursementId == disbursementId).Select(x => x.Requisition).Distinct().ToList();

        }

        public List<IGrouping<int, RequisitionDetail>> GetUnfulfilledRequisitionDetailsGroupedByDept()
        {
            return Context.RequisitionDetails.Where(x => x.Status.Equals("COLLECTED")).GroupBy(x => x.Requisition.Employee.DepartmentId).ToList();
        }

        

        public List<RequisitionDetail> GetRequisitionDetailsByClerkDisbursementId(int EmployeeId)
        {
            var result = from rd in Context.RequisitionDetails
                         join d in Context.Disbursements on rd.DisbursementId equals d.Id
                         join s in Context.Stationeries on rd.StationeryId equals s.Id
                         join r in Context.Requisitions on rd.RequisitionId equals r.Id
                         join e in Context.Employees on d.ReceivedEmployeeId equals e.Id
                         join dep in Context.Departments on e.DepartmentId equals dep.Id
                         join cp in Context.CollectionPoints on dep.CollectionPointId equals cp.Id
                         where d.DeliveredEmployeeId == EmployeeId
                         select rd;
            return result.ToList();
        }

        public List<RequisitionDetail> GetRequisitionDetailsByDepRepDisbursementId(int EmployeeId)
        {
            var result = from rd in Context.RequisitionDetails
                         join d in Context.Disbursements on rd.DisbursementId equals d.Id
                         join s in Context.Stationeries on rd.StationeryId equals s.Id
                         join r in Context.Requisitions on rd.RequisitionId equals r.Id
                         join e in Context.Employees on d.ReceivedEmployeeId equals e.Id
                         join dep in Context.Departments on e.DepartmentId equals dep.Id
                         join cp in Context.CollectionPoints on dep.CollectionPointId equals cp.Id
                         where d.ReceivedEmployeeId == EmployeeId
                         select rd;
            return result.ToList();
        }

        public List<RequisitionDetail> GetRequisitionDetailsByDepartmentId(int DepartmentId)
        {
            var result = from rd in Context.RequisitionDetails
                         join r in Context.Requisitions on rd.RequisitionId equals r.Id
                         join e in Context.Employees on r.EmployeeId equals e.Id
                         join d in Context.Departments on e.DepartmentId equals d.Id
                         join s in Context.Stationeries on rd.StationeryId equals s.Id
                         join c in Context.Categories on s.CategoryId equals c.Id
                         where d.Id == DepartmentId
                         select rd;

            return result.ToList();
        }
    }
}