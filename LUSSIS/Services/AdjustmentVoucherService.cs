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
        private IStationeryRepo stationeryRepo;
        private static AdjustmentVoucherService instance = new AdjustmentVoucherService();

        private AdjustmentVoucherService()
        {
            adjustmentVoucherRepo = AdjustmentVoucherRepo.Instance;
            adjustmentVoucherDetailRepo = AdjustmentVoucherDetailRepo.Instance;
            stationeryRepo = StationeryRepo.Instance;
        }


        //returns single instance
        public static IAdjustmentVoucherService Instance
        {
            get { return instance; }
        }

        public IEnumerable<AdjustmentVoucher> getAllAdjustmentVoucher()
        {
            return adjustmentVoucherRepo.FindAll().ToList();
        }

        public AdjustmentVoucher getAdjustmentVoucherById(int adjId)
        {
            return adjustmentVoucherRepo.FindById(adjId);
        }

        public AdjustmentVoucherDetail getAdjustmentVoucherDetailById(int adjdId)
        {
            return adjustmentVoucherDetailRepo.FindById(adjdId);
        }

        public AdjustmentVoucher getOpenAdjustmentVoucherByClerk(int clerkId)
        {
            return adjustmentVoucherRepo.FindOneBy(x => x.Status == "Open" && x.EmployeeId == clerkId);
        }

        public void CreateAdjustmentVoucher(AdjustmentVoucher adj)
        {
            adjustmentVoucherRepo.Create(adj);
        }
        public void UpdateAdjustmentVoucher(AdjustmentVoucher adj)
        {
            if(adj.Status.Equals("Submitted"))
            {
                foreach(AdjustmentVoucherDetail avd in adj.AdjustmentVoucherDetails)
                {
                    int sId = avd.StationeryId;
                    Stationery s = stationeryRepo.FindById(sId);
                    s.Quantity += avd.Quantity;
                    stationeryRepo.Update(s);
                }
            }
            adjustmentVoucherRepo.Update(adj);
        }

        public void CreateAdjustmentVoucherDetail(AdjustmentVoucherDetail adjdetail)
        {
            adjustmentVoucherDetailRepo.Create(adjdetail);
        }
        public void UpdateAdjustmentVoucherDetail(AdjustmentVoucherDetail adjdetail)
        {
            adjustmentVoucherDetailRepo.Update(adjdetail);
        }

        public void DeleteAdjustmentVoucherDetail(AdjustmentVoucherDetail adjdetail)
        {
            adjustmentVoucherDetailRepo.Delete(adjdetail);
        }

        public List<AdjustmentVoucherDTO> getTotalAmountDTO()
        {
            List<AdjustmentVoucherDTO> voucherDTO = new List<AdjustmentVoucherDTO>();
            IEnumerable<AdjustmentVoucher> adjs = adjustmentVoucherRepo.FindAll();

            foreach (var adj in adjs)
            {
                AdjustmentVoucherDTO newVoucher = new AdjustmentVoucherDTO();
                newVoucher.adjustmentVoucher = adj;
                if(adj.AdjustmentVoucherDetails.Count > 0)
                {
                    var total = adjustmentVoucherRepo.GetTotalAmount(adj.Id);
                    if(total < 0 )
                    {
                        total = total * -1;
                    }
                    newVoucher.TotalAmount = total;
                }
                else
                {
                    newVoucher.TotalAmount = 0;
                }
                voucherDTO.Add(newVoucher);
            }
            return voucherDTO;
        }

        public void AutoAdjustmentsForRetrieval(int clerkEmployeeId, List<RetrievalItemDTO> retrievalList)
        {
            //any existing open adjustment voucher for this clerk?

            //could be null
            AdjustmentVoucher openAV = adjustmentVoucherRepo.FindOneBy(x=>x.EmployeeId == clerkEmployeeId && x.Status == "Open");

            foreach (var item in retrievalList)
            {
                if (item.NeededQuantity > item.RetrievedQty) //retrieval short of needed
                {
                    if (openAV == null) //no open AV for this clerk
                    {
                        //open new AV
                        openAV = new AdjustmentVoucher { Date = DateTime.Now, EmployeeId = clerkEmployeeId, Status = AdjustmentVoucherStatus.Open.ToString() };

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