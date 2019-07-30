using LUSSIS.Models.DTOs;
using LUSSIS.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LUSSIS.Controllers
{
    public class LoginController : Controller
    {
        ILoginService loginService;

        public LoginController(ILoginService loginService)
        {
            this.loginService = loginService;
        }

        //some routing
        // GET: Login
        public ActionResult Index()
        {
            //receives a username and password
            //call loginservice to check if valid user
            //if yes please give all the detail of this user that enables me to show him his dashboard

            LoginDTO loginDTO = loginService.GetEmployeeLoginByUsernameAndPassword("some username", "some password");
            if (loginDTO == null)
            {
                //return loginpage with error message

            }
            else
            {
                //return dashboard view
                return View(loginDTO);
            }

            return View();
        }
    }
}