using LUSSIS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LUSSIS.Repositories.Interfaces
{
    public interface IPurchaseOrderRepo : IGenericRepo<PurchaseOrder, int>
    {
        IEnumerable<PurchaseOrder> GetPurchaseOrderByStationeryId(int stationeryId);

        IEnumerable<PO_getPOCatalogue_Result> GetPOCatalogue();

        List<int> getPurchaseOrderIdsWithClosedStatus();
    }
}