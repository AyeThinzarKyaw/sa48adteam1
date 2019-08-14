using LUSSIS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LUSSIS.Services.Interfaces
{
    public interface IEmailNotificationService
    {
        void NotifyDeptHeadToApprovePendingRequisition(Requisition newRequisition);
        void SendNotificationEmail(string receipient, string subject, string body, IEnumerable<string> attachments = null, string cc = null);
        void NotifyEmployeeApprovedOrRejectedRequisition(Requisition r, Employee e);
        void NotifyEmployeeCompletedRequisition(Requisition r, Employee e);
        void NotifyClerkShortFallInStationery(Stationery s, Employee clerk);

    }
}
