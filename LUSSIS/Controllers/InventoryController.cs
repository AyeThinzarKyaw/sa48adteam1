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
    public class InventoryController : Controller
    {
        IInventoryService inventoryService;


        public InventoryController()
        {
            inventoryService = InventoryService.Instance;
        }


        [Authorizer]
        public ActionResult ViewInventoryList()
        {
            if (Session["existinguser"] != null)
            {
                LoginDTO currentUser = (LoginDTO)Session["existinguser"];
                if (currentUser.RoleId != (int)Enums.Roles.StoreClerk)
                {
                    return RedirectToAction("RedirectToClerkOrDepartmentView", "Login");
                }
                List<InventoryListDTO> inventoryList = inventoryService.RetrieveStationeryAndCategory();
                InventoryListRecordsDTO model = new InventoryListRecordsDTO { InventoryList = inventoryList };

                return View(model);
            }
            return RedirectToAction("Index", "Login");
        }

        [Authorizer]
        public ActionResult ViewStockCardAndSuppliers(int stationeryId)
        {
            if (Session["existinguser"] != null)
            {
                LoginDTO currentUser = (LoginDTO)Session["existinguser"];
                if (currentUser.RoleId != (int)Enums.Roles.StoreClerk)
                {
                    return RedirectToAction("RedirectToClerkOrDepartmentView", "Login");
                }
                StockAndSupplierDTO model = inventoryService.RetrieveStockMovement(stationeryId);
               
                return View(model);
            }
            return RedirectToAction("Index", "Login");
        }


    }
}