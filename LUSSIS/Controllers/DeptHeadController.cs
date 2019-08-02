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

        public DeptHeadController()
        {
            requisitionManagementService = RequisitionManagementService.Instance;
        }

        // GET: DeptHead
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ViewDepartmentRequisitions(LoginDTO loginDTO)
        {
            return View();
        }

        public ActionResult ReviewRequisitionDetails(LoginDTO loginDTO, int requisitionId)
        {
            return View();
        }

        public ActionResult ApproveRequisition(LoginDTO loginDTO, int requisitionId)
        {
            return View();
        }

        public ActionResult RejectRequisition(LoginDTO loginDTO, int requisitionId)
        {
            return View();
        }
    }
}