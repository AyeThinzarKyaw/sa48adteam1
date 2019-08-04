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

        public List<Requisition> DepartmentRequisitionsEagerLoadEmployee(int deptId)
        {
            return Context.Requisitions.Include(r => r.Employee).Where(x => x.Employee.DepartmentId == deptId).ToList();

        }

        public List<Requisition> SchoolRequisitionsEagerLoadEmployeeIncDepartment()
        {
            return Context.Requisitions.Include(r => r.Employee.Department).ToList();
        }

        public Requisition OneRequisitionEagerLoadEmployeeIncDepartment(int requisitionId)
        {
            return Context.Requisitions
                .Where(r => r.Id == requisitionId)
                .Include(r => r.Employee.Department)
                .SingleOrDefault();
        }
    }
}