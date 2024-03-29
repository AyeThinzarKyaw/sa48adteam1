﻿using LUSSIS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LUSSIS.Repositories.Interfaces
{
    public interface IRequisitionDetailRepo : IGenericRepo<RequisitionDetail, int>
    {
        int GetReservedCountForStationery(int stationeryId);

        int GetRequisitionCountForUnfulfilledStationery(int requisitionDetailId);

        List<RequisitionDetail> RequisitionDetailsEagerLoadStationery(int requisitionId);

        List<RequisitionDetail> RequisitionDetailsEagerLoadStationeryIncCategory(int requisitionId);

        List<Requisition> GetUniqueRequisitionsForDisbursement(int disbursementId);

        List<IGrouping<int, RequisitionDetail>> GetUnfulfilledRequisitionDetailsGroupedByDept();

        List<RequisitionDetail> GetRequisitionDetailsByClerkDisbursementId(int EmployeeId);

        List<RequisitionDetail> GetRequisitionDetailsByDepRepDisbursementId(int EmployeeId);

        List<RequisitionDetail> GetRequisitionDetailsByDepartmentIdByCategoryIdByStationeryId(int DepartmentId, int CategoryId, int SstationeryId);
    }
}