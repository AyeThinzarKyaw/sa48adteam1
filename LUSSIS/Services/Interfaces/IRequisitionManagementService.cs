using LUSSIS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LUSSIS.Services.Interfaces
{
    public interface IRequisitionManagementService
    {
        List<Requisition> GetDepartmentRequisitions(int deptId);

        void ApproveRejectPendingRequisition(int requisitionId, string action, string remarks)
    }
}
