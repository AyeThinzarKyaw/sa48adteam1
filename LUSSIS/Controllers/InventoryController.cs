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

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ViewInventoryList(LoginDTO loginDTO)
        {
            List<InventoryListDTO> inventoryList = inventoryService.RetrieveStationeryAndCategory();
            InventoryListRecordsDTO model = new InventoryListRecordsDTO { InventoryList = inventoryList, LoginDTO = loginDTO };

            return View(model);
        }


    }
}