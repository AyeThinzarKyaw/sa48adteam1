using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LUSSIS.Models;
using LUSSIS.Models.DTOs;

namespace LUSSIS.Services.Interfaces
{
    public interface IPurchaseOrderService
    {
        IEnumerable<PurchaseOrder> getAllPurchaseOrders();
        PurchaseOrder getPurchaseOrderById(int poId);
        IEnumerable<PO_getPOCatalogue_Result> RetrievePurchaseOrderCatalogue();
        void CreatePO(PurchaseOrder po);
        void UpdatePO(PurchaseOrder po);
        void CreatePODetail(PurchaseOrderDetail pod);
        void UpdatePODetail(PurchaseOrderDetail pod);

        void RaisePO(POCreateDTO poCreateDTO,int createdBy);
        List<ChartDTO> TrendChartInfo(int SupplierId, int CategoryId, int StationeryId);
        ChartFilteringDTO FilteringByAttributes();

    }
}
