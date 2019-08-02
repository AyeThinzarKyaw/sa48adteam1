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


        public RequisitionController()
        {
            requisitionCatalogueService = RequisitionCatalogueService.Instance;
        }

        // GET: Requisition
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ViewCatalogue(LoginDTO loginDTO)
        {
            List<CatalogueItemDTO> catalogueItems = requisitionCatalogueService.GetCatalogueItems(loginDTO.EmployeeId);
            FormRequisitionDTO model = new FormRequisitionDTO { CatalogueItems = catalogueItems };

            return View(model);
        }

        public JsonResult AddItemToCart(int employeeId, int stationeryId, int inputQty)
        {
            CatalogueItemDTO catalogueItemDTO = requisitionCatalogueService.AddCartDetail(employeeId, stationeryId, inputQty);
            return Json(new { availstatus = catalogueItemDTO.StockAvailability.ToString(),
                lowstockcount = catalogueItemDTO.LowStockAvailability,
                reserved = catalogueItemDTO.ReservedCount,
                waitlist = catalogueItemDTO.WaitlistCount }, JsonRequestBehavior.AllowGet);

        }

        public JsonResult RemoveItemFromCart(int employeeId, int stationeryId)
        {
            CatalogueItemDTO catalogueItemDTO = requisitionCatalogueService.RemoveCartDetail(employeeId, stationeryId);
            return Json(new {
                availstatus = catalogueItemDTO.StockAvailability.ToString(),
                lowstockcount = catalogueItemDTO.LowStockAvailability
            }, JsonRequestBehavior.AllowGet);
        }

        public void SubmitRequisitionForm(LoginDTO loginDTO)
        {

            //convert all cart items into requsitiondetails for this requisition
            Requisition newRequisition = requisitionCatalogueService.ConvertCartDetailsToRequisitionDetails(loginDTO.EmployeeId);

           //notify dept head for approval


           //return to catalogue/dashboard view
           ViewCatalogue(loginDTO); 
        }

        public ActionResult ViewRequisitionList(LoginDTO loginDTO)
        {
            //Get all requsition from this employee
            List<Requisition> requisitionHistory = requisitionCatalogueService.GetPersonalRequisitionHistory(loginDTO.EmployeeId);

            RequisitionsDTO model = new RequisitionsDTO() {LoginDTO = loginDTO, Requisitions = requisitionHistory };
            
            //viewData add additional data
            return View(model);
        }

        public ActionResult ViewRequisitionDetail(LoginDTO loginDTO, int requisitionId)
        {
            RequisitionDetailsDTO model = requisitionCatalogueService.GetRequisitionDetailsForPersonalRequisition(requisitionId, loginDTO.EmployeeId);
            model.LoginDTO = loginDTO;
            return View(model);
        }

        public ActionResult CancelPendingRequisition(LoginDTO loginDTO, int requisitionId)
        {
            return View();
        }

        public ActionResult CancelWaitlistedRequisitionDetail(LoginDTO loginDTO, int requisitionDetailId)
        {
            return View();
        }

        //Department head methods
        //approve/reject requested requisition
        //view requisition detail

    }
}