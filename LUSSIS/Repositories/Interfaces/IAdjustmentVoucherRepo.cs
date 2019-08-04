using LUSSIS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LUSSIS.Repositories.Interfaces
{
    public interface IAdjustmentVoucherRepo : IGenericRepo<AdjustmentVoucher, int>
    {
        int GetOpenAdjustmentVoucherCountForStationery(int stationeryId);

        List<int> getAdjustmentVoucherIdsWithAcknowledgedStatus();
    }
}
