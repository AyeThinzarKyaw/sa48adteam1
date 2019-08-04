using LUSSIS.Models;
using LUSSIS.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LUSSIS.Services.Interfaces
{
    public interface IRequisitionCatalogueService
    {
        //Owed Items

        void CheckStockAndUpdateStatusForWaitlistApprovedRequisitionDetails(int PurchaseOrderId);

        //update QtyDelivered and change status
        //assign qties on fcfs basis
        void UpdateRequisitionDetailsAfterRetrieval(int qtyRetrieved, List<int> requisitionDetailIds);

        //update QtyDelivered and change status
        //assign qties on fcfs basis
        void UpdateRequisitionDetailsAfterDisbursement(int qtyCollected, List<int> requisitionDetailIds);

        void CheckRequisitionCompletenessAfterDisbursement(int disbursementId);

        List<CatalogueItemDTO> GetCatalogueItems(int employeeId);

        CatalogueItemDTO AddCartDetail(int employeeId, int stationeryId, int inputQty);

        CatalogueItemDTO RemoveCartDetail(int employeeId, int stationeryId);

        CatalogueItemDTO UpdateCartDetail(int employeeId, int stationeryId, int inputQty);

        List<Requisition> GetPersonalRequisitionHistory(int employeeId);

        RequisitionDetailsDTO GetRequisitionDetailsForSingleRequisition(int requisitionId, int employeeId);

        Requisition ConvertCartDetailsToRequisitionDetails(int employeeId);

        void CancelWaitlistedRequisitionDetail(int requisitionDetailId);

        void CancelPendingRequisition(int requisitionId);

        List<Requisition> GetSchoolRequisitionsWithEmployeeAndDept();

        RequisitionDetailsDTO GetRequisitionDetailsForClerk(int requisitionId);

    }
}
