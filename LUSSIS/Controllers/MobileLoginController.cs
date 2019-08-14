using LUSSIS.Models.MobileDTOs;
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
        //private LUSSISContext db = new LUSSISContext();
        ILoginService loginService;

        public MobileLoginController()
        {
            this.loginService = LoginService.Instance;
        }

        //POST: api/MobileLogin
        //Written by Charles
        //public LoginDTO Post([FromBody]Employee employee)
        public LoginDTO Post([FromBody]Models.Employee employee)
        {
            LoginDTO loginDTO = loginService.GetEmployeeLoginByUsernameAndPassword2(employee.Username, employee.Password);
            if (loginDTO == null)
            {
                //invalid user
                return null;
            }
            else
            {
                return loginDTO;
            }
        }
    }
}
