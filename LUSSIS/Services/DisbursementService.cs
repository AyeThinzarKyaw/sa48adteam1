using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using LUSSIS.Models;
using LUSSIS.Services.Interfaces;
using LUSSIS.Repositories;
using LUSSIS.Repositories.Interfaces;
using LUSSIS.Models.DTOs;
using LUSSIS.Enums;

namespace LUSSIS.Services
{
    public class DisbursementService : IDisbursementService
    {
        private IDisbursementRepo disbursementRepo;
        private IStationeryRepo stationeryRepo;
        private ICategoryRepo categoryRepo;
        private ISupplierTenderRepo supplierTenderRepo;
        private ISupplierRepo supplierRepo;
        private IAdjustmentVoucherRepo adjustmentVoucherRepo;
        private IAdjustmentVoucherDetailRepo adjustmentVoucherDetailRepo;
        private IRequisitionRepo requisitionRepo;
        private IRequisitionDetailRepo requisitionDetailRepo;
        private IEmployeeRepo employeeRepo;
        private IDepartmentRepo departmentRepo;
        private IPurchaseOrderRepo purchaseOrderRepo;
        private IPurchaseOrderDetailRepo purchaseOrderDetailRepo;
        public DisbursementService()
        {
            stationeryRepo = StationeryRepo.Instance;
            categoryRepo = CategoryRepo.Instance;
            supplierTenderRepo = SupplierTenderRepo.Instance;
            supplierRepo = SupplierRepo.Instance;
            adjustmentVoucherRepo = AdjustmentVoucherRepo.Instance;
            adjustmentVoucherDetailRepo = AdjustmentVoucherDetailRepo.Instance;
            requisitionRepo = RequisitionRepo.Instance;
            requisitionDetailRepo = RequisitionDetailRepo.Instance;
            disbursementRepo = DisbursementRepo.Instance;
            employeeRepo = EmployeeRepo.Instance;
            departmentRepo = DepartmentRepo.Instance;
            purchaseOrderRepo = PurchaseOrderRepo.Instance;
            purchaseOrderDetailRepo = PurchaseOrderDetailRepo.Instance;
        }
        private static DisbursementService instance = new DisbursementService();

        public static IDisbursementService Instance
        {
            get { return instance; }
        }



        public List<DisbursementListDTO> GetDepRepDisbursementsDetails(int EmployeeId)
        {
            List<RequisitionDetail> RequisitionDetailsList = requisitionDetailRepo.GetRequisitionDetailsByDepRepDisbursementId(EmployeeId);
            List<DisbursementListDTO> DisplayDisbursementDetailsList = new List<DisbursementListDTO>();
            foreach (RequisitionDetail rd in RequisitionDetailsList)
            {
                DisbursementListDTO disbursementDTO = new DisbursementListDTO()
                {
                    Id = rd.Id,
                    DisbursementId = (int)rd.DisbursementId,
                    ReceivedEmployeeId = (int)disbursementRepo.FindById((int)rd.DisbursementId).ReceivedEmployeeId,
                    DeliveredEmployeeId = disbursementRepo.FindById((int)rd.DisbursementId).DeliveredEmployeeId,
                    DeliveryDateTime = (DateTime)disbursementRepo.FindById((int)rd.DisbursementId).DeliveryDateTime,
                    CollectionPoint = disbursementRepo.FindById((int)rd.DisbursementId).CollectionPoint,
                    QuantityOrdered = rd.QuantityOrdered,
                    QuantityDelivered = (int)rd.QuantityDelivered,
                    StationeryId = stationeryRepo.FindById(rd.StationeryId).Id,
                    Status = rd.Status,
                    UnitOfMeasure = stationeryRepo.FindById(rd.StationeryId).UnitOfMeasure,
                    EmployeeId = employeeRepo.FindById((int)disbursementRepo.FindById((int)rd.DisbursementId).ReceivedEmployeeId).Id,
                    EmployeeName = employeeRepo.FindById(requisitionRepo.FindById(rd.RequisitionId).EmployeeId).Name,
                    DepartmentName = departmentRepo.FindById(employeeRepo.FindById((int)disbursementRepo.FindById((int)rd.DisbursementId).ReceivedEmployeeId).DepartmentId).DepartmentName,
                    DepartmentId = departmentRepo.FindById(employeeRepo.FindById((int)disbursementRepo.FindById((int)rd.DisbursementId).ReceivedEmployeeId).DepartmentId).Id,
                    DepartmentCode = departmentRepo.FindById(employeeRepo.FindById((int)disbursementRepo.FindById((int)rd.DisbursementId).ReceivedEmployeeId).DepartmentId).DepartmentCode,
                    DepartmentContactName = departmentRepo.FindById(employeeRepo.FindById((int)disbursementRepo.FindById((int)rd.DisbursementId).ReceivedEmployeeId).DepartmentId).ContactName,
                    DepartmentContactNumber = departmentRepo.FindById(employeeRepo.FindById((int)disbursementRepo.FindById((int)rd.DisbursementId).ReceivedEmployeeId).DepartmentId).TelephoneNo,
                    Category = categoryRepo.FindById(stationeryRepo.FindById(rd.StationeryId).CategoryId).Type,
                    ItemName = stationeryRepo.FindById(rd.StationeryId).Description,
                    ItemCode = stationeryRepo.FindById(rd.StationeryId).Code,
                    RequisitionID = requisitionRepo.FindById(rd.RequisitionId).Id,
                    RequisitionDateTime = requisitionRepo.FindById(rd.RequisitionId).DateTime,

                };
                DisplayDisbursementDetailsList.Add(disbursementDTO);
            }

            return DisplayDisbursementDetailsList;
        }

