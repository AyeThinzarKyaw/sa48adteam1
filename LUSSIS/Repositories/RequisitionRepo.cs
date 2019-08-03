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

    }
}