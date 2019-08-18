using LUSSIS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LUSSIS.Repositories.Interfaces
{
    public interface IStationeryRepo: IGenericRepo<Stationery, int>
    {
        IEnumerable<Stationery> GetStationeriesBySupplierIdAndYear(int supplierId, int year);

        List<Stationery> getAllStationeries();

        List<Stationery> getStationeriesByCategoryId(int catId);
    }
}
