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
        
        public StationeryController(IStationeryService stationeryService)
        {
            this.stationeryService = stationeryService;
        }
        //StationeryService stationeryService;

        // GET: Stationery
        //By ATZK
        public ActionResult Index()
        {
            var stationeries = stationeryService.GetAllStationeries().OrderBy(x => x.Code).ToList();
            return View(stationeryService);
        }

        //CRETE stationery getMethod
        //By ATZK
        public ActionResult Create()
        {

            return View();
        }

        //CRETE stationery postMethod 
        //By ATZK
        [HttpPost]
        public ActionResult Create(Stationery stationery)
        {
            return View();
        }

        //UPDATE stationery getMethod
        //By ATZK
        public ActionResult Update()
        {

            return View();
        }

        //UPDATE stationery postMethod 
        //By ATZK
        [HttpPost]
        public ActionResult Update(Stationery stationery)
        {
            return View();
        }


    }
}