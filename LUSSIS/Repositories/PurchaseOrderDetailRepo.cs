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

    }
}