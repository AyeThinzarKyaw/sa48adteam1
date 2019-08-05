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


        public MobileRequisitionController()
        {
            requisitionCatalogueService = RequisitionCatalogueService.Instance;
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
