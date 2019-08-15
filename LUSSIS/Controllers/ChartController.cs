using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LUSSIS.Controllers
{
    public class ChartController : Controller
    {
        // GET: Chart
        public ActionResult StationeryRequisitionTrend()
        {
            return View();
        }
        public ActionResult PurchaseOrderTrend()
        {
            return View();
        }

        public ActionResult InventoryHistoricalData()
        {
            return View();
        }
    }
}