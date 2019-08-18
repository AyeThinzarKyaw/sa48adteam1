using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LUSSIS.Models;
using LUSSIS.Models.DTOs;
using LUSSIS.Enums;
using LUSSIS.Services;
using LUSSIS.Services.Interfaces;
using LUSSIS.Filters;

namespace LUSSIS.Controllers
{
    public class StationeryController : Controller
    {
        // GET: Stationery List
        //By ATZK
        [Authorizer]
        public ActionResult Index()
        {
            if (Session["existinguser"] != null)
            {
                LoginDTO currentUser = (LoginDTO)Session["existinguser"];
                if (currentUser.RoleId != (int)Enums.Roles.StoreManager)
                {
                    return RedirectToAction("RedirectToClerkOrDepartmentView", "Login");
                }
                var stationeries = StationeryService.Instance.GetAllStationeries().OrderBy(x => x.Code).ToList();
                return View(stationeries);
            }
            return RedirectToAction("Index", "Login");
        }

        // GET: StationeryDetails
        //By ATZK
        [Authorizer]
        public ActionResult Detail(int stationeryId)
        {
            if (Session["existinguser"] != null)
            {
                LoginDTO currentUser = (LoginDTO)Session["existinguser"];
                if (currentUser.RoleId != (int)Enums.Roles.StoreManager)
                {
                    return RedirectToAction("RedirectToClerkOrDepartmentView", "Login");
                }
                Stationery stationery = StationeryService.Instance.GetStationeryById(stationeryId);
                return View(stationery);
            }
            return RedirectToAction("Index", "Login");
        }

        //CRETE stationery getMethod
        //By ATZK
        [Authorizer]
        public ActionResult Create()
        {
            if (Session["existinguser"] != null)
            {
                LoginDTO currentUser = (LoginDTO)Session["existinguser"];
                if (currentUser.RoleId != (int)Enums.Roles.StoreManager)
                {
                    return RedirectToAction("RedirectToClerkOrDepartmentView", "Login");
                }
                StationeryDetailsDTO stationery = new StationeryDetailsDTO();
                stationery.Categories = StationeryService.Instance.GetAllCategories();
                stationery.Suppliers = StationeryService.Instance.GetAllSuppliers();
                return View(stationery);
            }
            return RedirectToAction("Index", "Login");
        }

