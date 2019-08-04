using LUSSIS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LUSSIS.Repositories.Interfaces
{
    public interface IRequisitionRepo : IGenericRepo<Requisition, int>
    {
        List<Requisition> DepartmentRequisitionsEagerLoadEmployee(int deptHeadEmployeeId);

        List<Requisition> SchoolRequisitionsEagerLoadEmployeeIncDepartment();

        Requisition OneRequisitionEagerLoadEmployeeIncDepartment(int requisitionId);
    }
}
