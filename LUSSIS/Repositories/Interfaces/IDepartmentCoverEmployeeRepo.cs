using LUSSIS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LUSSIS.Repositories.Interfaces
{
    public interface IDepartmentCoverEmployeeRepo : IGenericRepo<DepartmentCoverEmployee, int>
    {
        IEnumerable<DepartmentCoverEmployee> GetDepartmentCoverEmployeesByDepartmentId(int departmentId);

        DepartmentCoverEmployee GetDepartmentCoverEmployeeByDepartmentIdAndDate(int departmentId, DateTime date);

        IEnumerable<DepartmentCoverEmployee> GetCurrentDepartmentCoverEmployeesByDepartmentId(int departmentId, DateTime date);

        IEnumerable<DepartmentCoverEmployee> GetExistingDepartmentCoverEmployeesWithinDateRange(DateTime FromDate, DateTime ToDate);
    }
}