﻿using LUSSIS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LUSSIS.Repositories.Interfaces
{
    public interface IStationeryRepo: IGenericRepo<Stationery, int>
    {
        IEnumerable<Stationery> GetStationeriesBySupplierAndYear(Supplier supplier, int year);
    }
}
