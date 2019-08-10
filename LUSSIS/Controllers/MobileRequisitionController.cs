using LUSSIS.Models;
using LUSSIS.Models.DTOs;
using LUSSIS.Models.MobileDTOs;
using LUSSIS.Services;
using LUSSIS.Services.Interfaces;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Web.Http;

namespace LUSSIS.Controllers
{
    [RoutePrefix("api/MobileRequisition")]
    public class MobileRequisitionController : ApiController
    {
        IRequisitionCatalogueService requisitionCatalogueService;
        IRequisitionManagementService requisitionManagementService;

        public MobileRequisitionController()
        {
            requisitionCatalogueService = RequisitionCatalogueService.Instance; ;
            requisitionManagementService = RequisitionManagementService.Instance;
        }

        // GET: api/MobileRequisition/5
        public RequisitionListDTO Get(int id)
        {
            //Get all requsition from this employee
            List<Requisition> requisitionHistory = requisitionCatalogueService.GetPersonalRequisitionHistory(id);
            List<RequisitionDTO> moDepartmentRequisition = new List<RequisitionDTO>();
            foreach (Requisition r in requisitionHistory)
            {
                RequisitionDTO rDTO = new RequisitionDTO
                {
                    Employee = new EmployeeDTO
                    {
                        DepartmentId = r.Employee.DepartmentId,
                        Email = r.Employee.Email,
                        Name = r.Employee.Name,
                        Id = r.Employee.Id,
                        Image = r.Employee.Image,
                        Password = r.Employee.Password,
                        RoleId = r.Employee.RoleId,
                        Username = r.Employee.Username,
                        Title = r.Employee.Title
                    },

                    Id = r.Id,
                    EmployeeId = r.EmployeeId,
                    DateTime = r.DateTime,
                    Status = r.Status,
                    Remarks = r.Remarks
                };

                foreach (RequisitionDetail rd in r.RequisitionDetails)
                {
                    RequisitionDetailDTO rdDTO = new RequisitionDetailDTO
                    {
                        Id = rd.Id,
                        RequisitionId = rd.RequisitionId,
                        DisbursementId = rd.DisbursementId,
                        StationeryId = rd.StationeryId,
                        QuantityOrdered = rd.QuantityOrdered,
                        QuantityDelivered = rd.QuantityDelivered,
                        Status = rd.Status,

                        Stationery = new StationeryDTO
                        {
                            Id = rd.Stationery.Id,
                            Description = rd.Stationery.Description,
                            Bin = rd.Stationery.Bin,
                            Image = rd.Stationery.Image
                        }
                    };

                    rDTO.RequisitionDetails.Add(rdDTO);
                }

                moDepartmentRequisition.Add(rDTO);
            }
            RequisitionListDTO model = new RequisitionListDTO() { Requisitions = moDepartmentRequisition };
            return model;
        }

        // GET: api/MobileRequisition/Pending/5
        //[Route("Pending/{Id}")]
        //public RequisitionsDTO GetPending(int id)
        //{
        //    //Get all pending requsition from this employee's department
        //    List<Requisition> temp = new List<Requisition>();
        //    List<Requisition> departmentRequisitions = requisitionManagementService.GetPendingDepartmentRequisitions(id);
        //    temp = departmentRequisitions;
        //    foreach (Requisition r in temp)
        //    {
        //        var virtualProperties = typeof(Employee).GetProperties(BindingFlags.Instance | BindingFlags.Public).Where(x => x.GetSetMethod().IsVirtual && !x.PropertyType.IsPrimitive);
        //        foreach (var propInfo in virtualProperties)
        //        {
        //            propInfo.GetValue(r.Employee);
        //            propInfo.SetValue(r.Employee, null);
        //        }

        //        foreach (RequisitionDetail rd in r.RequisitionDetails)
        //        {
        //            virtualProperties = typeof(RequisitionDetail).GetProperties(BindingFlags.Instance | BindingFlags.Public).Where(x => x.GetSetMethod().IsVirtual && !x.PropertyType.IsPrimitive);
        //            foreach (PropertyInfo propInfo in virtualProperties)
        //            {
        //                if (propInfo.PropertyType != typeof(Stationery))
        //                {
        //                    propInfo.GetValue(rd);
        //                    propInfo.SetValue(rd, null);
        //                }
        //            }

        //            virtualProperties = typeof(Stationery).GetProperties(BindingFlags.Instance | BindingFlags.Public).Where(x => x.GetSetMethod().IsVirtual && !x.PropertyType.IsPrimitive);
        //            foreach (var propInfo in virtualProperties)
        //            {
        //                propInfo.GetValue(rd.Stationery);
        //                propInfo.SetValue(rd.Stationery, null);
        //            }

        //        }

        //    }

        //    RequisitionsDTO model = new RequisitionsDTO() { LoginDTO = null, Requisitions = temp };

        //    return model;
        //}

        [Route("Pending/{Id}")]
        public RequisitionListDTO GetPending(int id)
        {
            //Get all pending requsition from this employee's department
            List<Requisition> departmentRequisitions = requisitionManagementService.GetPendingDepartmentRequisitions(id);
            List<RequisitionDTO> moDepartmentRequisition = new List<RequisitionDTO>();
            foreach (Requisition r in departmentRequisitions)
            {
                RequisitionDTO rDTO = new RequisitionDTO
                {
                    Employee = new EmployeeDTO
                    {
                        DepartmentId = r.Employee.DepartmentId,
                        Email = r.Employee.Email,
                        Name = r.Employee.Name,
                        Id = r.Employee.Id,
                        Image = r.Employee.Image,
                        Password = r.Employee.Password,
                        RoleId = r.Employee.RoleId,
                        Username = r.Employee.Username,
                        Title = r.Employee.Title
                    },

                    Id = r.Id,
                    EmployeeId = r.EmployeeId,
                    DateTime = r.DateTime,
                    Status = r.Status,
                    Remarks = r.Remarks
                };

                foreach (RequisitionDetail rd in r.RequisitionDetails)
                {
                    RequisitionDetailDTO rdDTO = new RequisitionDetailDTO
                    {
                        Id = rd.Id,
                        RequisitionId = rd.RequisitionId,
                        DisbursementId = rd.DisbursementId,
                        StationeryId = rd.StationeryId,
                        QuantityOrdered = rd.QuantityOrdered,
                        QuantityDelivered = rd.QuantityDelivered,
                        Status = rd.Status,

                        Stationery = new StationeryDTO
                        {
                            Id = rd.Stationery.Id,
                            Description = rd.Stationery.Description,
                            Bin = rd.Stationery.Bin,
                            Image = rd.Stationery.Image
                        }
                    };

                    rDTO.RequisitionDetails.Add(rdDTO);
                }

                moDepartmentRequisition.Add(rDTO);
            }
            RequisitionListDTO model = new RequisitionListDTO() { Requisitions = moDepartmentRequisition };
            return model;
        }

        public void Post([FromBody]RequisitionApprovalDTO value)
        {
            requisitionManagementService.ApproveRejectPendingRequisition(value.Id, value.Button, value.Comment);
        }

        // GET: api/MobileRequisition/Details/5/5
        //[Route("Details/{Id}/{Id2}")]
        //public RequisitionDetailsDTO GetDetails(int id, int id2)
        //{
        //    //Get all requsition from this employee

        //    RequisitionDetailsDTO model = requisitionCatalogueService.GetRequisitionDetailsForSingleRequisition(id2, id);

        //    return model;
        //}
 
    }
}
