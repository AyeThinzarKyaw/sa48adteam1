using LUSSIS.Models;
using LUSSIS.Repositories;
using LUSSIS.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LUSSIS.Services
{
    public class AssignStaffService : IAssignStaffService
    {
        private AssignStaffService() { }

        private static AssignStaffService instance = new AssignStaffService();
        public static IAssignStaffService Instance
        {
            get { return instance; }
        }

        public Employee GetDeptRep(int eId)
        {
            Employee employee = EmployeeRepo.Instance.FindById(eId);
            int dId = employee.DepartmentId;
            Employee deptrep = EmployeeRepo.Instance.GetDeptRepByDepartmentId(dId);

            return deptrep;
        }

        public Employee GetStaff(int eId)
        {
            return EmployeeRepo.Instance.FindById(eId);
        }


        public IEnumerable<Employee> GetAllStaffAndRepInDept(int dId)
        {
            return EmployeeRepo.Instance.GetAllStaffAndRepInDept(dId);
        }

        public void UpdateStaff(Employee e)
        {
            e.RoleId = (int)Enums.Roles.DepartmentStaff;
            EmployeeRepo.Instance.Update(e);
        }
        public void UpdateDeptRep(Employee e)
        {
            e.RoleId = (int)Enums.Roles.DepartmentRepresentative;
            EmployeeRepo.Instance.Update(e);
        }

        public IEnumerable<Employee> GetAllStaffAndCoverHeadInDept(int dId)
        {
            return EmployeeRepo.Instance.GetAllStaffAndCoverHeadInDept(dId);
        }

        public IEnumerable<DepartmentCoverEmployee> GetCurrentDepartmentCoverEmployeesByDepartmentId(int departmentId)
        {
            DateTime date = DateTime.Now;
            return DepartmentCoverEmployeeRepo.Instance.GetCurrentDepartmentCoverEmployeesByDepartmentId(departmentId, date);
        }

        public DepartmentCoverEmployee GetDepartmentCoverEmployeeById(int coverId)
        {
            return DepartmentCoverEmployeeRepo.Instance.FindById(coverId);
        }

        public void CreateDepartmentCoverEmployee(DepartmentCoverEmployee cover)
        {
            DepartmentCoverEmployeeRepo.Instance.Create(cover);
        }

        public void CancelDepartmentCoverEmployee(DepartmentCoverEmployee cover)
        {
            cover.Status = Enum.GetName(typeof(Enums.ActiveStatus), Enums.ActiveStatus.INACTIVE);
            DepartmentCoverEmployeeRepo.Instance.Update(cover);
        }

        public IEnumerable<DepartmentCoverEmployee> GetExistingDepartmentCoverEmployeesWithinDateRange(DateTime FromDate, DateTime ToDate)
        {
            return DepartmentCoverEmployeeRepo.Instance.GetExistingDepartmentCoverEmployeesWithinDateRange(FromDate, ToDate);


        }
    }
}