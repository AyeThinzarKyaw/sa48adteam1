using System;
using System.Collections.Generic;
using LUSSIS.Services.Interfaces;
using System.Linq;
using System.Web;
using LUSSIS.Models;
using LUSSIS.Repositories;

namespace LUSSIS.Services
{
    public class AdjustmentVoucherService : IAdjustmentVoucherService
    {
        private AdjustmentVoucherService() { }

        private static AdjustmentVoucherService instance = new AdjustmentVoucherService();
        public static IAdjustmentVoucherService Instance
        {
            get { return instance; }
        }

        public IEnumerable<AdjustmentVoucher> getAllAdjustmentVoucher()
        {
            return AdjustmentVoucherRepo.Instance.FindAll().ToList();
        }

        public AdjustmentVoucher getAdjustmentVoucherById(int adjId)
        {
            return AdjustmentVoucherRepo.Instance.FindById(adjId);
        }
        public void CreateAdjustmentVoucher(AdjustmentVoucher adj)
        {
            AdjustmentVoucherRepo.Instance.Create(adj);
        }
        public void UpdateAdjustmentVoucher(AdjustmentVoucher adj)
        {
            AdjustmentVoucherRepo.Instance.Update(adj);
        }
    }
}