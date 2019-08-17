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
        private IRetrievalService retrievalService;
        private IRequisitionCatalogueService requisitionCatalogueService;
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
            retrievalService = RetrievalService.Instance;
            requisitionCatalogueService = RequisitionCatalogueService.Instance;
        }
        private static DisbursementService instance = new DisbursementService();

        public static IDisbursementService Instance
        {
            get { return instance; }
        }



        public List<DisbursementDetailsDTO> GetDepRepDisbursementsDetails(int EmployeeId)
        {
            List<RequisitionDetail> RequisitionDetailsList = requisitionDetailRepo.GetRequisitionDetailsByDepRepDisbursementId(EmployeeId);
            List<DisbursementDetailsDTO> DisplayDisbursementDetailsList = new List<DisbursementDetailsDTO>();
            foreach (RequisitionDetail rd in RequisitionDetailsList)
            {

                DisbursementDetailsDTO disbursementDTO = new DisbursementDetailsDTO()
                {
                    Id = rd.Id,
                    DisbursementId = (int)rd.DisbursementId,
                    ReceivedEmployeeId = (int)rd.Disbursement.ReceivedEmployeeId,
                    DeliveredEmployeeId = rd.Disbursement.DeliveredEmployeeId,
                    CollectionPoint = rd.Disbursement.CollectionPoint,
                    QuantityOrdered = rd.QuantityOrdered,
                    QuantityDelivered = rd.QuantityDelivered == null ? 0 : (int)rd.QuantityDelivered,
                    StationeryId = rd.StationeryId,
                    Status = rd.Status,
                    UnitOfMeasure = rd.Stationery.UnitOfMeasure,
                    EmployeeId = rd.Disbursement.Employee1.Id,
                    EmployeeName = rd.Disbursement.Employee1.Name,
                    DepartmentName = rd.Disbursement.Employee1.Department.DepartmentName,
                    DepartmentId = rd.Disbursement.Employee1.Department.Id,
                    DepartmentCode = rd.Disbursement.Employee1.Department.DepartmentCode,
                    DepartmentContactName = rd.Disbursement.Employee1.Department.ContactName,
                    DepartmentContactNumber = rd.Disbursement.Employee1.Department.TelephoneNo,
                    Category = rd.Stationery.Category.Type,
                    ItemName = rd.Stationery.Description,
                    ItemCode = rd.Stationery.Code,
                    RequisitionID = rd.RequisitionId,
                    RequisitionDateTime = rd.Requisition.DateTime,

                };
                DisplayDisbursementDetailsList.Add(disbursementDTO);


            }

            return DisplayDisbursementDetailsList;
        }

        public List<DisbursementDetailsDTO> GetClerkDisbursementsDetails(int EmployeeId)
        {

            List<RequisitionDetail> RequisitionDetailsList = requisitionDetailRepo.GetRequisitionDetailsByClerkDisbursementId(EmployeeId);
            List<DisbursementDetailsDTO> DisplayDisbursementDetailsList = new List<DisbursementDetailsDTO>();
            foreach (RequisitionDetail rd in RequisitionDetailsList)
            {

                DisbursementDetailsDTO disbursementDTO = new DisbursementDetailsDTO()
                {
                    Id = rd.Id,
                    DisbursementId = (int)rd.DisbursementId,
                    ReceivedEmployeeId = (int)rd.Disbursement.ReceivedEmployeeId,
                    DeliveredEmployeeId = rd.Disbursement.DeliveredEmployeeId,
                    CollectionPoint = rd.Disbursement.CollectionPoint,
                    QuantityOrdered = rd.QuantityOrdered,
                    QuantityDelivered = rd.QuantityDelivered == null ? 0 : (int)rd.QuantityDelivered,
                    StationeryId = rd.StationeryId,
                    Status = rd.Status,
                    UnitOfMeasure = rd.Stationery.UnitOfMeasure,
                    EmployeeId = rd.Disbursement.Employee.Id,
                    EmployeeName = rd.Disbursement.Employee.Name,
                    DepartmentName = rd.Disbursement.Employee.Department.DepartmentName,
                    DepartmentId = rd.Disbursement.Employee.Department.Id,
                    DepartmentCode = rd.Disbursement.Employee.Department.DepartmentCode,
                    DepartmentContactName = rd.Disbursement.Employee.Department.ContactName,
                    DepartmentContactNumber = rd.Disbursement.Employee.Department.TelephoneNo,
                    Category = rd.Stationery.Category.Type,
                    ItemName = rd.Stationery.Description,
                    ItemCode = rd.Stationery.Code,
                    RequisitionID = rd.RequisitionId,
                    RequisitionDateTime = rd.Requisition.DateTime,

                };
                DisplayDisbursementDetailsList.Add(disbursementDTO);


            }
            return DisplayDisbursementDetailsList;
        }

        public Models.MobileDTOs.DisbursementListDTO GetDeptRepDisbursements(int EmployeeId)
        {
            Models.MobileDTOs.DisbursementListDTO disbursementList = new Models.MobileDTOs.DisbursementListDTO();
            List<Disbursement> disbursements = (List<Disbursement>)disbursementRepo.GetDisbursementsByDeptRepId(EmployeeId);
            List<Models.MobileDTOs.DisbursementDTO> disbursements1 = new List<Models.MobileDTOs.DisbursementDTO>();
            foreach (Disbursement disbursement in disbursements)
            {
                Models.MobileDTOs.DisbursementDTO disbursement1 = new Models.MobileDTOs.DisbursementDTO
                {
                    Id = disbursement.Id,
                    DeliveredEmployeeId = disbursement.DeliveredEmployeeId,
                    ReceivedEmployeeId = (int)disbursement.ReceivedEmployeeId,
                    ReceivedEmployeeName = disbursement.Employee1.Name,
                    DepartmentName = disbursement.Employee1.Department.DepartmentName,
                    AdHoc = disbursement.AdHoc,
                    CollectionPoint = disbursement.CollectionPoint,
                    DeliveryDateTime = (DateTime)disbursement.DeliveryDateTime,
                    OnRoute = disbursement.OnRoute
                };
                disbursement1.RequisitionDetails = new List<Models.MobileDTOs.RequisitionDetailDTO>();
                foreach (RequisitionDetail rd in disbursement.RequisitionDetails)
                {
                    Models.MobileDTOs.RequisitionDetailDTO rd1 = new Models.MobileDTOs.RequisitionDetailDTO
                    {
                        Id = rd.Id,
                        RequisitionId = rd.RequisitionId,
                        DisbursementId = (int)rd.DisbursementId,
                        StationeryId = rd.StationeryId,
                        QuantityOrdered = rd.QuantityOrdered,
                        QuantityDelivered = (int)rd.QuantityDelivered,
                        Status = rd.Status,
                        Stationery = new Models.MobileDTOs.StationeryDTO
                        {
                            Description = rd.Stationery.Description
                        },
                        Requisition = new Models.MobileDTOs.RequisitionDTO
                        {
                            Employee = new Models.MobileDTOs.EmployeeDTO
                            {
                                Name = rd.Requisition.Employee.Name
                            }
                        }
                    };
                    disbursement1.RequisitionDetails.Add(rd1);
                }
                disbursements1.Add(disbursement1);
            }
            disbursementList.Disbursements = disbursements1;
            return disbursementList;
        }

        public Models.MobileDTOs.DisbursementListDTO GetClerkDisbursements(int EmployeeId)
        {
            Models.MobileDTOs.DisbursementListDTO disbursementList = new Models.MobileDTOs.DisbursementListDTO();
            List<Disbursement> disbursements = (List<Disbursement>)disbursementRepo.GetDisbursementsByClerkId(EmployeeId);
            List<Models.MobileDTOs.DisbursementDTO> disbursements1 = new List<Models.MobileDTOs.DisbursementDTO>();
            foreach (Disbursement disbursement in disbursements)
            {
                Models.MobileDTOs.DisbursementDTO disbursement1 = new Models.MobileDTOs.DisbursementDTO
                {
                    Id = disbursement.Id,
                    DeliveredEmployeeId = disbursement.DeliveredEmployeeId,
                    ReceivedEmployeeId = (int)disbursement.ReceivedEmployeeId,
                    ReceivedEmployeeName = disbursement.Employee1.Name,
                    DepartmentName = disbursement.Employee1.Department.DepartmentName,
                    AdHoc = disbursement.AdHoc,
                    CollectionPoint = disbursement.CollectionPoint,
                    DeliveryDateTime = (DateTime)disbursement.DeliveryDateTime,
                    OnRoute = disbursement.OnRoute
                };
                disbursement1.RequisitionDetails = new List<Models.MobileDTOs.RequisitionDetailDTO>();
                foreach (RequisitionDetail rd in disbursement.RequisitionDetails)
                {
                    Models.MobileDTOs.RequisitionDetailDTO rd1 = new Models.MobileDTOs.RequisitionDetailDTO
                    {
                        Id = rd.Id,
                        RequisitionId = rd.RequisitionId,
                        DisbursementId = (int)rd.DisbursementId,
                        StationeryId = rd.StationeryId,
                        QuantityOrdered = rd.QuantityOrdered,
                        QuantityDelivered = (int)rd.QuantityDelivered,
                        Status = rd.Status,
                        Stationery = new Models.MobileDTOs.StationeryDTO
                        {
                            Description = rd.Stationery.Description
                        },
                        Requisition = new Models.MobileDTOs.RequisitionDTO
                        {
                            Employee = new Models.MobileDTOs.EmployeeDTO
                            {
                                Name = rd.Requisition.Employee.Name
                            }
                        }
                    };
                    disbursement1.RequisitionDetails.Add(rd1);
                }
                disbursements1.Add(disbursement1);
            }
            disbursementList.Disbursements = disbursements1;
            return disbursementList;
        }

        public void CompleteDisbursementProcess(Models.MobileDTOs.DisbursementDTO disbursement)
        {
            List<Models.MobileDTOs.RequisitionDetailDTO> requisitionDetails = disbursement.RequisitionDetails;
            List<RequisitionDetail> unfulfilledRds = new List<RequisitionDetail>();

            foreach (Models.MobileDTOs.RequisitionDetailDTO rd in requisitionDetails)
            {
                if (rd.QuantityOrdered != rd.QuantityDelivered)
                {
                    int qtyDifference = (int)rd.QuantityDelivered - rd.QuantityOrdered;

                    AdjustmentVoucher targetAdjustmentVoucher = retrievalService.retrieveNewOrAvailableAdjustmentVoucherForClerk(disbursement.DeliveredEmployeeId);
                    retrievalService.createNewAdjustmentVoucherDetail(targetAdjustmentVoucher, rd.StationeryId, qtyDifference);

                    if (rd.QuantityDelivered > 0)
                    {
                        RequisitionDetail rd1 = requisitionDetailRepo.FindById(rd.Id);
                        rd1.QuantityDelivered = rd.QuantityDelivered;
                        rd1.Status = RequisitionDetailStatusEnum.COLLECTED.ToString();
                        requisitionDetailRepo.Update(rd1);

                        int diff = rd.QuantityOrdered - (int)rd.QuantityDelivered;
                        int availStockForUnfulfilled = requisitionCatalogueService.GetAvailStockForUnfulfilledRd(rd.StationeryId, rd.Id);

                        if (availStockForUnfulfilled < diff) //insufficient stock
                        {
                            int waitlistCount = diff - availStockForUnfulfilled;
                            requisitionCatalogueService.createNewRequisitionDetail(waitlistCount, rd.RequisitionId, rd.StationeryId, RequisitionDetailStatusEnum.WAITLIST_APPROVED);

                            if (availStockForUnfulfilled > 0)
                            {
                                requisitionCatalogueService.createNewRequisitionDetail(availStockForUnfulfilled, rd.RequisitionId, rd.StationeryId, RequisitionDetailStatusEnum.PREPARING);
                            }
                        }
                        else
                        {
                            requisitionCatalogueService.createNewRequisitionDetail(diff, rd.RequisitionId, rd.StationeryId, RequisitionDetailStatusEnum.PREPARING);
                        }
                    }
                }
                else
                {
                    RequisitionDetail rd1 = requisitionDetailRepo.FindById(rd.Id);
                    rd1.QuantityDelivered = rd.QuantityDelivered;
                    rd1.Status = RequisitionDetailStatusEnum.COLLECTED.ToString();
                    requisitionDetailRepo.Update(rd1);
                }
            }
            requisitionCatalogueService.CheckRequisitionCompletenessAfterDisbursement(disbursement.Id, disbursement);

        }

        public void SetDisbursementOnRoute(int disbursementId)
        {
            Disbursement d = disbursementRepo.FindById(disbursementId);
            d.OnRoute = true;
            disbursementRepo.Update(d);
        }
    }
}