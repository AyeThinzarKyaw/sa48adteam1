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
    public class RequisitionController : Controller
    {
        IRequisitionCatalogueService requisitionCatalogueService;
        IEmailNotificationService emailNotificationService;

        public RequisitionController()
        {
            requisitionCatalogueService = RequisitionCatalogueService.Instance;
            emailNotificationService = EmailNotificationService.Instance;
        }


        // GET: Requisition
        //[Authorizer]
        //public ActionResult Index()
        //{
        //    return View();
        //}

        [Authorizer]
        public ActionResult ViewCatalogue()
        {
            if (Session["existinguser"] != null)
            {
                LoginDTO currentUser = (LoginDTO)Session["existinguser"];
                if (currentUser.RoleId == (int)Enums.Roles.StoreClerk || currentUser.RoleId == (int)Enums.Roles.StoreSupervisor || currentUser.RoleId == (int)Enums.Roles.StoreManager)
                {
                    return RedirectToAction("RedirectToClerkOrDepartmentView", currentUser);
                }
                List<CatalogueItemDTO> catalogueItems = requisitionCatalogueService.GetCatalogueItems(currentUser.EmployeeId);
                FormRequisitionDTO model = new FormRequisitionDTO { CatalogueItems = catalogueItems };

                return View(model);
            }
            return RedirectToAction("Index", "Login");
        }
        [Authorizer]
        public JsonResult AddItemToCart(int employeeId, int stationeryId, int inputQty)
        {
            CatalogueItemDTO catalogueItemDTO = requisitionCatalogueService.AddCartDetail(employeeId, stationeryId, inputQty);
            return Json(new
            {
                availstatus = catalogueItemDTO.StockAvailability.ToString(),
                lowstockcount = catalogueItemDTO.LowStockAvailability,
                reserved = catalogueItemDTO.ReservedCount,
                waitlist = catalogueItemDTO.WaitlistCount
            }, JsonRequestBehavior.AllowGet);

        }
        [Authorizer]
        public JsonResult RemoveItemFromCart(int employeeId, int stationeryId)
        {
            CatalogueItemDTO catalogueItemDTO = requisitionCatalogueService.RemoveCartDetail(employeeId, stationeryId);
            return Json(new
            {
                availstatus = catalogueItemDTO.StockAvailability.ToString(),
                lowstockcount = catalogueItemDTO.LowStockAvailability
            }, JsonRequestBehavior.AllowGet);
        }
        [Authorizer]
        public ActionResult SubmitRequisitionForm()
        {
            if (Session["existinguser"] != null)
            {
                LoginDTO currentUser = (LoginDTO)Session["existinguser"];
                if (currentUser.RoleId == (int)Enums.Roles.StoreClerk || currentUser.RoleId == (int)Enums.Roles.StoreSupervisor || currentUser.RoleId == (int)Enums.Roles.StoreManager)
                {
                    return RedirectToAction("RedirectToClerkOrDepartmentView", currentUser);
                }
                //convert all cart items into requsitiondetails for this requisition
                Requisition newRequisition = requisitionCatalogueService.ConvertCartDetailsToRequisitionDetails(currentUser.EmployeeId);

                //notify dept head for approval
                emailNotificationService.NotifyDeptHeadToApprovePendingRequisition(newRequisition);

                return RedirectToAction("ViewCatalogue");
                //return to catalogue/dashboard view
                //ViewCatalogue(loginDTO); 
            }
            return RedirectToAction("Index", "Login");
        }
        [Authorizer]
        public ActionResult ViewRequisitionList()
        {
            if (Session["existinguser"] != null)
            {
                LoginDTO currentUser = (LoginDTO)Session["existinguser"];
                if (currentUser.RoleId == (int)Enums.Roles.StoreClerk || currentUser.RoleId == (int)Enums.Roles.StoreSupervisor || currentUser.RoleId == (int)Enums.Roles.StoreManager)
                {
                    return RedirectToAction("RedirectToClerkOrDepartmentView", currentUser);
                }
                //Get all requsition from this employee
                List<Requisition> requisitionHistory = requisitionCatalogueService.GetPersonalRequisitionHistory(currentUser.EmployeeId);

                RequisitionsDTO model = new RequisitionsDTO() { Requisitions = requisitionHistory };

                //viewData add additional data
                return View(model);
            }
            return RedirectToAction("Index", "Login");
        }

        [Authorizer]
        public ActionResult ViewRequisitionDetail(int requisitionId)
        {
            if (Session["existinguser"] != null)
            {
                LoginDTO currentUser = (LoginDTO)Session["existinguser"];
                if (currentUser.RoleId == (int)Enums.Roles.StoreClerk || currentUser.RoleId == (int)Enums.Roles.StoreSupervisor || currentUser.RoleId == (int)Enums.Roles.StoreManager)
                {
                    return RedirectToAction("RedirectToClerkOrDepartmentView", currentUser);
                }
                RequisitionDetailsDTO model = requisitionCatalogueService.GetRequisitionDetailsForSingleRequisition(requisitionId, currentUser.EmployeeId);
                return View(model);
            }
            return RedirectToAction("Index", "Login");
        }

        [Authorizer]
        public ActionResult CancelPendingRequisition(int requisitionId)
        {
            if (Session["existinguser"] != null)
            {
                LoginDTO currentUser = (LoginDTO)Session["existinguser"];
                if (currentUser.RoleId == (int)Enums.Roles.StoreClerk || currentUser.RoleId == (int)Enums.Roles.StoreSupervisor || currentUser.RoleId == (int)Enums.Roles.StoreManager)
                {
                    return RedirectToAction("RedirectToClerkOrDepartmentView", currentUser);
                }
                if (requisitionCatalogueService.CancelPendingRequisition(requisitionId, currentUser.EmployeeId))
                {
                    return RedirectToAction("ViewRequisitionList");
                }
                return RedirectToAction("ViewRequisitionList");
            }
            return RedirectToAction("Index", "Login");
        }
        [Authorizer]
        public ActionResult CancelWaitlistedRequisitionDetail(int requisitionDetailId, int requisitionId)
        {
            if (Session["existinguser"] != null)
            {
                LoginDTO currentUser = (LoginDTO)Session["existinguser"];
                if (currentUser.RoleId == (int)Enums.Roles.StoreClerk || currentUser.RoleId == (int)Enums.Roles.StoreSupervisor || currentUser.RoleId == (int)Enums.Roles.StoreManager)
                {
                    return RedirectToAction("RedirectToClerkOrDepartmentView", currentUser);
                }
                requisitionCatalogueService.CancelWaitlistedRequisitionDetail(requisitionDetailId, currentUser.EmployeeId);
                return RedirectToAction("ViewRequisitionDetail",
                    new
                    {
                        @requisitionId = requisitionId
                    });
            }
            return RedirectToAction("Index", "Login");
        }

    }
}