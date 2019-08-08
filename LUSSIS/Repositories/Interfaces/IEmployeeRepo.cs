using LUSSIS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LUSSIS.Repositories.Interfaces
{
    public interface IEmployeeRepo : IGenericRepo<Employee, int>
    {
        Employee GetCoverStaffByDepartmentIdAndDate(int depId, DateTime date);

        IEnumerable<Employee> GetAllClerksByCollectionPointId(int collectionPointId);

        IEnumerable<Employee> GetAllClerks();

        Employee GetClerkByCollectionPointId(int cpId);

        Employee GetDeptRepByDepartmentId(int dId);

        IEnumerable<Employee> GetAllStaffAndRepInDept(int dId);

        IEnumerable<Employee> GetAllStaffAndCoverHeadInDept(int dId);
    }
}
