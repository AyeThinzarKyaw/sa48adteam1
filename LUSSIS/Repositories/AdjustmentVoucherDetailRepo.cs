using LUSSIS.Models;
using LUSSIS.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LUSSIS.Repositories
{
    public class AdjustmentVoucherDetailRepo : GenericRepo<AdjustmentVoucherDetail, int>, IAdjustmentVoucherDetailRepo
    {
        private AdjustmentVoucherDetailRepo() { }

        private static AdjustmentVoucherDetailRepo instance = new AdjustmentVoucherDetailRepo();

        public static IAdjustmentVoucherDetailRepo Instance
        {
            get { return instance; }
        }
    }
}