        public List<DisbursementListDTO> GetClerkDisbursementsDetails(int EmployeeId)
        {

            List<RequisitionDetail> RequisitionDetailsList = requisitionDetailRepo.GetRequisitionDetailsByClerkDisbursementId(EmployeeId);
            List<DisbursementListDTO> DisplayDisbursementDetailsList = new List<DisbursementListDTO>();
            foreach (RequisitionDetail rd in RequisitionDetailsList)
            {
                DisbursementListDTO disbursementDTO = new DisbursementListDTO()
                {
                    Id = rd.Id,
                    DisbursementId = (int)rd.DisbursementId,
                    ReceivedEmployeeId = (int)disbursementRepo.FindById((int)rd.DisbursementId).ReceivedEmployeeId,
                    DeliveredEmployeeId = disbursementRepo.FindById((int)rd.DisbursementId).DeliveredEmployeeId,
                    DeliveryDateTime = (DateTime)disbursementRepo.FindById((int)rd.DisbursementId).DeliveryDateTime,
                    CollectionPoint = disbursementRepo.FindById((int)rd.DisbursementId).CollectionPoint,
                    QuantityOrdered = rd.QuantityOrdered,
                    QuantityDelivered = (int)rd.QuantityDelivered,
                    StationeryId = stationeryRepo.FindById(rd.StationeryId).Id,
                    Status = rd.Status,
                    UnitOfMeasure = stationeryRepo.FindById(rd.StationeryId).UnitOfMeasure,
                    EmployeeId = employeeRepo.FindById((int)disbursementRepo.FindById((int)rd.DisbursementId).ReceivedEmployeeId).Id,
                    EmployeeName = employeeRepo.FindById(requisitionRepo.FindById(rd.RequisitionId).EmployeeId).Name,
                    DepartmentName = departmentRepo.FindById(employeeRepo.FindById((int)disbursementRepo.FindById((int)rd.DisbursementId).ReceivedEmployeeId).DepartmentId).DepartmentName,
                    DepartmentId = departmentRepo.FindById(employeeRepo.FindById((int)disbursementRepo.FindById((int)rd.DisbursementId).ReceivedEmployeeId).DepartmentId).Id,
                    DepartmentCode = departmentRepo.FindById(employeeRepo.FindById((int)disbursementRepo.FindById((int)rd.DisbursementId).ReceivedEmployeeId).DepartmentId).DepartmentCode,
                    DepartmentContactName = departmentRepo.FindById(employeeRepo.FindById((int)disbursementRepo.FindById((int)rd.DisbursementId).ReceivedEmployeeId).DepartmentId).ContactName,
                    DepartmentContactNumber = departmentRepo.FindById(employeeRepo.FindById((int)disbursementRepo.FindById((int)rd.DisbursementId).ReceivedEmployeeId).DepartmentId).TelephoneNo,
                    Category = categoryRepo.FindById(stationeryRepo.FindById(rd.StationeryId).CategoryId).Type,
                    ItemName = stationeryRepo.FindById(rd.StationeryId).Description,
                    ItemCode = stationeryRepo.FindById(rd.StationeryId).Code,
                    RequisitionID = requisitionRepo.FindById(rd.RequisitionId).Id,
                    RequisitionDateTime = requisitionRepo.FindById(rd.RequisitionId).DateTime,

                };
                DisplayDisbursementDetailsList.Add(disbursementDTO);
            }
            return DisplayDisbursementDetailsList;
        }
    }
}