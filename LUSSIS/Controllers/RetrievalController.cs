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
            return View(model);
        }

        [HttpGet]
        public ActionResult SubmitRetrieval(RetrievalDTO retrieval)
        {
            LoginDTO loginDTO = retrieval.LoginDTO;
            retrievalService.completeRetrievalProcess(retrieval);

            return RedirectToAction("ViewRetrieval", loginDTO);
        }


    }
}