using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LUSSIS.Models;

namespace LUSSIS.Repositories.Interfaces
{
    public interface ISupplierTenderRepo : IGenericRepo<SupplierTender,int>
    {
        IEnumerable<SupplierTender> GetSupplierTendersOfCurrentYearByStationeryId(int stationeryId);

        IEnumerable<SupplierTender> GetAllSupplierTendersOfCurrentYear();
    }
}