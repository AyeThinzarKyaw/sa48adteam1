﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using LUSSIS.Models;

namespace LUSSIS.Services.Interfaces
{
    public interface IStationeryService
    {
        IEnumerable<Stationery> GetStationeriesBySupplierAndYear(Supplier supplier, int year);
    }
}