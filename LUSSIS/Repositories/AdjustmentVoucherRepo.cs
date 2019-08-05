using LUSSIS.Models;
using LUSSIS.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LUSSIS.Repositories
{
    public class AdjustmentVoucherRepo : GenericRepo<AdjustmentVoucher, int>, IAdjustmentVoucherRepo
    {
        private AdjustmentVoucherRepo() { }

        private static AdjustmentVoucherRepo instance = new AdjustmentVoucherRepo();
        public static IAdjustmentVoucherRepo Instance
        {
            get { return instance; }
        }

        public int GetOpenAdjustmentVoucherCountForStationery(int stationeryId)
        {
            return (from av in Context.AdjustmentVouchers
                    join avd in Context.AdjustmentVoucherDetails on av.Id equals avd.AdjustmentVoucherId
                    where av.Status.Equals("Open") || av.Status.Equals("Pending")
                    where avd.StationeryId == stationeryId
                    select (int?)avd.Quantity).Sum() ?? 0;
        }

        public List<int> getAdjustmentVoucherIdsWithAcknowledgedStatus()
        {
            var adjustments = from a in Context.AdjustmentVouchers
                              where a.Status.Equals("test")
                              select a.Id;

            List<int> adjustmentsList = adjustments.ToList();
            return adjustmentsList;
        }

    }
}