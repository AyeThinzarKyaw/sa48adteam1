using LUSSIS.Models.MobileDTOs;
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
    [RoutePrefix("api/MobileDisbursement")]
    public class MobileDisbursementController : ApiController
    {
        IDisbursementService disbursementService;

        public MobileDisbursementController()
        {
            disbursementService = DisbursementService.Instance;
        }

        // GET: api/MobileDisbursement/Department/5
        [Route("Department/{Id}")]
        public DisbursementListDTO GetDeptRepDisbursement(int id)
        {
            //Get all disbursements from department using EmployeeId
            DisbursementListDTO disbursements = disbursementService.GetDeptRepDisbursements(id);

            return disbursements;
        }

        // GET: api/MobileDisbursement/Store/5
        [Route("Store/{Id}")]
        public DisbursementListDTO GetStoreClerkDisbursement(int id)
        {
            //Get all disbursements for clerk using EmployeeId
            DisbursementListDTO disbursements = disbursementService.GetClerkDisbursements(id);

            return disbursements;
        }

        public void Post([FromBody]DisbursementDTO disbursement)
        {
            disbursementService.CompleteDisbursementProcess(disbursement);

        }

    }
}
