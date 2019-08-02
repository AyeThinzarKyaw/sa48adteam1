using LUSSIS.Models;
using LUSSIS.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace LUSSIS.Repositories
{
    public class RequisitionRepo : GenericRepo<Requisition, int> , IRequisitionRepo
    {
        private RequisitionRepo() { }

        private static RequisitionRepo instance = new RequisitionRepo();
        public static IRequisitionRepo Instance
        {
            get { return instance; }
        }

        public List<Requisition> RequisitionsEagerLoadEmployee(int employeeId)
        {
           return Context.Requisitions.Include(x => x.Employee).Where(x => x.EmployeeId == employeeId).ToList();             
        }

        public List<Requisition> DepartmentRequisitionsEagerLoadEmployee(int deptId)
        {
            var result = from r in Context.Requisitions
                         join e in Context.Employees on r.EmployeeId equals e.Id
                         join d in Context.Departments on e.DepartmentId equals d.Id
                         where d.Id == deptId
                         select r;
            return result.ToList(); 
        }

    }
}