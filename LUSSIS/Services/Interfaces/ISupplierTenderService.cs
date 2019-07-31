using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LUSSIS.Models;

namespace LUSSIS.Services.Interfaces
{
    public interface ISupplierTenderService
    {
        IEnumerable<SupplierTender> GetSupplierTendersOfCurrentYearByStationeryId(int stationeryId);

        IEnumerable<SupplierTender> GetAllSupplierTendersOfCurrentYear();
        void CreateSupplierTender(SupplierTender supplierTender);
        void UpdateSupplierTender(SupplierTender supplierTender);
    }
}
