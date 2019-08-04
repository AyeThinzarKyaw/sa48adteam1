using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LUSSIS.Services;
using LUSSIS.Models;
using LUSSIS.Models.DTOs;

namespace LUSSIS.Controllers
{
    public class SupplierController : Controller
    {
        // GET: SupplierList
        //By NESS
        public ActionResult Index()
        {
            var suppliers = SupplierService.Instance.getAllSupplier().ToList();
            return View(suppliers);
        }

        // GET: SupplierDetails
        //By NESS
        public ActionResult Detail(int supplierId)
        {
            Supplier supplier = SupplierService.Instance.getSupplierById(supplierId);
            return View(supplier);
        }

        //CRETE Supplier getMethod
        //By NESS
        public ActionResult Create()
        {
            SupplierDetailsDTO supplier = new SupplierDetailsDTO();
            return View(supplier);
        }

        //CRETE Supplier postMethod 
        //By NESS
        [HttpPost]
        public ActionResult Create(SupplierDetailsDTO supplier)
        {
            if (ModelState.IsValid)
            {
                Supplier newSupplier = this.generateSupplier(supplier);
                SupplierService.Instance.CreateSupplier(newSupplier);

                return RedirectToAction("Index");
            }
            return View(supplier);
        }

        //UPDATE Supplier getMethod
        //By NESS
        public ActionResult Update(int supplierId)
        {
            SupplierDetailsDTO supplierDetails = this.generateSupplierDetailsDTO(supplierId);
            
            return View(supplierDetails);
        }

        //UPDATE Supplier postMethod 
        //By NESS
        [HttpPost]
        public ActionResult Update(SupplierDetailsDTO supplier)
        {
            if (ModelState.IsValid)
            {

                Supplier newSupplier = this.generateSupplier(supplier);
                SupplierService.Instance.UpdateSupplier(newSupplier);
                return RedirectToAction("Index");
            }

            return View(supplier);
        }

        //By NESS
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
        public ActionResult ChangeStatus(int supplierId)
        {
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
    }
}