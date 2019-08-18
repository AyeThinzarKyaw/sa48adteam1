using LUSSIS.Models;
using LUSSIS.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;

namespace LUSSIS.Repositories
{
    public class PurchaseOrderDetailRepo : GenericRepo<PurchaseOrderDetail, int>, IPurchaseOrderDetailRepo
    {
        private PurchaseOrderDetailRepo() { }

        private static PurchaseOrderDetailRepo instance = new PurchaseOrderDetailRepo();

        public static IPurchaseOrderDetailRepo Instance
        {
            get { return instance; }
        }

        public List<PurchaseOrderDetail> GetPurchaseOrderDetailsBySupplierId(int SupplierId)
        {
            var results = from pod in Context.PurchaseOrderDetails
                          join po in Context.PurchaseOrders on pod.PurchaseOrderId equals po.Id
                          join e in Context.Employees on po.EmployeeId equals e.Id
                          join su in Context.Suppliers on po.SupplierId equals su.Id
                          join s in Context.Stationeries on pod.StationeryId equals s.Id
                          join c in Context.Categories on s.CategoryId equals c.Id
                          where su.Id == SupplierId
                          select pod;
            return results.ToList();
        }

        public List<PurchaseOrderDetail> GetPurchaseOrderDetailsBySupplierIdByCategoryName(int SupplierId, string CategoryName)
        {
            var results = from pod in Context.PurchaseOrderDetails
                          join po in Context.PurchaseOrders on pod.PurchaseOrderId equals po.Id
                          join e in Context.Employees on po.EmployeeId equals e.Id
                          join su in Context.Suppliers on po.SupplierId equals su.Id
                          join s in Context.Stationeries on pod.StationeryId equals s.Id
                          join c in Context.Categories on s.CategoryId equals c.Id
                          where su.Id == SupplierId && c.Type == CategoryName
                          select pod;
            return results.ToList();
        }

        public List<PurchaseOrderDetail> GetPurchaseOrderDetailsBySupplierIdByCategoryIdByItemId(int SupplierId, string CategoryName, string ItemName)
        {
            var results = from pod in Context.PurchaseOrderDetails
                          join po in Context.PurchaseOrders on pod.PurchaseOrderId equals po.Id
                          join e in Context.Employees on po.EmployeeId equals e.Id
                          join su in Context.Suppliers on po.SupplierId equals su.Id
                          join s in Context.Stationeries on pod.StationeryId equals s.Id
                          join c in Context.Categories on s.CategoryId equals c.Id
                          where su.Id == SupplierId && c.Type == CategoryName && s.Description == ItemName
                          select pod;
            return results.ToList();
        }

        public List<RequisitionDetail> GetRequisitionDetailsByDepartmentIdByCategoryIdByStationeryId(int DepartmentId, int CategoryId, int StationeryId)
        {
            var result = from rd in Context.RequisitionDetails
                         join r in Context.Requisitions on rd.RequisitionId equals r.Id
                         join e in Context.Employees on r.EmployeeId equals e.Id
                         join d in Context.Departments on e.DepartmentId equals d.Id
                         join s in Context.Stationeries on rd.StationeryId equals s.Id
                         join c in Context.Categories on s.CategoryId equals c.Id
                         where d.Id == DepartmentId && c.Id == CategoryId && s.Id == StationeryId
                         select rd;
            return result.ToList();
        }
    }
}