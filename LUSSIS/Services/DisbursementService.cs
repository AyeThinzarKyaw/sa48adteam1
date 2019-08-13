﻿using System;
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
                    ReceivedEmployeeId = (int)disbursementRepo.FindById((int)rd.DisbursementId).ReceivedEmployeeId,
                    RepName = employeeRepo.FindById((int)disbursementRepo.FindById((int)rd.DisbursementId).ReceivedEmployeeId).Name,
                    DeliveredEmployeeId = disbursementRepo.FindById((int)rd.DisbursementId).DeliveredEmployeeId,
                    //DeliveryDateTime = (DateTime)disbursementRepo.FindById((int)rd.DisbursementId).DeliveryDateTime,
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
                    ReceivedEmployeeId = (int)disbursementRepo.FindById((int)rd.DisbursementId).ReceivedEmployeeId,
                    RepName = employeeRepo.FindById((int)disbursementRepo.FindById((int)rd.DisbursementId).ReceivedEmployeeId).Name,
                    DeliveredEmployeeId = disbursementRepo.FindById((int)rd.DisbursementId).DeliveredEmployeeId,
                    //DeliveryDateTime = (DateTime)disbursementRepo.FindById((int)rd.DisbursementId).DeliveryDateTime,
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

        public Models.MobileDTOs.DisbursementListDTO GetDeptRepDisbursements(int EmployeeId)
        {
            Models.MobileDTOs.DisbursementListDTO disbursementList = new Models.MobileDTOs.DisbursementListDTO();
            List<Disbursement> disbursements = (List <Disbursement>) disbursementRepo.GetDisbursementsByDeptRepId(EmployeeId);
            List<Models.MobileDTOs.DisbursementDTO> disbursements1 = new List<Models.MobileDTOs.DisbursementDTO>();
            foreach(Disbursement disbursement in disbursements)
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
                    //DeliveryDateTime = disbursement.DeliveryDateTime
            };
                disbursement1.RequisitionDetails = new List<Models.MobileDTOs.RequisitionDetailDTO>();
                foreach(RequisitionDetail rd in disbursement.RequisitionDetails)
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
                    //DeliveryDateTime = disbursement.DeliveryDateTime
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
    }
}