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
    public class RetrievalController : Controller
    {
        IRetrievalService retrievalService;

        public RetrievalController()
        {
            retrievalService = RetrievalService.Instance;
        }

        [Authorizer]
        public ActionResult ViewRetrieval()
        {
            if (Session["existinguser"] != null)
            {
                LoginDTO currentUser = (LoginDTO)Session["existinguser"];
                if (currentUser.RoleId != (int)Enums.Roles.StoreClerk)
                {
                    return RedirectToAction("RedirectToClerkOrDepartmentView", "Login");
                }
                RetrievalDTO model = retrievalService.constructRetrievalDTO(currentUser);
                TempData["RetrievalModel"] = model;
                return View(model);
            }
            return RedirectToAction("Index", "Login");
        }

        [Authorizer]
        public JsonResult UpdateRetrievalQuantity(int stationeryId, int quantity)
        {
            RetrievalDTO model = (RetrievalDTO)TempData["RetrievalModel"];
            model.RetrievalItem.Single(x => x.StationeryId == stationeryId).RetrievedQty = quantity;
            TempData["RetrievalModel"] = model;
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        [Authorizer]
        [HttpPost]
        public ActionResult SubmitRetrieval()
        {
            if (Session["existinguser"] != null)
            {
                LoginDTO currentUser = (LoginDTO)Session["existinguser"];
                if (currentUser.RoleId != (int)Enums.Roles.StoreClerk)
                {
                    return RedirectToAction("RedirectToClerkOrDepartmentView", "Login");
                }
                RetrievalDTO retrieval = (RetrievalDTO)TempData["RetrievalModel"];
                LoginDTO loginDTO = currentUser;
                retrievalService.completeRetrievalProcess(retrieval,currentUser.EmployeeId);
                return RedirectToAction("ViewRetrieval");
            }
            return RedirectToAction("Index", "Login");
        }

        [Authorizer]
        public ActionResult ViewAdHocRetrievalMenu()
        {
            if (Session["existinguser"] != null)
            {
                LoginDTO currentUser = (LoginDTO)Session["existinguser"];
                if (currentUser.RoleId != (int)Enums.Roles.StoreClerk)
                {
                    return RedirectToAction("RedirectToClerkOrDepartmentView", "Login");
                }
                AdHocRetrievalMenuDTO model = retrievalService.generateAdHocRetrievalMenuDTO();
                TempData["AdHocRetrievalMenuModel"] = model;
                return View(model);
            }
            return RedirectToAction("Index", "Login");
        }


        [Authorizer]
        public JsonResult SelectRetrievalId(int requisitionId)
        {
            AdHocRetrievalMenuDTO model = (AdHocRetrievalMenuDTO)TempData["AdHocRetrievalMenuModel"];
            model.RequisitionId = requisitionId;
            TempData["AdHocRetrievalMenuModel"] = model;
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        [Authorizer]
        [HttpGet]
        public ActionResult RetrieveSelectedAdHocRetrieval()
        {
            if (Session["existinguser"] != null)
            {
                LoginDTO currentUser = (LoginDTO)Session["existinguser"];
                if (currentUser.RoleId != (int)Enums.Roles.StoreClerk)
                {
                    return RedirectToAction("RedirectToClerkOrDepartmentView", "Login");
                }
                AdHocRetrievalMenuDTO model = (AdHocRetrievalMenuDTO)TempData["AdHocRetrievalMenuModel"];
                RetrievalDTO rtM = new RetrievalDTO() { AdHocRetrievalId = model.RequisitionId };
                return RedirectToAction("ViewSelectedAdHocRetrieval", new { @requisitionId = model.RequisitionId });
            }
            return RedirectToAction("Index", "Login");
        }

        [Authorizer]
        public ActionResult ViewSelectedAdHocRetrieval(int requisitionId)
        {
            if (Session["existinguser"] != null)
            {
                LoginDTO currentUser = (LoginDTO)Session["existinguser"];
                if (currentUser.RoleId != (int)Enums.Roles.StoreClerk)
                {
                    return RedirectToAction("RedirectToClerkOrDepartmentView", "Login");
                }
                RetrievalDTO model = retrievalService.constructAdHocRetrievalDTO(currentUser, requisitionId);
                TempData["RetrievalModel"] = model;
                return View(model);
            }
            return RedirectToAction("Index", "Login");
        }
    }
}