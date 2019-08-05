using LUSSIS.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LUSSIS.Services.Interfaces
{
    public interface IAdjustmentVoucherService
    {
        void AutoAdjustmentsForRetrieval (int clerkEmployeeId, List<RetrievalItemDTO> retrievalList);

        //void AutoAdjustmentsForDisbursement(int clerkEmployeeId, List<> disbursementList);
    }
}
