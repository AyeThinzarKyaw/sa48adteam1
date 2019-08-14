using LUSSIS.Filters;
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

        [Authorizer]
        public ActionResult ViewDepartmentRequisitions()
        {
            if (Session["existinguser"] != null)
            {
                LoginDTO currentUser = (LoginDTO)Session["existinguser"];
                if (currentUser.RoleId != (int)Enums.Roles.DepartmentHead && currentUser.RoleId != (int)Enums.Roles.DepartmentCoverHead)
                {
                    return RedirectToAction("RedirectToClerkOrDepartmentView", "Login");
                }
                List<Requisition> deptReqs = requisitionManagementService.GetDepartmentRequisitions(currentUser.EmployeeId);
                RequisitionsDTO model = new RequisitionsDTO() { Requisitions = deptReqs };

                return View(model);
            }
            return RedirectToAction("Index", "Login");
        }

        [Authorizer]
        public ActionResult ReviewRequisitionDetails(int requisitionId)
        {
            if (Session["existinguser"] != null)
            {
                LoginDTO currentUser = (LoginDTO)Session["existinguser"];
                if (currentUser.RoleId != (int)Enums.Roles.DepartmentHead && currentUser.RoleId != (int)Enums.Roles.DepartmentCoverHead)
                {
                    return RedirectToAction("RedirectToClerkOrDepartmentView", "Login");
                }
                RequisitionDetailsDTO model = requisitionCatalogueService.GetRequisitionDetailsForSingleRequisition(requisitionId, currentUser.EmployeeId);
                //model.LoginDTO = loginDTO;
                return View(model);
            }
            return RedirectToAction("Index", "Login");
        }

        [Authorizer]
        public ActionResult ApproveRejectPendingRequisition(string button, RequisitionDetailsDTO viewModel)
        {
            if (Session["existinguser"] != null)
            {
                LoginDTO currentUser = (LoginDTO)Session["existinguser"];
                if (currentUser.RoleId != (int)Enums.Roles.DepartmentHead && currentUser.RoleId != (int)Enums.Roles.DepartmentCoverHead)
                {
                    return RedirectToAction("RedirectToClerkOrDepartmentView", "Login");
                }
                requisitionManagementService.ApproveRejectPendingRequisition(viewModel.RequisitionFormId, button, viewModel.Remarks);
                return RedirectToAction("ViewDepartmentRequisitions");
            }
            return RedirectToAction("Index", "Login");
        }
    }
}