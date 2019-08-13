using LUSSIS.Models.DTOs;
using LUSSIS.Models;
using LUSSIS.Services.Interfaces;
using LUSSIS.Services;
using LUSSIS.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;

namespace LUSSIS.Controllers
{
    public class DisbursementController : Controller
    {
        IRequisitionCatalogueService RequisitionCatalogueService;
        DisbursementService disbursementService;
        EmailNotificationService emailNotificationService;

        // GET: Disbursement
        public ActionResult Index(LoginDTO LoginDTO)
        {
            LoginDTO.EmployeeId = 9; //hard coded ID
            disbursementService = new DisbursementService();
            List<DisbursementListDTO> ViewDepRepDisbursementList = disbursementService.GetDepRepDisbursementsDetails(LoginDTO.EmployeeId);
            List<DisbursementListDTO> ViewClerkDisbursementList = disbursementService.GetClerkDisbursementsDetails(LoginDTO.EmployeeId);

            if (ViewDepRepDisbursementList.Any(x => x.ReceivedEmployeeId == LoginDTO.EmployeeId))
            {
                DisbursementListDTO model = new DisbursementListDTO { LoginDTO = LoginDTO, disbursementDTOList = ViewDepRepDisbursementList, ReceivedEmployeeId = LoginDTO.EmployeeId };
                return View(model);
            }
            else if (ViewClerkDisbursementList.Any(x => x.DeliveredEmployeeId == LoginDTO.EmployeeId))
            {
                DisbursementListDTO model = new DisbursementListDTO { LoginDTO = LoginDTO, disbursementDTOList = ViewClerkDisbursementList, DeliveredEmployeeId = LoginDTO.EmployeeId };
                return View(model);
            }
            else return View();
        }

        public ActionResult Detail(LoginDTO LoginDTO, int DisbursementId, string DisbursementStatus)
        {
            LoginDTO.EmployeeId = 9; //hard coded ID
            disbursementService = new DisbursementService();
            List<DisbursementListDTO> ViewDepRepDisbursementList = disbursementService.GetDepRepDisbursementsDetails(LoginDTO.EmployeeId);
            List<DisbursementListDTO> ViewClerkDisbursementList = disbursementService.GetClerkDisbursementsDetails(LoginDTO.EmployeeId);


            if (ViewDepRepDisbursementList.Any(x => x.ReceivedEmployeeId == LoginDTO.EmployeeId))
            {

                DisbursementListDTO model = new DisbursementListDTO { LoginDTO = LoginDTO, disbursementDTOList = ViewDepRepDisbursementList, ReceivedEmployeeId = LoginDTO.EmployeeId, DisbursementId = DisbursementId };
                return View(model);
            }
            else if (ViewClerkDisbursementList.Any(x => x.DeliveredEmployeeId == LoginDTO.EmployeeId))
            {
                DisbursementListDTO model = new DisbursementListDTO { LoginDTO = LoginDTO, disbursementDTOList = ViewClerkDisbursementList, DeliveredEmployeeId = LoginDTO.EmployeeId, DisbursementId = DisbursementId };

                if (DisbursementStatus == "PENDING_COLLECTION")
                {
                    foreach (var vcdl in ViewClerkDisbursementList)
                    {

                        emailNotificationService = new EmailNotificationService();
                        emailNotificationService.SendNotificationEmail(receipient: "sa48team1@gmail.com", subject: "Disbursement Details for " + DateTime.Now.ToString("dd/MM/yyyy"), body: "Please collect your department items.", attachments: null);

                        return View(model);

                    }
                }
                return View(model);
            }
            else return View();
        }


    }
}