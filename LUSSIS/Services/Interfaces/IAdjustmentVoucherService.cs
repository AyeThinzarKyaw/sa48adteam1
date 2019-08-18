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

        AdjustmentVoucherDetail getAdjustmentVoucherDetailById(int adjdId);

        AdjustmentVoucher getOpenAdjustmentVoucherByClerk(int clerkId);

        void CreateAdjustmentVoucher(AdjustmentVoucher adj);

        void CreateAdjustmentVoucherDetail(AdjustmentVoucherDetail adjdetail);

        void UpdateAdjustmentVoucherDetail(AdjustmentVoucherDetail adjdetail);

        void DeleteAdjustmentVoucherDetail(AdjustmentVoucherDetail adjdetail);

        void UpdateAdjustmentVoucher(AdjustmentVoucher adj);

        List<AdjustmentVoucherDTO> getTotalAmountDTO();

        void AutoAdjustmentsForRetrieval (int clerkEmployeeId, List<RetrievalItemDTO> retrievalList);
    }
}