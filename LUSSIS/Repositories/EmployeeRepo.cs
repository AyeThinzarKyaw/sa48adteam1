using LUSSIS.Models;
using LUSSIS.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;

namespace LUSSIS.Repositories
{
    public class EmployeeRepo : GenericRepo<Employee, int>, IEmployeeRepo
    {
        private EmployeeRepo() { }

        private static EmployeeRepo instance = new EmployeeRepo();

        public static IEmployeeRepo Instance
        {
            get { return instance; }
        }

        public IEnumerable<Employee> GetAllClerksByCollectionPointId(int collectionPointId)
        {
            var result = from e in Context.Employees
                         join cp in Context.CollectionPoints on e.Id equals cp.EmployeeId
                         where e.RoleId == 7
                         where cp.Id == collectionPointId
                         select e;
            return result.ToList();
        }

        public Employee GetCoverStaffByDepartmentIdAndDate(int depId, DateTime date)
        {
            var result = from e in Context.Employees
                         join c in Context.DepartmentCoverEmployees on e.Id equals c.EmployeeId
                         where e.DepartmentId == depId
                         where date >= c.FromDate
                         where date <= c.ToDate
                         select e;
            return result.SingleOrDefault<Employee>();
        }

        public IEnumerable<Employee> GetAllClerks()
        {
            var result = from e in Context.Employees
                         where e.RoleId == 7
                         select e;
            return result.ToList();
        }

        public IEnumerable<Employee> GetAllStaffAndRepInDept(int dId)
        {
            var result = from e in Context.Employees
                         join d in Context.Departments on e.DepartmentId equals d.Id
                         where d.Id == dId
                         where e.RoleId == 2 || e.RoleId == 3
                         select e;
            return result.ToList();
        }

        public Employee GetClerkByCollectionPointId(int cpId)
        {
            var result = from e in Context.Employees
                         join cp in Context.CollectionPoints on e.Id equals cp.EmployeeId
                         where e.RoleId == 7
                         where cp.Id == cpId
                         select e;
            return result.SingleOrDefault();
        }

        public Employee GetDeptRepByDepartmentId(int dId)
        {
            var result = from e in Context.Employees
                         where e.RoleId == 3
                         where e.DepartmentId == dId
                         select e;
            return result.SingleOrDefault();
        }

        public IEnumerable<Employee> GetAllStaffAndCoverHeadInDept(int dId)
        {
            var result = from e in Context.Employees
                         where e.DepartmentId == dId
                         where e.RoleId == 2 || e.RoleId == 4
                         select e;
            return result.ToList();
        }
    }
}