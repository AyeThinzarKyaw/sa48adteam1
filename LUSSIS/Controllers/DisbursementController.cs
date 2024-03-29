﻿using LUSSIS.Models.DTOs;
using LUSSIS.Models;
using LUSSIS.Services.Interfaces;
using LUSSIS.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using LUSSIS.Filters;

namespace LUSSIS.Controllers
{
    public class DisbursementController : Controller
    {
        DisbursementService disbursementService;
        EmailNotificationService emailNotificationService;

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
                List<DisbursementDetailsDTO> disbursementList = new List<DisbursementDetailsDTO>();
                if (currentUser.RoleId==(int)Enums.Roles.DepartmentRepresentative)
                {
                    disbursementList= disbursementService.GetDepRepDisbursementsDetails(currentUser.EmployeeId);
                    DisbursementDTO model = new DisbursementDTO { DisbursementDetailsDTOList = disbursementList, ReceivedEmployeeId = currentUser.EmployeeId };
                    return View(model);
                }
                else if (currentUser.RoleId==(int)Enums.Roles.StoreClerk)
                {
                    disbursementList = disbursementService.GetClerkDisbursementsDetails(currentUser.EmployeeId);
                    DisbursementDTO model = new DisbursementDTO { DisbursementDetailsDTOList = disbursementList, DeliveredEmployeeId = currentUser.EmployeeId };
                    return View(model);
                }
               
                //disbursementService = new DisbursementService();
                //List<DisbursementDetailsDTO> ViewDepRepDisbursementList = disbursementService.GetDepRepDisbursementsDetails(currentUser.EmployeeId);
                //List<DisbursementDetailsDTO> ViewClerkDisbursementList = disbursementService.GetClerkDisbursementsDetails(currentUser.EmployeeId);

                //if (ViewDepRepDisbursementList.Any(x => x.ReceivedEmployeeId == currentUser.EmployeeId))
                //{
                //    DisbursementDTO model = new DisbursementDTO { DisbursementDetailsDTOList = ViewDepRepDisbursementList, ReceivedEmployeeId = currentUser.EmployeeId };
                //    return View(model);
                //}
                //else if (ViewClerkDisbursementList.Any(x => x.DeliveredEmployeeId == currentUser.EmployeeId))
                //{
                //    DisbursementDTO model = new DisbursementDTO { DisbursementDetailsDTOList = ViewClerkDisbursementList, DeliveredEmployeeId = currentUser.EmployeeId };
                //    return View(model);
                //}
                //else return View();
            }
            return RedirectToAction("Index", "Login");
        }

        public ActionResult Detail(int DisbursementId, string DisbursementStatus)
        {
            if (Session["existinguser"] != null)
            {
                LoginDTO currentUser = (LoginDTO)Session["existinguser"];
                if (currentUser.RoleId != (int)Enums.Roles.StoreClerk && currentUser.RoleId != (int)Enums.Roles.DepartmentRepresentative)
                {
                    return RedirectToAction("RedirectToClerkOrDepartmentView", "Login");
                }

                disbursementService = new DisbursementService();
                List<DisbursementDetailsDTO> ViewDepRepDisbursementList = disbursementService.GetDepRepDisbursementsDetails(currentUser.EmployeeId);
                List<DisbursementDetailsDTO> ViewClerkDisbursementList = disbursementService.GetClerkDisbursementsDetails(currentUser.EmployeeId);


                if (ViewDepRepDisbursementList.Any(x => x.ReceivedEmployeeId == currentUser.EmployeeId))
                {

                    DisbursementDTO model = new DisbursementDTO {  DisbursementDetailsDTOList = ViewDepRepDisbursementList, ReceivedEmployeeId = currentUser.EmployeeId, DisbursementId = DisbursementId };
                    return View(model);
                }
                else if (ViewClerkDisbursementList.Any(x => x.DeliveredEmployeeId == currentUser.EmployeeId))
                {
                    DisbursementDTO model = new DisbursementDTO { DisbursementDetailsDTOList = ViewClerkDisbursementList, DeliveredEmployeeId = currentUser.EmployeeId, DisbursementId = DisbursementId };
                   

                    if (DisbursementStatus == "PENDING_COLLECTION")
                    {
                        foreach (var vcdl in ViewClerkDisbursementList)
                        {
                            
                            
                            emailNotificationService = new EmailNotificationService();
                            emailNotificationService.SendNotificationEmail(receipient: "sa48team1@gmail.com", subject: "Disbursement Details for " +vcdl.ReceivedEmployeedDepName+" Department"+" on"+ DateTime.Now.ToString("dd/MM/yyyy"), body: "Dear "+vcdl.ReceivedEmployeedName+":\n"+"\nYour Department Items is ready for collection, Please refer to Disbursement Number: "+vcdl.DisbursementId+"\n\nBest Regards\n\n"+vcdl.DeliveredEmployeeName, attachments: null);

                            return View(model);

                        }
                    }
                    return View(model);
                }
                else return View();
            }
            return RedirectToAction("Index", "Login");
        }


    }
}