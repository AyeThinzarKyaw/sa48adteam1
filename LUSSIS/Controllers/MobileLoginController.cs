using LUSSIS.Models;
using LUSSIS.Models.DTOs;
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
        public LoginDTO Post([FromBody]Employee employee)
        {
            LoginDTO loginDTO = loginService.GetEmployeeLoginByUsernameAndPassword(employee.Username, employee.Password);
            if (loginDTO == null)
            {
                //invalid user
                return null;

            }
            else
            {
                //check for role
                if (loginDTO.RoleId >= 1 && loginDTO.RoleId <= 4)
                {
                    //List<CatalogueItemDTO> catalogueItems = requisitionCatalogueService.GetCatalogueItems(loginDTO.EmployeeId);
                    //FormRequisitionDTO model = new FormRequisitionDTO { CatalogueItems = catalogueItems, LoginDTO = loginDTO };

                    return loginDTO;
                }
                else
                {
                    //return clerk view
                }

                return null; ;
            }

            //if (db.Employees.Any(user => user.Username.Equals(employee.Username)))
            //{
            //    Employee emp = db.Employees.Where(user => user.Username.Equals(employee.Username)).First();

            //    //testing data transfer
            //    emp.Sessions = db.Sessions.Where(u => u.EmployeeId.Equals(emp.Id)).ToList();
            //    emp.Requisitions = db.Requisitions.Where(u => u.EmployeeId.Equals(emp.Id)).ToList();
            //    emp.Disbursements = db.Disbursements.Where(u => u.ReceivedEmployeeId==(emp.Id) && u.DeliveryDateTime != null).ToList();
                
            //    //ICollection<RequisitionDetail> requisitionDetails;
            //    foreach (Requisition r in emp.Requisitions)
            //    {
            //        r.RequisitionDetails = db.RequisitionDetails.Where(u => u.RequisitionId.Equals(r.Id)).ToList();
            //        foreach (RequisitionDetail rd in r.RequisitionDetails)
            //        {
            //            rd.Stationery = db.Stationeries.Where(u => u.Id.Equals(rd.StationeryId)).First();
            //        }
            //    }

            //    foreach (Disbursement d in emp.Disbursements)
            //    {
            //        d.RequisitionDetails = db.RequisitionDetails.Where(u => u.RequisitionId.Equals(d.Id)).ToList();
            //    }

            //    if(loginService.HashPassword(employee.Password).Equals(emp.Password))
            //        return emp;
            //    else
            //        return null;
            //}
            //else
            //{
            //    return null;
            //}

        }
    }
}
