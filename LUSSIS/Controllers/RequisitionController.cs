using LUSSIS.Services;
using LUSSIS.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LUSSIS.Controllers
{
    public class RequisitionController : Controller
    {
        IRequisitionCatalogueService requisitionCatalogueService;

        IRequisitionManagementService requisitionManagementService;

        RequisitionController()
        {
            requisitionCatalogueService = RequisitionCatalogueService.Instance;
            requisitionManagementService = RequisitionManagementService.Instance;
        }

        // GET: Requisition
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ViewCatalogue(int employeeId)
        {


            return View();
        }
    }
}