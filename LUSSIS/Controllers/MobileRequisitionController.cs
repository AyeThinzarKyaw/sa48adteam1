using LUSSIS.Models;
using LUSSIS.Models.DTOs;
using LUSSIS.Services;
using LUSSIS.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
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
            requisitionCatalogueService = RequisitionCatalogueService.Instance;
            requisitionManagementService = RequisitionManagementService.Instance;
        }

        // GET: api/MobileRequisition/5
        public RequisitionsDTO Get(int id)
        {
            //Get all requsition from this employee
            List<Requisition> requisitionHistory = requisitionCatalogueService.GetPersonalRequisitionHistory(id);
            foreach(Requisition r in requisitionHistory)
            {
                RequisitionDetailsDTO model1 = requisitionCatalogueService.GetRequisitionDetailsForSingleRequisition(r.Id, 1);
                r.RequisitionDetails = model1.RequisitionDetails;
            }

            RequisitionsDTO model = new RequisitionsDTO() { LoginDTO = null, Requisitions = requisitionHistory };

            return model;
        }

        // GET: api/MobileRequisition/Pending/5
        [Route("Pending/{Id}")]
        public RequisitionsDTO GetPending(int id)
        {
            //Get all pending requsition from this employee's department

            List<Requisition> departmentRequisitions = requisitionManagementService.GetPendingDepartmentRequisitions(id);
            foreach (Requisition r in departmentRequisitions)
            {
                RequisitionDetailsDTO model1 = requisitionCatalogueService.GetRequisitionDetailsForSingleRequisition(r.Id, 1);
                r.RequisitionDetails = model1.RequisitionDetails;
            }

            RequisitionsDTO model = new RequisitionsDTO() { LoginDTO = null, Requisitions = departmentRequisitions };

            return model;
        }

        public string Post([FromBody]RequisitionApprovalDTO value)
        {
            requisitionManagementService.ApproveRejectPendingRequisition(value.Id, value.Button, value.Comment);
        }

        // GET: api/MobileRequisition/Details/5/5
        [Route("Details/{Id}/{Id2}")]
        public RequisitionDetailsDTO GetDetails(int id, int id2)
        {
            //Get all requsition from this employee

            RequisitionDetailsDTO model = requisitionCatalogueService.GetRequisitionDetailsForSingleRequisition(id2, id);

            return model;
        }
 
    }
}
