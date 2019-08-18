using LUSSIS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LUSSIS.Repositories.Interfaces
{
    public interface IDisbursementRepo : IGenericRepo<Disbursement, int>
    {
        IEnumerable<Disbursement> GetDisbursementsByDepartmentId(int depId);
        IEnumerable<Disbursement> GetDisbursementsByDeptRepId(int deptRepId);
        IEnumerable<Disbursement> GetDisbursementsByClerkId(int clerkId);
    }
}