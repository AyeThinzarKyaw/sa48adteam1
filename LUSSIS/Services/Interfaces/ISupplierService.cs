using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using LUSSIS.Models;

namespace LUSSIS.Services.Interfaces
{
    public interface ISupplierService
    {
        IEnumerable<Supplier> getAllSupplier();

        Supplier getSupplierById(int sId);

        void CreateSupplier(Supplier supplier);

        void UpdateSupplier(Supplier supplier);
    }
}