﻿using LUSSIS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LUSSIS.Repositories.Interfaces
{
    public interface IRequisitionDetailRepo : IGenericRepo<RequisitionDetail, int>
    {
        int GetReservedCountForStationery(int stationeryId);
    }
}
