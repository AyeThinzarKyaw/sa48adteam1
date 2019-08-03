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
    public class DeptHeadController : Controller
    {
        IRequisitionManagementService requisitionManagementService;
        IRequisitionCatalogueService requisitionCatalogueService;

        public DeptHeadController()
        {
            requisitionManagementService = RequisitionManagementService.Instance;
            requisitionCatalogueService = RequisitionCatalogueService.Instance;
        }

        // GET: DeptHead
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ViewDepartmentRequisitions(LoginDTO loginDTO)
        {
            List<Requisition> deptReqs = requisitionManagementService.GetDepartmentRequisitions(loginDTO.EmployeeId);
            RequisitionsDTO model = new RequisitionsDTO() { LoginDTO = loginDTO, Requisitions = deptReqs };

            return View(model);
        }

        public ActionResult ReviewRequisitionDetails(LoginDTO loginDTO, int requisitionId)
        {
            RequisitionDetailsDTO model = requisitionCatalogueService.GetRequisitionDetailsForSingleRequisition(requisitionId, loginDTO.EmployeeId);
            model.LoginDTO = loginDTO;
            return View(model);
        }

        public ActionResult ApproveRejectPendingRequisition(string button, RequisitionDetailsDTO viewModel)
        {
            requisitionManagementService.ApproveRejectPendingRequisition(viewModel.RequisitionFormId, button, viewModel.Remarks);
            return RedirectToAction("ViewDepartmentRequisitions", viewModel.LoginDTO);
        }
    }
}