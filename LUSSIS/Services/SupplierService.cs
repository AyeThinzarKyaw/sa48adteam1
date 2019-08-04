using System;
using System.Collections.Generic;
using LUSSIS.Services.Interfaces;
using System.Linq;
using System.Web;
using LUSSIS.Models;
using LUSSIS.Repositories;

namespace LUSSIS.Services
{
    public class SupplierService : ISupplierService
    {
        private SupplierService() { }

        private static SupplierService instance = new SupplierService();
        public static ISupplierService Instance
        {
            get { return instance; }
        }

        public IEnumerable<Supplier> getAllSupplier()
        {
            return SupplierRepo.Instance.FindAll().ToList();
        }

        public Supplier getSupplierById(int poId)
        {
            return SupplierRepo.Instance.FindById(poId);
        }
        public void CreateSupplier(Supplier supplier)
        {
            SupplierRepo.Instance.Create(supplier);
        }
        public void UpdateSupplier(Supplier supplier)
        {
            SupplierRepo.Instance.Update(supplier);
        }
    }
}