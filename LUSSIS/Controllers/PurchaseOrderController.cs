using LUSSIS.Models;
using LUSSIS.Models.DTOs;
using LUSSIS.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LUSSIS.Controllers
{
    public class PurchaseOrderController : Controller
    {

        // GET: All PurchaseOrder
        //By ATZK
        public ActionResult Index()
        {
            PurchaseOrderListDTO purchaseOrders = new PurchaseOrderListDTO();
            purchaseOrders.PurchaseOrders = PurchaseOrderService.Instance.getAllPurchaseOrders();

            return View(purchaseOrders);
        }

        // POST: Search PO between chosen dates
        //By ATZK
        [HttpPost]
        public ActionResult Index(PurchaseOrderListDTO purchaseOrderListDTO)
        {
            if(purchaseOrderListDTO.FromDate!=null && purchaseOrderListDTO.ToDate != null)
            {
                purchaseOrderListDTO.PurchaseOrders = PurchaseOrderService.Instance.getAllPurchaseOrders()
                                                    .Where(x => x.OrderDateTime.Date >= purchaseOrderListDTO.FromDate.Date
                                                    && x.OrderDateTime <= purchaseOrderListDTO.ToDate.Date)
                                                    .OrderByDescending(x => x.OrderDateTime);
            }
            else
            {
                purchaseOrderListDTO.PurchaseOrders = PurchaseOrderService.Instance.getAllPurchaseOrders()
                                                    .OrderByDescending(x => x.OrderDateTime);
            }
            return View(purchaseOrderListDTO);
        }

        // GET: View PurchaseOrder Catalogue to select items
        //By ATZK
        public ActionResult Catalogue()
        {
            return View(PurchaseOrderService.Instance.RetrievePurchaseOrderCatalogue());
        }

        // cancelPOStatus as cancelled
        //By ATZK
        public JsonResult CancelPO(int poId)
        {
            PurchaseOrder purchaseOrder = PurchaseOrderService.Instance.getPurchaseOrderById(poId);
            if (purchaseOrder!=null)
            {
                if (purchaseOrder.Status== Enum.GetName(typeof(Enums.POStatus), Enums.POStatus.OPEN))
                {
                    purchaseOrder.Status = Enum.GetName(typeof(Enums.POStatus), Enums.POStatus.CANCELLED);
                    PurchaseOrderService.Instance.UpdatePOStatus(purchaseOrder);
                    return Json(true, JsonRequestBehavior.AllowGet);
                }
                return Json(false, JsonRequestBehavior.AllowGet);
            }
            return Json(false, JsonRequestBehavior.AllowGet);
        }


        // View PO Details
        //By ATZK
        public ActionResult Detail(int poId)
        {
            PurchaseOrder purchaseOrder = PurchaseOrderService.Instance.getPurchaseOrderById(poId);
            
            return View(purchaseOrder);
        }


    }
}