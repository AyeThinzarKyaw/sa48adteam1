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
        private IDepartmentRepo departmentRepo;
        private static RequisitionManagementService instance = new RequisitionManagementService();

        private RequisitionManagementService()
        {
            //stationeryRepo = StationeryRepo.Instance;
            requisitionRepo = RequisitionRepo.Instance;
            requisitionDetailRepo = RequisitionDetailRepo.Instance;
            employeeRepo = EmployeeRepo.Instance;
            departmentRepo = DepartmentRepo.Instance;

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
                        //change waitlist pending to waitlist approved
                        rd.Status = RequisitionDetailStatusEnum.WAITLIST_APPROVED.ToString();
                        requisitionDetailRepo.Update(rd);
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
    }
}