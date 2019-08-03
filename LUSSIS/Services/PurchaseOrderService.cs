using LUSSIS.Models;
using LUSSIS.Repositories;
using LUSSIS.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LUSSIS.Services
{
    public class PurchaseOrderService :IPurchaseOrderService
    {
        private PurchaseOrderService() { }

        private static PurchaseOrderService instance = new PurchaseOrderService();
        public static IPurchaseOrderService Instance
        {
            get { return instance; }
        }

        public IEnumerable<PurchaseOrder> getAllPurchaseOrders()
        {
            return PurchaseOrderRepo.Instance.FindAll().OrderByDescending(x=>x.OrderDateTime).ToList();
        }

        public PurchaseOrder getPurchaseOrderById(int poId)
        {
            return PurchaseOrderRepo.Instance.FindById(poId);
        }

        public IEnumerable<PO_getPOCatalogue_Result> RetrievePurchaseOrderCatalogue()
        {
            return PurchaseOrderRepo.Instance.GetPOCatalogue();
        }
        public void UpdatePOStatus(PurchaseOrder po)
        {
            PurchaseOrderRepo.Instance.Update(po);
        }
    }
}