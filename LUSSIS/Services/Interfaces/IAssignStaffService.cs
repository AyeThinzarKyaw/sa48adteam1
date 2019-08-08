using LUSSIS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LUSSIS.Services.Interfaces
{
    public interface IAssignStaffService
    {
        Employee GetDeptRep(int eId);
        Employee GetStaff(int eId);
        IEnumerable<Employee> GetAllStaffAndRepInDept(int dId);
        void UpdateStaff(Employee e);
        void UpdateDeptRep(Employee e);
        IEnumerable<Employee> GetAllStaffAndCoverHeadInDept(int dId);
        IEnumerable<DepartmentCoverEmployee> GetCurrentDepartmentCoverEmployeesByDepartmentId(int departmentId);
        void CreateDepartmentCoverEmployee(DepartmentCoverEmployee cover);
        void CancelDepartmentCoverEmployee(DepartmentCoverEmployee cover);
        DepartmentCoverEmployee GetDepartmentCoverEmployeeById(int coverId);
        IEnumerable<DepartmentCoverEmployee> GetExistingDepartmentCoverEmployeesWithinDateRange(DateTime FromDate, DateTime ToDate);

    }
}
