using LUSSIS.Models;
using LUSSIS.Models.DTOs;
using LUSSIS.Services;
using LUSSIS.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LUSSIS.Controllers
{
    public class ClerkRequisitionsController : Controller
    {
        IRequisitionCatalogueService requisitionCatalogueService;

        public ClerkRequisitionsController()
        {
            requisitionCatalogueService = RequisitionCatalogueService.Instance;
        }

        // GET: ClerkRequisitions
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ViewSchoolRequisitions(LoginDTO loginDTO)
        {
            List<Requisition> requisitions = requisitionCatalogueService.GetSchoolRequisitionsWithEmployeeAndDept();
            //return requisitionsDTO model
            RequisitionsDTO model = new RequisitionsDTO() { LoginDTO = loginDTO, Requisitions = requisitions };
            return View(model);
        }

        public ActionResult ViewRequisitionDetails(LoginDTO loginDTO, int requisitionId)
        {
            RequisitionDetailsDTO model = requisitionCatalogueService.GetRequisitionDetailsForClerk(requisitionId);
            model.LoginDTO = loginDTO;

            return View(model);
        }
    }
}