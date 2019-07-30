using LUSSIS.Models;
using LUSSIS.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LUSSIS.Repositories
{
    public class AdjustmentVoucherRepo : GenericRepo<AdjustmentVoucher, int> , IAdjustmentVoucherRepo
    {
        private AdjustmentVoucherRepo() { }

        private static AdjustmentVoucherRepo instance = new AdjustmentVoucherRepo();
        public static IAdjustmentVoucherRepo Instance
        {
            get { return instance; }
        }
    }
}