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
    public class MachineLearningController : Controller
    {
        // GET: MachineLearning
        public ActionResult MachineLearning()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> MachineLearning(MachineLearningDTO predModel)
        {
            using (var client = new HttpClient())
            {

                //http POST

                // send a POST request to the server uri with the data and get the response as HttpResponseMessage object
                // add 'Microsoft.AspNet.WebApi.Client' Nuget package
                HttpResponseMessage res = await client.PostAsJsonAsync("http://127.0.0.1:5000/", predModel);

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
                                int qty= Convert.ToInt32(a);
                                updatedStationeries.Find(x => x.Id == currentId).ReorderLevel = qty;
                                updatedStationeries.Find(x => x.Id == currentId).ReorderQuantity = qty;
                            }
                           
                            i = i + 1;
                        }
                        
                    }
                    //string[] ElementArray = new string[6];
                    //ArrayList arrayList1 = new ArrayList();
                    //JArray jsonArray = JArray.Parse(res.Content.ReadAsStringAsync().Result);
                    //int i;
                    //foreach (JArray ja in jsonArray)
                    //{
                    //    i = 0;
                    //    foreach (string a in ja)
                    //    {
                    //        if (i == 0)
                    //            ElementArray[0] = a;
                    //        if (i == 1)
                    //            ElementArray[1] = a;
                    //        //if (i == 2)
                    //        //    ElementArray[2] = a;
                    //        //if (i == 3)
                    //        //    ElementArray[3] = a;
                    //        //if (i == 4)
                    //        //    ElementArray[4] = a;
                    //        //if (i == 5)
                    //        //    ElementArray[5] = a;
                    //        i = i + 1;
                    //    }
                    //    arrayList1.Add(ja);
                    //}
                    TempData["updatedStationeryQty"] = updatedStationeries;
                    ViewBag.data = updatedStationeries;
                    return View();
                }
                else
                {
                    return View("Error");
                }

            }

        }

        public void UpdateReorderQuantity()
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

        }
    }
}