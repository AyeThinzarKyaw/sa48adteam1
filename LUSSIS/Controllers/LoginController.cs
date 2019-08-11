using LUSSIS.Models;
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

        public LoginController()
        {
            this.loginService = LoginService.Instance;
        }


        // GET: Login  

        public ActionResult Index()
        {
            //is this user already logged into this browser?
            if (Session["existinguser"] != null)
            {
                LoginDTO loginDto = (LoginDTO)Session["existinguser"];
                //search for this employee in db
                Session s = loginService.GetExistingSessionFromGUID(loginDto.SessionGuid);
                if (s != null) //valid GUID in db
                {
                    //LoginDTO loginDTO = new LoginDTO {EmployeeId = s.EmployeeId, RoleId = s.Employee.RoleId, SessionGuid = GUID };

                    return RedirectToAction("RedirectToClerkOrDepartmentView", loginDto);
                }
            }
            //else need to present the login page
            return View();
        }

        public ActionResult RedirectToClerkOrDepartmentView()
        {
            if (Session["existinguser"] != null)
            {
                LoginDTO loginDTO = (LoginDTO)Session["existinguser"];
                if (loginDTO.RoleId >= (int)Enums.Roles.DepartmentHead && loginDTO.RoleId <= (int)Enums.Roles.DepartmentCoverHead) //dept staff
                {
                    return RedirectToAction("ViewCatalogue", "Requisition");
                }
                else //clerks
                {
                    //return clerk view
                    return RedirectToAction("", "", loginDTO);
                }
            }
            return RedirectToAction("Index", "Login");
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
                ViewBag.ErrorMessage = "Incorrect Username or Password!";
                return View("Index"); //with error message

            }
            else
            {
                //store in browser session also
                Session["existinguser"] = loginDTO;
                Session["role"] = loginDTO.RoleId;
                return RedirectToAction("RedirectToClerkOrDepartmentView", loginDTO);
            }

        }

        public ActionResult Logout()
        {
            if (Session["existinguser"] != null)
            {
                LoginDTO loginDTO = (LoginDTO)Session["existinguser"];
                loginService.LogoutUser(loginDTO.SessionGuid);
                Session["existinguser"] = null;
                Session["role"] = null;
                return RedirectToAction("Index");
            }
            return RedirectToAction("Index", "Login");
        }
    }
}