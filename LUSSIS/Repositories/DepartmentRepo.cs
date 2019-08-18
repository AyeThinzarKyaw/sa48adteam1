using LUSSIS.Models;
using LUSSIS.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LUSSIS.Repositories
{
    public class DepartmentRepo : GenericRepo<Department, int>, IDepartmentRepo
    {
        private DepartmentRepo() { }

        private static DepartmentRepo instance = new DepartmentRepo();

        public static IDepartmentRepo Instance
        {
            get { return instance; }
        }

        public Department GetDepartmentByEmployeeId(int employeeId)
        {
            var result = from d in Context.Departments
                         join e in Context.Employees on d.Id equals e.DepartmentId
                         where e.Id == employeeId
                         select d;
            return result.SingleOrDefault();
        }
    }
}