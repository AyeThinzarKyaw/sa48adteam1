using LUSSIS.Models;
using LUSSIS.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LUSSIS.Repositories
{
    public class DepartmentCoverEmployeeRepo : GenericRepo<DepartmentCoverEmployee, int>, IDepartmentCoverEmployeeRepo
    {
        private DepartmentCoverEmployeeRepo() { }

        private static DepartmentCoverEmployeeRepo instance = new DepartmentCoverEmployeeRepo();

        public static IDepartmentCoverEmployeeRepo Instance
        {
            get { return instance; }
        }

        public DepartmentCoverEmployee GetDepartmentCoverEmployeeByDepartmentIdAndDate(int departmentId, DateTime date)
        {
            var result = from d in Context.DepartmentCoverEmployees
                         join e in Context.Employees on d.EmployeeId equals e.Id
                         where e.DepartmentId == departmentId
                         where date >= d.FromDate
                         where date <= d.ToDate
                         select d;
            return result.SingleOrDefault<DepartmentCoverEmployee>();
        }

        public IEnumerable<DepartmentCoverEmployee> GetDepartmentCoverEmployeesByDepartmentId(int departmentId)
        {
            var result = from d in Context.DepartmentCoverEmployees
                         join e in Context.Employees on d.EmployeeId equals e.Id
                         where e.DepartmentId == departmentId
                         select d;
            return result.ToList();
        }

        public IEnumerable<DepartmentCoverEmployee> GetCurrentDepartmentCoverEmployeesByDepartmentId(int departmentId, DateTime date)
        {
            var result = from d in Context.DepartmentCoverEmployees
                         join e in Context.Employees on d.EmployeeId equals e.Id
                         where e.DepartmentId == departmentId
                         where date <= d.ToDate
                         where d.Status.Equals("ACTIVE")
                         select d;
            return result.ToList();
        }

        public IEnumerable<DepartmentCoverEmployee> GetExistingDepartmentCoverEmployeesWithinDateRange(DateTime From, DateTime To)
        {
            var result = from d in Context.DepartmentCoverEmployees
                         where (d.FromDate <= From && d.ToDate <= To && d.ToDate >= From) || (d.FromDate >= From && d.ToDate <= To) || (d.FromDate >= From && d.FromDate <= To && d.ToDate >= To || (From >= d.FromDate && To <= d.ToDate))
                         where d.Status.Equals("ACTIVE")
                         select d;
            return result.ToList();
        }
    }
}