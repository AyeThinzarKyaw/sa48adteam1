using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LUSSIS.Models;
using LUSSIS.Services;
using LUSSIS.Services.Interfaces;

namespace LUSSIS.Controllers
{
    public class StationeryController : Controller
    {
        IStationeryService stationeryService;
        // GET: Stationery
        public ActionResult Index()
        {
            
            return View();
        }
    }
}