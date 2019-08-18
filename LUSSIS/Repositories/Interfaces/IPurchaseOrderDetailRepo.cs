using LUSSIS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LUSSIS.Repositories.Interfaces
{
    public interface IPurchaseOrderDetailRepo : IGenericRepo<PurchaseOrderDetail, int>
    {
        List<PurchaseOrderDetail> GetPurchaseOrderDetailsBySupplierId(int SupplierId);

        List<PurchaseOrderDetail> GetPurchaseOrderDetailsBySupplierIdByCategoryName(int SupplierId, string CategoryName);

        List<PurchaseOrderDetail> GetPurchaseOrderDetailsBySupplierIdByCategoryIdByItemId(int SupplierId, string CategoryName, string ItemName);
    }
}