using LUSSIS.Models.DTOs;
using LUSSIS.Models;
using LUSSIS.Services.Interfaces;
using LUSSIS.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LUSSIS.Controllers
{
    public class DisbursementController : Controller
    {
        IRequisitionCatalogueService RequisitionCatalogueService;
        DisbursementService disbursementService;

        // GET: Disbursement
        public ActionResult Index(LoginDTO LoginDTO)
        {
            LoginDTO.EmployeeId = 15; //hard coded ID
            disbursementService = new DisbursementService();
            List<DisbursementDetailsDTO> ViewDepRepDisbursementList = disbursementService.GetDepRepDisbursementsDetails(LoginDTO.EmployeeId);
            List<DisbursementDetailsDTO> ViewClerkDisbursementList = disbursementService.GetClerkDisbursementsDetails(LoginDTO.EmployeeId);

            if (ViewDepRepDisbursementList.Any(x => x.ReceivedEmployeeId == LoginDTO.EmployeeId))
            {
                DisbursementDTO model = new DisbursementDTO { LoginDTO = LoginDTO, DisbursementDetailsDTOList = ViewDepRepDisbursementList, ReceivedEmployeeId = LoginDTO.EmployeeId };
                return View(model);
            }
            else if (ViewClerkDisbursementList.Any(x => x.DeliveredEmployeeId == LoginDTO.EmployeeId))
            {
                DisbursementDTO model = new DisbursementDTO { LoginDTO = LoginDTO, DisbursementDetailsDTOList = ViewClerkDisbursementList, DeliveredEmployeeId = LoginDTO.EmployeeId };
                return View(model);
            }
            else return View();
        }

        public ActionResult Detail(LoginDTO LoginDTO, int DisbursementId)
        {
            LoginDTO.EmployeeId = 15; //hard coded ID
            disbursementService = new DisbursementService();
            List<DisbursementDetailsDTO> ViewDepRepDisbursementList = disbursementService.GetDepRepDisbursementsDetails(LoginDTO.EmployeeId);
            List<DisbursementDetailsDTO> ViewClerkDisbursementList = disbursementService.GetClerkDisbursementsDetails(LoginDTO.EmployeeId);


            if (ViewDepRepDisbursementList.Any(x => x.ReceivedEmployeeId == LoginDTO.EmployeeId))
            {

                DisbursementDTO model = new DisbursementDTO { LoginDTO = LoginDTO, DisbursementDetailsDTOList = ViewDepRepDisbursementList, ReceivedEmployeeId = LoginDTO.EmployeeId, DisbursementId = DisbursementId };
                return View(model);
            }
            else if (ViewClerkDisbursementList.Any(x => x.DeliveredEmployeeId == LoginDTO.EmployeeId))
            {

                DisbursementDTO model = new DisbursementDTO { LoginDTO = LoginDTO, DisbursementDetailsDTOList = ViewClerkDisbursementList, DeliveredEmployeeId = LoginDTO.EmployeeId, DisbursementId = DisbursementId };
                return View(model);
            }
            else return View();
        }
    }
}