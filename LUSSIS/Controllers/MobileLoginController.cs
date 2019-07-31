using LUSSIS.Models;
using LUSSIS.Services;
using LUSSIS.Services.Interfaces;
using Newtonsoft.Json;
using System;
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
