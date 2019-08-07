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

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ViewRetrieval(LoginDTO loginDTO)
        {
            loginDTO.EmployeeId = 11;
            RetrievalDTO model = retrievalService.constructRetrievalDTO(loginDTO);
            model.LoginDTO = loginDTO;

            TempData["RetrievalModel"] = model;

            return View(model);
        }


        public JsonResult UpdateRetrievalQuantity(int stationeryId, int quantity)
        {
            RetrievalDTO model = (RetrievalDTO)TempData["RetrievalModel"];

            model.RetrievalItem.Single(x => x.StationeryId == stationeryId).RetrievedQty = quantity;

            TempData["RetrievalModel"] = model;

            return Json(true, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult SubmitRetrieval()
        {
            RetrievalDTO retrieval = (RetrievalDTO)TempData["RetrievalModel"];
            LoginDTO loginDTO = retrieval.LoginDTO;
            retrievalService.completeRetrievalProcess(retrieval);

            return RedirectToAction("ViewRetrieval", loginDTO);
        }


        public ActionResult ViewAdHocRetrievalMenu(LoginDTO loginDTO)
        {
            loginDTO.EmployeeId = 11;
            AdHocRetrievalMenuDTO model = retrievalService.generateAdHocRetrievalMenuDTO();

            model.LoginDTO = loginDTO;

            TempData["AdHocRetrievalMenuModel"] = model;

            return View(model);
        }


        public JsonResult SelectRetrievalId(int requisitionId)
        {
            AdHocRetrievalMenuDTO model = (AdHocRetrievalMenuDTO)TempData["AdHocRetrievalMenuModel"];

            model.requisitionId = requisitionId;

            TempData["AdHocRetrievalMenuModel"] = model;

            return Json(true, JsonRequestBehavior.AllowGet);
        }


        [HttpGet]
        public ActionResult RetrieveSelectedAdHocRetrieval()
        {
            AdHocRetrievalMenuDTO model = (AdHocRetrievalMenuDTO)TempData["AdHocRetrievalMenuModel"];

            RetrievalDTO rtM = new RetrievalDTO() { LoginDTO = model.LoginDTO, AdHocRetrievalId = model.requisitionId };
            TempData["rtM"] = rtM;

            return RedirectToAction("ViewSelectedAdHocRetrieval");
        }

        // to code

        public ActionResult ViewSelectedAdHocRetrieval()
        {
            RetrievalDTO rtM = (RetrievalDTO)TempData["rtM"];

            LoginDTO loginDTO = rtM.LoginDTO;

            int requisitionId = 1;

            RetrievalDTO model = retrievalService.constructAdHocRetrievalDTO(loginDTO, requisitionId);

            model.LoginDTO = loginDTO;

            TempData["RetrievalModel"] = model;

            return View(model);
        }



    }
}