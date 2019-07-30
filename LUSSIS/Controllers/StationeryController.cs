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
        // GET: Stationery List
        //By ATZK
        public ActionResult Index()
        {
            var stationeries = StationeryService.Instance.GetAllStationeries().OrderBy(x => x.Code).ToList();
            return View(stationeries);
        }

        // GET: StationeryDetails
        //By ATZK
        public ActionResult Detail(int stationeryId)
        {
            Stationery stationery= StationeryService.Instance.GetStationeryById(stationeryId);
            stationery.SupplierTenders = stationery.SupplierTenders.OrderBy(x => x.Rank).ToList();
            return View(stationery);
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