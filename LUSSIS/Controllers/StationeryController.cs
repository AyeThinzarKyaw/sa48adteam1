using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LUSSIS.Models;
using LUSSIS.Models.DTOs;
using LUSSIS.Services;
using LUSSIS.Services.Interfaces;

namespace LUSSIS.Controllers
{
    public class StationeryController : Controller
    {
        // GET: Stationery List
        //By ATZK
        public ActionResult Index()
        {
            var stationeries = StationeryService.Instance.GetAllStationeries().OrderBy(x => x.Code).ToList();
            return View(stationeries);
        }

        // GET: StationeryDetails
        //By ATZK
        public ActionResult Detail(int stationeryId)
        {
            Stationery stationery= StationeryService.Instance.GetStationeryById(stationeryId);
            return View(stationery);
        }

        //CRETE stationery getMethod
        //By ATZK
        public ActionResult Create()
        {
            StationeryDetailsDTO stationery = new StationeryDetailsDTO();
            stationery.Categories = StationeryService.Instance.GetAllCategories();
            stationery.Suppliers = StationeryService.Instance.GetAllSuppliers();
            return View(stationery);
        }

        //CRETE stationery postMethod 
        //By ATZK
        [HttpPost]
        public ActionResult Create(StationeryDetailsDTO stationery)
        {
            if (ModelState.IsValid)
            {
                Stationery newStationery = new Stationery();
                newStationery.Id = stationery.StationeryId;
                newStationery.Code = stationery.Code;
                newStationery.Description = stationery.Description;
                newStationery.CategoryId = stationery.CategoryId;
                newStationery.UnitOfMeasure = Enum.GetName(typeof(EnumDTO.UOM), stationery.UOM);
                newStationery.Bin = stationery.Bin;
                newStationery.Status = Enum.GetName(typeof(EnumDTO.ActiveStatus), EnumDTO.ActiveStatus.ACTIVE);
                StationeryService.Instance.CreateStationery(newStationery);

                this.generateSupplierTender(stationery.Supplier1, 1, newStationery.Id, stationery.Price1);
                this.generateSupplierTender(stationery.Supplier2, 2, newStationery.Id, stationery.Price2);
                this.generateSupplierTender(stationery.Supplier3, 3, newStationery.Id, stationery.Price3);

                return RedirectToAction("Index");
            }
            stationery.Categories = StationeryService.Instance.GetAllCategories();
            stationery.Suppliers = StationeryService.Instance.GetAllSuppliers();
            return View(stationery);
        }

        private void generateSupplierTender(int supplierId, int rank, int stationeryId,decimal price)
        {
            SupplierTender supplierTender = new SupplierTender();
            
            supplierTender = SupplierTenderService.Instance.GetSupplierTendersOfCurrentYearByStationeryId(stationeryId).SingleOrDefault(s => s.Rank == rank);
            
            if(supplierTender==null)
            {
                supplierTender = new SupplierTender();
                supplierTender.StationeryId = stationeryId;
                supplierTender.Year = DateTime.Now.Year;
                supplierTender.SupplierId = supplierId;
                supplierTender.Rank = rank;
                supplierTender.Price = price;
                SupplierTenderService.Instance.CreateSupplierTender(supplierTender);
            }
            else
            {
                supplierTender.SupplierId = supplierId;
                supplierTender.Rank = rank;
                supplierTender.Price = price;
                SupplierTenderService.Instance.UpdateSupplierTender(supplierTender);
            }
            
            //return supplierTender;
            
           
        }
        //UPDATE stationery getMethod
        //By ATZK
        public ActionResult Update()
        {

            return View();
        }

        //UPDATE stationery postMethod 
        //By ATZK
        [HttpPost]
        public ActionResult Update(Stationery stationery)
        {
            return View();
        }


        //CREATE category postMethod call from ajax
        //By ATZK
        
        public JsonResult CreateCategory(string type)
        {
            Category category = new Category();
            category.Type = type;
            category.Id = 20;
            StationeryService.Instance.CreateCategory(category);
            var aa = StationeryService.Instance.GetAllCategories();

            var bb =
                from c in aa
                orderby c.Type
                select new
                {
                    Id = c.Id,
                    Type = c.Type
                };
            return Json( bb, JsonRequestBehavior.AllowGet);
        }

        //change status from current status to other
        //By ATZK
        public ActionResult ChangeStatus(int stationeryId)
        {
            Stationery stationery = StationeryService.Instance.GetStationeryById(stationeryId);
            if (stationery.Status== Enum.GetName(typeof(EnumDTO.ActiveStatus), EnumDTO.ActiveStatus.ACTIVE))
            {
                stationery.Status = Enum.GetName(typeof(EnumDTO.ActiveStatus), EnumDTO.ActiveStatus.INACTIVE);
            }
            else
            {
                stationery.Status = Enum.GetName(typeof(EnumDTO.ActiveStatus), EnumDTO.ActiveStatus.ACTIVE);
            }
            StationeryService.Instance.UpdateStationery(stationery);
            return RedirectToAction("Index");
        }

    }
}