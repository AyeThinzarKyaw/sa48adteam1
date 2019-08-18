using LUSSIS.Models;
using LUSSIS.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LUSSIS.Repositories
{
    public class SupplierRepo : GenericRepo<Supplier, int>, ISupplierRepo
    {
        private SupplierRepo() { }

        private static SupplierRepo instance = new SupplierRepo();

        public static ISupplierRepo Instance
        {
            get { return instance; }
        }
    }
}