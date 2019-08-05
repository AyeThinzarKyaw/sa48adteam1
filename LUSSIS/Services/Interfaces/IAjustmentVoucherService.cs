using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using LUSSIS.Models;

namespace LUSSIS.Services.Interfaces
{
    public interface IAdjustmentVoucherService
    {
        IEnumerable<AdjustmentVoucher> getAllAdjustmentVoucher();

        AdjustmentVoucher getAdjustmentVoucherById(int adjId);

        void CreateAdjustmentVoucher(AdjustmentVoucher adj);

        void UpdateAdjustmentVoucher(AdjustmentVoucher adj);
    }
}