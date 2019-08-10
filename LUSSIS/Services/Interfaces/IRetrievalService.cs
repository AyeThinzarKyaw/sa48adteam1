using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LUSSIS.Models;
using LUSSIS.Models.DTOs;

namespace LUSSIS.Services.Interfaces
{
    public interface IRetrievalService
    {
        RetrievalDTO constructRetrievalDTO(LoginDTO loginDTO);

        void completeRetrievalProcess(RetrievalDTO retrieval);

        void createNewDisbursementsFromUpdatedRequisitionDetails(List<RequisitionDetail> reqDetailList, int deliveredEmployeeId);

        List<RequisitionDetail> getRequisitionDetailListByPendingCollectionAndStationeryId(int stationeryId);

        void createNewAdjustmentVoucherDetail(AdjustmentVoucher adjustmentVoucher, int stationeryId, int quantity);

        AdjustmentVoucher retrieveNewOrAvailableAdjustmentVoucherForClerk(int deliveredEmployeeId);

        int retrieveNewOrAvailableDisbursementIdForDept(int deliveredEmployeeId, int receivedEmployeeId);

        List<Department> retrieveAllDepartmentsWithApprovedRequisitions();

        List<Requisition> retrieveAllApprovedRequisitionIdsByDepartmentName(string departmentName);

        AdHocRetrievalMenuDTO generateAdHocRetrievalMenuDTO();

        RetrievalDTO constructAdHocRetrievalDTO(LoginDTO loginDTO, int requisitionId);
    }
}
