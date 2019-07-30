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

    }
}