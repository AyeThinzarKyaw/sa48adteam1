using LUSSIS.Models.DTOs;
using LUSSIS.Services;
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
        IRequisitionCatalogueService requisitionCatalogueService;
        

        public LoginController()
        {
            this.loginService = LoginService.Instance;
            this.requisitionCatalogueService = RequisitionCatalogueService.Instance;
        }


        // GET: Login  

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult VerifyUser(FormLoginDTO loginForm)
        {

            //receives a username and password
            //call loginservice to check if valid user
            //if yes please give all the detail of this user that enables me to show him his dashboard

            LoginDTO loginDTO = loginService.GetEmployeeLoginByUsernameAndPassword(loginForm.Username, loginForm.Password);
            if (loginDTO == null)
            {
                //invalid user
                return View("Index"); //with error message

            }
            else
            {
                //check for role
                if (loginDTO.RoleId >=1 && loginDTO.RoleId <= 4) //dept staff
                {
                    return RedirectToAction("ViewCatalogue", "Requisition", loginDTO);
                }
                else //clerks
                {
                    //return clerk view
                }

                return View("dashboard");
            }

        }

        public ActionResult Logout(LoginDTO loginDTO)
        {
            loginService.LogoutUser(loginDTO.SessionGuid);

            return RedirectToAction("Index");
        }
    }
}