using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LUSSIS.Models;
using LUSSIS.Models.DTOs;

namespace LUSSIS.Services.Interfaces
{
    public interface IChartService
    {
        List<ChartDTO> TrendChartInfoForSupplier(int SupplierId, int CategoryId, int StationeryId);

        List<ChartDTO> TrendChartInfoForDepartment(int DepartmentId, int CategoryId, int StationeryId);
    }
}
