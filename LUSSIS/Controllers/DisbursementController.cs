﻿using LUSSIS.Models.DTOs;
using LUSSIS.Models;
using LUSSIS.Services.Interfaces;
using LUSSIS.Services;
using LUSSIS.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LUSSIS.Filters;

namespace LUSSIS.Controllers
{
    public class DisbursementController : Controller
    {
        IRequisitionCatalogueService RequisitionCatalogueService;
        DisbursementService disbursementService;

        // GET: Disbursement
        [Authorizer]
        public ActionResult Index()
        {
            if (Session["existinguser"] != null)
            {
                LoginDTO currentUser = (LoginDTO)Session["existinguser"];
                if (currentUser.RoleId != (int)Enums.Roles.StoreClerk && currentUser.RoleId != (int)Enums.Roles.DepartmentRepresentative)
                {
                    return RedirectToAction("RedirectToClerkOrDepartmentView", "Login");
                }

                disbursementService = new DisbursementService();
                List<DisbursementListDTO> ViewDepRepDisbursementList = disbursementService.GetDepRepDisbursementsDetails(currentUser.EmployeeId);
                List<DisbursementListDTO> ViewClerkDisbursementList = disbursementService.GetClerkDisbursementsDetails(currentUser.EmployeeId);

                if (ViewDepRepDisbursementList.Any(x => x.ReceivedEmployeeId == currentUser.EmployeeId))
                {
                    DisbursementListDTO model = new DisbursementListDTO { disbursementDTOList = ViewDepRepDisbursementList, ReceivedEmployeeId = currentUser.EmployeeId };
                    return View(model);
                }
                else if (ViewClerkDisbursementList.Any(x => x.DeliveredEmployeeId == currentUser.EmployeeId))
                {
                    DisbursementListDTO model = new DisbursementListDTO { disbursementDTOList = ViewClerkDisbursementList, DeliveredEmployeeId = currentUser.EmployeeId };
                    return View(model);
                }
                else return View();
            }
            return RedirectToAction("Index", "Login");
        }

        [Authorizer]
        public ActionResult Detail(int DisbursementId)
        {
            if (Session["existinguser"] != null)
            {
                LoginDTO currentUser = (LoginDTO)Session["existinguser"];
                if (currentUser.RoleId != (int)Enums.Roles.StoreClerk && currentUser.RoleId != (int)Enums.Roles.DepartmentRepresentative)
                {
                    return RedirectToAction("RedirectToClerkOrDepartmentView", "Login");
                }
               
                disbursementService = new DisbursementService();
                List<DisbursementListDTO> ViewDepRepDisbursementList = disbursementService.GetDepRepDisbursementsDetails(currentUser.EmployeeId);
                List<DisbursementListDTO> ViewClerkDisbursementList = disbursementService.GetClerkDisbursementsDetails(currentUser.EmployeeId);


                if (ViewDepRepDisbursementList.Any(x => x.ReceivedEmployeeId == currentUser.EmployeeId))
                {

                    DisbursementListDTO model = new DisbursementListDTO {  disbursementDTOList = ViewDepRepDisbursementList, ReceivedEmployeeId = currentUser.EmployeeId, DisbursementId = DisbursementId };
                    return View(model);
                }
                else if (ViewClerkDisbursementList.Any(x => x.DeliveredEmployeeId == currentUser.EmployeeId))
                {

                    DisbursementListDTO model = new DisbursementListDTO {disbursementDTOList = ViewClerkDisbursementList, DeliveredEmployeeId = currentUser.EmployeeId, DisbursementId = DisbursementId };
                    return View(model);
                }
                else return View();
            }
            return RedirectToAction("Index", "Login");
        }
    }
}