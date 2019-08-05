using LUSSIS.Enums;
using LUSSIS.Models;
using LUSSIS.Models.DTOs;
using LUSSIS.Repositories;
using LUSSIS.Repositories.Interfaces;
using LUSSIS.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LUSSIS.Services
{
    public class AdjustmentVoucherService : IAdjustmentVoucherService
    {
        private IAdjustmentVoucherRepo adjustmentVoucherRepo;
        private IAdjustmentVoucherDetailRepo adjustmentVoucherDetailRepo;
        private static AdjustmentVoucherService instance = new AdjustmentVoucherService();

        private AdjustmentVoucherService()
        {
            adjustmentVoucherRepo = AdjustmentVoucherRepo.Instance;
            adjustmentVoucherDetailRepo = AdjustmentVoucherDetailRepo.Instance;
        }

        //returns single instance
        public static IAdjustmentVoucherService Instance
        {
            get { return instance; }
        }

        public void AutoAdjustmentsForRetrieval(int clerkEmployeeId, List<RetrievalItemDTO> retrievalList)
        {
            //any existing open adjustment voucher for this clerk?

            //could be null
            AdjustmentVoucher openAV = adjustmentVoucherRepo.FindOneBy(x=>x.EmployeeId == clerkEmployeeId && x.Status == "OPEN");

            foreach (var item in retrievalList)
            {
                if (item.NeededQuantity > item.RetrievedQty) //retrieval short of needed
                {
                    if (openAV == null) //no open AV for this clerk
                    {
                        //open new AV
                        openAV = new AdjustmentVoucher { Date = DateTime.Now, EmployeeId = clerkEmployeeId, Status = AdjustmentVoucherStatusEnum.OPEN.ToString() };

                        //persist in DB
                        openAV = adjustmentVoucherRepo.Create(openAV);

                    }
                    //create adjustment voucher detail                   
                    int adjustmentQty = item.RetrievedQty - item.NeededQuantity;
                    AdjustmentVoucherDetail newAVD = new AdjustmentVoucherDetail {AdjustmentVoucherId = openAV.Id, DateTime = DateTime.Now,
                        Quantity = adjustmentQty,
                        Reason = AdjustmentVoucherDefaultReasons.RETRIEVAL_SHORTAGE.ToString(),
                        StationeryId = item.StationeryId };
                    //persist in db
                    adjustmentVoucherDetailRepo.Create(newAVD);
                }
            }
        }
    }
}