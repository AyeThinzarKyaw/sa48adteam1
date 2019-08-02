using LUSSIS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LUSSIS.Services.Interfaces
{
    interface IEmailNotificationService
    {
        void NotifyDeptHeadToApprovePendingRequisition(Requisition newRequisition);
    }
}
