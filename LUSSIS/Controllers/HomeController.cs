using LUSSIS.Models;
using LUSSIS.Repositories;
using LUSSIS.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LUSSIS.Controllers
{
    public class HomeController : Controller
    {
        //for testing
        IEmployeeRepo employeeRepo = EmployeeRepo.Instance;
        IRequisitionRepo reqRepo = RequisitionRepo.Instance;

        public ActionResult Index()
        {
            //for testing
            Employee e = employeeRepo.FindById(2);
            Requisition r = reqRepo.FindById(25);
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}