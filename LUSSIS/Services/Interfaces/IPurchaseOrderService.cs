using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LUSSIS.Models;

namespace LUSSIS.Services.Interfaces
{
    public interface IPurchaseOrderService
    {
        IEnumerable<PurchaseOrder> getAllPurchaseOrders();
        void RetrievePurchaseOrderCatalogue();
    }
}
