using LUSSIS.Models;
using LUSSIS.Services;
using LUSSIS.Services.Interfaces;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace LUSSIS.Controllers
{
    public class MobileLoginController : ApiController
    {
        private LUSSISContext db = new LUSSISContext();
        ILoginService loginService;

        public MobileLoginController()
        {
            this.loginService = LoginService.Instance;
        }

        //POST: api/MobileLogin
        //Written by Charles
        public Employee Post([FromBody]Employee employee)
        {
            if(db.Employees.Any(user => user.Username.Equals(employee.Username)))
            {
                Employee emp = db.Employees.Where(user => user.Username.Equals(employee.Username)).First();

                //testing data transfer
                emp.Sessions = db.Sessions.Where(u => u.EmployeeId.Equals(emp.Id)).ToList();
                emp.Requisitions = db.Requisitions.Where(u => u.EmployeeId.Equals(emp.Id)).ToList();
                emp.Disbursements = db.Disbursements.Where(u => u.ReceivedEmployeeId==(emp.Id) && u.DeliveryDateTime != null).ToList();
                
                //ICollection<RequisitionDetail> requisitionDetails;
                foreach (Requisition r in emp.Requisitions)
                {
                    r.RequisitionDetails = db.RequisitionDetails.Where(u => u.RequisitionId.Equals(r.Id)).ToList();
                }

                foreach (Disbursement d in emp.Disbursements)
                {
                    d.RequisitionDetails = db.RequisitionDetails.Where(u => u.RequisitionId.Equals(d.Id)).ToList();
                }

                if(loginService.HashPassword(employee.Password).Equals(emp.Password))
                    return emp;
                else
                    return null;
            }
            else
            {
                return null;
            }

        }
    }
}
