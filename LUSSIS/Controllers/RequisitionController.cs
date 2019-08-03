﻿using LUSSIS.Models.DTOs;
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

        IRequisitionManagementService requisitionManagementService;

        public RequisitionController()
        {
            requisitionCatalogueService = RequisitionCatalogueService.Instance;
            requisitionManagementService = RequisitionManagementService.Instance;
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

    }
}