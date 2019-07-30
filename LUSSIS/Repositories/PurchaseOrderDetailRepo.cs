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
    
    }
}