        //CRETE stationery postMethod 
        //By ATZK
        [Authorizer]
        [HttpPost]
        public ActionResult Create(StationeryDetailsDTO stationery)
        {
            if (Session["existinguser"] != null)
            {
                LoginDTO currentUser = (LoginDTO)Session["existinguser"];
                if (currentUser.RoleId != (int)Enums.Roles.StoreManager)
                {
                    return RedirectToAction("RedirectToClerkOrDepartmentView", "Login");
                }
                if (ModelState.IsValid)
                {
                    if (stationery.Supplier1 == stationery.Supplier2
                        || stationery.Supplier1 == stationery.Supplier3
                        || stationery.Supplier2 == stationery.Supplier3)
                    {
                        stationery.Error = new ErrorDTO();
                        stationery.Error.HasError = true;
                        stationery.Error.Message = "Suppliers must be three different suppliers!";
                        stationery.Categories = StationeryService.Instance.GetAllCategories();
                        stationery.Suppliers = StationeryService.Instance.GetAllSuppliers();
                        return View(stationery);
                    }
                    Stationery newStationery = this.generateStationery(stationery);
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
            return RedirectToAction("Index", "Login");
        }

        //By ATZK
        [Authorizer]
        private Stationery generateStationery(StationeryDetailsDTO stationery)
        {
           
            Stationery newStationery = StationeryService.Instance.GetStationeryById(stationery.StationeryId);
            if (newStationery == null) newStationery = new Stationery();
            newStationery.Id = stationery.StationeryId;
            newStationery.Code = stationery.Code;
            newStationery.Description = stationery.Description;
            newStationery.CategoryId = stationery.CategoryId;
            newStationery.UnitOfMeasure = Enum.GetName(typeof(Enums.UOM), stationery.UOM);
            newStationery.Bin = stationery.Bin;
            newStationery.Status = Enum.GetName(typeof(Enums.ActiveStatus), Enums.ActiveStatus.ACTIVE);
            return newStationery;
        }

        //By ATZK
        [Authorizer]
        private void generateSupplierTender(int supplierId, int rank, int stationeryId, decimal price)
        {
            SupplierTender supplierTender = new SupplierTender();
            supplierTender = SupplierTenderService.Instance.GetSupplierTendersOfCurrentYearByStationeryId(stationeryId).SingleOrDefault(s => s.Rank == rank);
            if (supplierTender == null)
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
        }

        //By ATZK
        [Authorizer]
        private StationeryDetailsDTO generateStationeryDetailsDTO(int stationeryId)
        {
            Stationery stationery = StationeryService.Instance.GetStationeryById(stationeryId);
            StationeryDetailsDTO stationeryDetails = new StationeryDetailsDTO();
            stationeryDetails.StationeryId = stationery.Id;
            stationeryDetails.Code = stationery.Code;
            stationeryDetails.Description = stationery.Description;
            stationeryDetails.Bin = stationery.Bin;
            stationeryDetails.CategoryId = stationery.CategoryId;
            stationeryDetails.Supplier1 = stationery.SupplierTenders.SingleOrDefault(x => x.Rank == 1).SupplierId;
            stationeryDetails.Supplier2 = stationery.SupplierTenders.SingleOrDefault(x => x.Rank == 2).SupplierId;
            stationeryDetails.Supplier3 = stationery.SupplierTenders.SingleOrDefault(x => x.Rank == 3).SupplierId;
            stationeryDetails.Price1 = stationery.SupplierTenders.SingleOrDefault(x => x.Rank == 1).Price;
            stationeryDetails.Price2 = stationery.SupplierTenders.SingleOrDefault(x => x.Rank == 2).Price;
            stationeryDetails.Price3 = stationery.SupplierTenders.SingleOrDefault(x => x.Rank == 3).Price;
            return stationeryDetails;
        }

        //UPDATE stationery getMethod
        //By ATZK
        [Authorizer]
        public ActionResult Update(int stationeryId)
        {
            if (Session["existinguser"] != null)
            {
                LoginDTO currentUser = (LoginDTO)Session["existinguser"];
                if (currentUser.RoleId != (int)Enums.Roles.StoreManager)
                {
                    return RedirectToAction("RedirectToClerkOrDepartmentView", "Login");
                }
                StationeryDetailsDTO stationeryDetails = this.generateStationeryDetailsDTO(stationeryId);
                stationeryDetails.Categories = StationeryService.Instance.GetAllCategories();
                stationeryDetails.Suppliers = StationeryService.Instance.GetAllSuppliers();
                return View(stationeryDetails);
            }
            return RedirectToAction("Index", "Login");
        }

        //UPDATE stationery postMethod 
        //By ATZK
        [Authorizer]
        [HttpPost]
        public ActionResult Update(StationeryDetailsDTO stationery)
        {
            if (Session["existinguser"] != null)
            {
                LoginDTO currentUser = (LoginDTO)Session["existinguser"];
                if (currentUser.RoleId != (int)Enums.Roles.StoreManager)
                {
                    return RedirectToAction("RedirectToClerkOrDepartmentView", "Login");
                }
                if (ModelState.IsValid)
                {
                    if (stationery.Supplier1 == stationery.Supplier2 || stationery.Supplier1 == stationery.Supplier3 || stationery.Supplier2 == stationery.Supplier3)
                    {
                        stationery.Error = new ErrorDTO();
                        stationery.Error.HasError = true;
                        stationery.Error.Message = "Suppliers must be three different suppliers!";
                        stationery.Categories = StationeryService.Instance.GetAllCategories();
                        stationery.Suppliers = StationeryService.Instance.GetAllSuppliers();
                        return View(stationery);
                    }
                    Stationery newStationery = this.generateStationery(stationery);
                    StationeryService.Instance.UpdateStationery(newStationery);
                    this.generateSupplierTender(stationery.Supplier1, 1, newStationery.Id, stationery.Price1);
                    this.generateSupplierTender(stationery.Supplier2, 2, newStationery.Id, stationery.Price2);
                    this.generateSupplierTender(stationery.Supplier3, 3, newStationery.Id, stationery.Price3);
                    return RedirectToAction("Index");
                }
                stationery.Categories = StationeryService.Instance.GetAllCategories();
                stationery.Suppliers = StationeryService.Instance.GetAllSuppliers();
                return View(stationery);
            }
            return RedirectToAction("Index", "Login");
        }

        //CREATE category postMethod call from ajax
        //By ATZK
        [Authorizer]
        public JsonResult CreateCategory(string type)
        {
            Category category = new Category();
            category.Type = type;
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
            return Json(bb, JsonRequestBehavior.AllowGet);
        }

        //change status from current status to other
        //By ATZK
        [Authorizer]
        public ActionResult ChangeStatus(int stationeryId)
        {
            if (Session["existinguser"] != null)
            {
                LoginDTO currentUser = (LoginDTO)Session["existinguser"];
                if (currentUser.RoleId != (int)Enums.Roles.StoreManager)
                {
                    return RedirectToAction("RedirectToClerkOrDepartmentView", "Login");
                }
                Stationery stationery = StationeryService.Instance.GetStationeryById(stationeryId);
                if (stationery.Status == Enum.GetName(typeof(Enums.ActiveStatus), Enums.ActiveStatus.ACTIVE))
                {
                    stationery.Status = Enum.GetName(typeof(Enums.ActiveStatus), Enums.ActiveStatus.INACTIVE);
                }
                else
                {
                    stationery.Status = Enum.GetName(typeof(Enums.ActiveStatus), Enums.ActiveStatus.ACTIVE);
                }
                StationeryService.Instance.UpdateStationery(stationery);
                return RedirectToAction("Index");
            }
            return RedirectToAction("Index", "Login");
        }
    }
}