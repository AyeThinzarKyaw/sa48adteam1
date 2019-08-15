using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using System.Collections;
using LUSSIS.Models.DTOs;
using LUSSIS.Models;
using LUSSIS.Services;

namespace LUSSIS.Controllers
{
    public class PredictionController : Controller
    {
        // GET: GenerateReorderInfo
        public ActionResult GenerateReorderInfo()
        {
            return View(new MachineLearningDTO() { chosenDate=DateTime.Now});
        }

        [HttpPost]
        public async Task<ActionResult> GenerateReorderInfo(MachineLearningDTO predictedDate)
        {
            using (var client = new HttpClient())
            {

                //http POST

                // send a POST request to the server uri with the data and get the response as HttpResponseMessage object
                // add 'Microsoft.AspNet.WebApi.Client' Nuget package
                HttpResponseMessage res = await client.PostAsJsonAsync("http://127.0.0.1:5000/", new { @InputYear =predictedDate.chosenDate.Year, @InputMonth =predictedDate.chosenDate.Month, @InputDay =predictedDate.chosenDate.Day});

                // Return the result from the server if the status code is 200 (everything is OK)
                // should raise exception or error if it's not
                if (res.IsSuccessStatusCode)
                {
                    // pass the result by setting the Viewdata property
                    // have to read as string for the data in response.body

                    List<Stationery> stationeries = StationeryService.Instance.GetAllStationeries().ToList();
                    List<Stationery> updatedStationeries = new List<Stationery>();


                    JArray jsonArray = JArray.Parse(res.Content.ReadAsStringAsync().Result);
                    int i;
                    foreach (JArray ja in jsonArray)
                    {
                        i = 0;
                        int currentId = 0;
                        foreach (string a in ja)
                        {
                            if (i == 0)
                            {
                                currentId = Convert.ToInt32(a);
                                updatedStationeries.Add(stationeries.Find(x => x.Id == currentId));

                            }

                            if (i == 1)
                            {
                                int qty = Convert.ToInt32(a);
                                updatedStationeries.Find(x => x.Id == currentId).ReorderLevel = qty;
                                updatedStationeries.Find(x => x.Id == currentId).ReorderQuantity = qty;
                            }

                            i = i + 1;
                        }

                    }
                    TempData["updatedStationeryQty"] = updatedStationeries;
                    ViewBag.data = updatedStationeries;
                    return View(predictedDate);
                }
                else
                {
                    return View("Error");
                }

            }

        }

        public ActionResult UpdateReorderQuantity(DateTime predictDate)
        {
            List<Stationery> stationeries = (List<Stationery>)TempData["updatedStationeryQty"];
            TempData.Keep("updatedStationeryQty");
            foreach (var item in stationeries)
            {
                if (item.ReorderQuantity>0)
                {
                    StationeryService.Instance.UpdateStationery(item);
                }
            }
            ViewBag.data = stationeries;
            ViewBag.Message = "Successfully updated the reorder information.";
            return View("GenerateReorderInfo", new MachineLearningDTO() { chosenDate = predictDate });
        }
    }
}