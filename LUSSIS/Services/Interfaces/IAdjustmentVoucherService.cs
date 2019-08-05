using LUSSIS.Models;
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
        IEnumerable<AdjustmentVoucher> getAllAdjustmentVoucher();

        AdjustmentVoucher getAdjustmentVoucherById(int adjId);

        void CreateAdjustmentVoucher(AdjustmentVoucher adj);

        void UpdateAdjustmentVoucher(AdjustmentVoucher adj);


        void AutoAdjustmentsForRetrieval (int clerkEmployeeId, List<RetrievalItemDTO> retrievalList);

        //void AutoAdjustmentsForDisbursement(int clerkEmployeeId, List<> disbursementList);
    }
}
