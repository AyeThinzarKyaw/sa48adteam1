using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LUSSIS.Services;
using LUSSIS.Models;
using LUSSIS.Models.DTOs;
using LUSSIS.Filters;

namespace LUSSIS.Controllers
{
    public class SupplierController : Controller
    {
        // GET: SupplierList
        //By NESS
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
                var suppliers = SupplierService.Instance.getAllSupplier().ToList();
                return View(suppliers);
            }
            return RedirectToAction("Index", "Login");
        }

        // GET: SupplierDetails
        //By NESS
        [Authorizer]
        public ActionResult Detail(int supplierId)
        {
            if (Session["existinguser"] != null)
            {
                LoginDTO currentUser = (LoginDTO)Session["existinguser"];
                if (currentUser.RoleId != (int)Enums.Roles.StoreManager)
                {
                    return RedirectToAction("RedirectToClerkOrDepartmentView", "Login");
                }
                Supplier supplier = SupplierService.Instance.getSupplierById(supplierId);
                return View(supplier);
            }
            return RedirectToAction("Index", "Login");
        }

        //CRETE Supplier getMethod
        //By NESS
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
                SupplierDetailsDTO supplier = new SupplierDetailsDTO();
                return View(supplier);
            }
            return RedirectToAction("Index", "Login");
        }

        //CRETE Supplier postMethod 
        //By NESS
        [Authorizer]
        [HttpPost]
        public ActionResult Create(SupplierDetailsDTO supplier)
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
                    Supplier newSupplier = this.generateSupplier(supplier);
                    SupplierService.Instance.CreateSupplier(newSupplier);

                    return RedirectToAction("Index");
                }
                return View(supplier);
            }
            return RedirectToAction("Index", "Login");
        }

        //UPDATE Supplier getMethod
        //By NESS
        [Authorizer]
        public ActionResult Update(int supplierId)
        {
            if (Session["existinguser"] != null)
            {
                LoginDTO currentUser = (LoginDTO)Session["existinguser"];
                if (currentUser.RoleId != (int)Enums.Roles.StoreManager)
                {
                    return RedirectToAction("RedirectToClerkOrDepartmentView", "Login");
                }
                SupplierDetailsDTO supplierDetails = this.generateSupplierDetailsDTO(supplierId);

                return View(supplierDetails);
            }
            return RedirectToAction("Index", "Login");
        }

        //UPDATE Supplier postMethod 
        //By NESS
        [Authorizer]
        [HttpPost]
        public ActionResult Update(SupplierDetailsDTO supplier)
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

                    Supplier newSupplier = this.generateSupplier(supplier);
                    SupplierService.Instance.UpdateSupplier(newSupplier);
                    return RedirectToAction("Index");
                }

                return View(supplier);
            }
            return RedirectToAction("Index", "Login");
        }

        //By NESS
        [Authorizer]
        private Supplier generateSupplier(SupplierDetailsDTO supplier)
        {
            Supplier newSupplier = SupplierService.Instance.getSupplierById(supplier.SupplierId);
            if (newSupplier == null) newSupplier = new Supplier();
            newSupplier.Id = supplier.SupplierId;
            newSupplier.Code = supplier.Code;
            newSupplier.ContactName = supplier.ContactName;
            newSupplier.FaxNo = supplier.FaxNo;
            newSupplier.Name = supplier.Name;
            newSupplier.PhoneNo = supplier.PhoneNo;
            newSupplier.GST_No = supplier.GST_No;
            newSupplier.Address1 = supplier.Address1;
            newSupplier.Address2 = supplier.Address2;
            newSupplier.Address3 = supplier.Address3;
            return newSupplier;
        }

        //By NESS
        [Authorizer]
        private SupplierDetailsDTO generateSupplierDetailsDTO(int supplierId)
        {
            Supplier supplier = SupplierService.Instance.getSupplierById(supplierId);

            SupplierDetailsDTO supplierDetails = new SupplierDetailsDTO();
            supplierDetails.SupplierId = supplier.Id;
            supplierDetails.Code = supplier.Code;
            supplierDetails.ContactName = supplier.ContactName;
            supplierDetails.FaxNo = supplier.FaxNo;
            supplierDetails.Name = supplier.Name;
            supplierDetails.PhoneNo = supplier.PhoneNo;
            supplierDetails.GST_No = supplier.GST_No;
            supplierDetails.Address1 = supplier.Address1;
            supplierDetails.Address2 = supplier.Address2;
            supplierDetails.Address3 = supplier.Address3;

            return supplierDetails;
        }

        //change status from active to inactive
        //By NESS
        [Authorizer]
        public ActionResult ChangeStatus(int supplierId)
        {
            if (Session["existinguser"] != null)
            {
                LoginDTO currentUser = (LoginDTO)Session["existinguser"];
                if (currentUser.RoleId != (int)Enums.Roles.StoreManager)
                {
                    return RedirectToAction("RedirectToClerkOrDepartmentView", "Login");
                }
                Supplier supplier = SupplierService.Instance.getSupplierById(supplierId);
                if (supplier.Active == Convert.ToBoolean(Enums.ActiveStatus.ACTIVE))
                {
                    supplier.Active = Convert.ToBoolean(Enums.ActiveStatus.INACTIVE);
                }
                else
                {
                    supplier.Active = Convert.ToBoolean(Enums.ActiveStatus.ACTIVE);
                }
                SupplierService.Instance.UpdateSupplier(supplier);
                return RedirectToAction("Index");
            }
            return RedirectToAction("Index", "Login");
        }
    }
}