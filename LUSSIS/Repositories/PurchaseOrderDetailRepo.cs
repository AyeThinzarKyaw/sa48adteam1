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
    }
}