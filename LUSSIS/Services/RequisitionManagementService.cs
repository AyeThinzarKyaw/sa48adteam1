
using LUSSIS.Enums;
using LUSSIS.Models;
using LUSSIS.Repositories;
using LUSSIS.Repositories.Interfaces;
using LUSSIS.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LUSSIS.Services
{
    public class RequisitionManagementService: IRequisitionManagementService
    {
        //private IStationeryRepo stationeryRepo;
        private IRequisitionRepo requisitionRepo;
        private IRequisitionDetailRepo requisitionDetailRepo;
        private IEmployeeRepo employeeRepo;
        private IAdjustmentVoucherRepo adjustmentVoucherRepo;
        private IStationeryRepo stationeryRepo;
        private IPurchaseOrderRepo purchaseOrderRepo;
        private IPurchaseOrderDetailRepo purchaseOrderDetailRepo;
        private IEmailNotificationService emailNotificationService;
        private static RequisitionManagementService instance = new RequisitionManagementService();

        private RequisitionManagementService()
        {
            requisitionRepo = RequisitionRepo.Instance;
            requisitionDetailRepo = RequisitionDetailRepo.Instance;
            employeeRepo = EmployeeRepo.Instance;
            adjustmentVoucherRepo = AdjustmentVoucherRepo.Instance;
            stationeryRepo = StationeryRepo.Instance;
            purchaseOrderRepo = PurchaseOrderRepo.Instance;
            purchaseOrderDetailRepo = PurchaseOrderDetailRepo.Instance;
            emailNotificationService = EmailNotificationService.Instance;
        }

        //returns single instance
        public static IRequisitionManagementService Instance
        {
            get { return instance; }
        }

        public List<Requisition> GetDepartmentRequisitions(int deptHeadEmployeeId)
        {
            //get dept Id
            Employee deptHead = employeeRepo.FindById(deptHeadEmployeeId);

            return requisitionRepo.DepartmentRequisitionsEagerLoadEmployee(deptHead.DepartmentId);
        }

        public List<Requisition> GetPendingDepartmentRequisitions(int deptHeadEmployeeId)
        {
            //get dept Id
            Employee deptHead = employeeRepo.FindById(deptHeadEmployeeId);

            return requisitionRepo.DepartmentPendingRequisitionsEagerLoadEmployee(deptHead.DepartmentId);
        }

        public void ApproveRejectPendingRequisition(int requisitionId, string action, string remarks)
        {
            Requisition r = requisitionRepo.FindById(requisitionId);
            if(remarks != null)
            {
                r.Remarks = remarks;
            }

            if (action.Equals("approve"))
            {
                r.Status = RequisitionStatusEnum.APPROVED.ToString();
                requisitionRepo.Update(r);
                CascadeToRequisitionDetails(action, requisitionId);
                
            }
            else
            {
                r.Status = RequisitionStatusEnum.REJECTED.ToString();
                requisitionRepo.Update(r);
                CascadeToRequisitionDetails(action, requisitionId);

            }

            emailNotificationService.NotifyEmployeeApprovedOrRejectedRequisition(r, r.Employee);
        }

        private void CascadeToRequisitionDetails(string action, int requisitionId)
        {
            List<RequisitionDetail> rds = (List<RequisitionDetail>)requisitionDetailRepo.FindBy(x => x.RequisitionId == requisitionId);
            
            if (action.Equals("approve"))
            {
                foreach (RequisitionDetail rd in rds)
                {
                    if (rd.Status.Equals(RequisitionDetailStatusEnum.RESERVED_PENDING.ToString()))
                    {
                        //change from reserved pending to preparing
                        rd.Status = RequisitionDetailStatusEnum.PREPARING.ToString();
                        requisitionDetailRepo.Update(rd);
                    }
                    else if(rd.Status.Equals(RequisitionDetailStatusEnum.WAITLIST_PENDING.ToString()))
                    {
                        //any stock at present for them?
                        //int availStock = GetAvailableStockForWaitlistApprovedItems(rd.StationeryId);

                        //change waitlist pending to waitlist approved
                        rd.Status = RequisitionDetailStatusEnum.WAITLIST_APPROVED.ToString();
                        requisitionDetailRepo.Update(rd);

                        NotifyClerkAboutAnyShortFallInWaitlistApprovedStationery(rd.StationeryId, (int)rd.Requisition.Employee.Department.CollectionPoint.EmployeeId);
                    }
                    else
                    {
                        //throw new Exception("This RD shouldnt belong to a pending Requsition!");
                    }
                    
                }
               
            }
            else
            {
                //rejected
                foreach (RequisitionDetail rd in rds)
                {
                    rd.Status = RequisitionDetailStatusEnum.REJECTED.ToString();
                    requisitionDetailRepo.Update(rd);
                }
            }
        }

        private void NotifyClerkAboutAnyShortFallInWaitlistApprovedStationery(int stationeryId, int clerkEmployeeId)
        {
            if (AnyShortFallInWaitlistApprovedStationery(stationeryId))
            {
                //email clerk
                Stationery s = stationeryRepo.FindById(stationeryId);
                Employee clerk = employeeRepo.FindById(clerkEmployeeId);
                emailNotificationService.NotifyClerkShortFallInStationery(s, clerk);
            }
        }

        private bool AnyShortFallInWaitlistApprovedStationery(int stationeryId)
        {
            int waitlistApprovedCount = requisitionDetailRepo.FindBy(x=>x.StationeryId == stationeryId && x.Status.Equals("WAITLIST_APPROVED")).Sum(x=>x.QuantityOrdered);

            List<PurchaseOrder> pendingPOs = (List<PurchaseOrder>)purchaseOrderRepo.FindBy(x=> x.Status.Equals("PENDING") || x.Status.Equals("APPROVED"));
            int sumIncomingStationery = 0;

            if(pendingPOs.Count != 0)
            {
                foreach(PurchaseOrder po in pendingPOs)
                {
                    //get podetails with the same stationeryid
                    PurchaseOrderDetail pod = purchaseOrderDetailRepo.FindOneBy(x => x.StationeryId == stationeryId);
                    if ( pod != null)
                    {
                        sumIncomingStationery += pod.QuantityOrdered;
                    }
                }
            }

            if(sumIncomingStationery < waitlistApprovedCount)
            {
                return true;
            }
            return false;
        }

        private int GetAvailableStockForWaitlistApprovedItems(int stationeryId)
        {
            int reqInTransitCount = requisitionDetailRepo.GetRequisitionCountForUnfulfilledStationery(stationeryId);

            int openAdjustmentCount = adjustmentVoucherRepo.GetOpenAdjustmentVoucherCountForStationery(stationeryId);

            int totalCount = stationeryRepo.FindById(stationeryId).Quantity;

            return totalCount + openAdjustmentCount - reqInTransitCount;
        }
    }
}