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
    public class ClerkRequisitionsController : Controller
    {
        IRequisitionCatalogueService requisitionCatalogueService;

        public ClerkRequisitionsController()
        {
            requisitionCatalogueService = RequisitionCatalogueService.Instance;
        }


        [Authorizer]
        public ActionResult ViewSchoolRequisitions()
        {
            if (Session["existinguser"] != null)
            {
                LoginDTO currentUser = (LoginDTO)Session["existinguser"];
                if (currentUser.RoleId != (int)Enums.Roles.StoreClerk)
                {
                    return RedirectToAction("RedirectToClerkOrDepartmentView", "Login");
                }
                List<Requisition> requisitions = requisitionCatalogueService.GetSchoolRequisitionsWithEmployeeAndDept();
                //return requisitionsDTO model
                RequisitionsDTO model = new RequisitionsDTO() { Requisitions = requisitions };
                return View(model);
            }
            return RedirectToAction("Index", "Login");
        }
        [Authorizer]
        public ActionResult ViewRequisitionDetails(int requisitionId)
        {
            if (Session["existinguser"] != null)
            {
                LoginDTO currentUser = (LoginDTO)Session["existinguser"];
                if (currentUser.RoleId != (int)Enums.Roles.StoreClerk)
                {
                    return RedirectToAction("RedirectToClerkOrDepartmentView", "Login");
                }
                RequisitionDetailsDTO model = requisitionCatalogueService.GetRequisitionDetailsForClerk(requisitionId);
                //model.LoginDTO = loginDTO;
                return View(model);
            }
            return RedirectToAction("Index", "Login");
        }
        [Authorizer]
        public ActionResult ViewOwedItems()
        {
            if (Session["existinguser"] != null)
            {
                LoginDTO currentUser = (LoginDTO)Session["existinguser"];
                if (currentUser.RoleId != (int)Enums.Roles.StoreClerk)
                {
                    return RedirectToAction("RedirectToClerkOrDepartmentView", "Login");
                }
                List<DeptOwedItemDTO> deptOwedItems = requisitionCatalogueService.GetListOfDeptOwedItems();
                return View(new VMOwedItemsDTO { DeptOwedItems = deptOwedItems });
            }
            return RedirectToAction("Index", "Login");
        }


    }
}