using LUSSIS.Models;
using LUSSIS.Models.DTOs;
using LUSSIS.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LUSSIS.Controllers
{
    public class CollectionPointController : Controller
    {
        // Head/CoverStaff Assign Department's CollectionPoint 
        public ActionResult AssignCollectionPoint(LoginDTO loginDTO)
        {
            DepartmentCollectionPointDTO cpdetails = new DepartmentCollectionPointDTO();
            cpdetails.LoginDTO = loginDTO;
            //int eid = loginDTO.EmployeeId;
            //to change 1 to eid
            cpdetails.DepartmentCollectionPoint = CollectionPointService.Instance.GetDepartmentCollectionPointByEmployeeId(1);
            cpdetails.CollectionPoint1 = CollectionPointService.Instance.GetCollectionPointById(1);
            cpdetails.CollectionPoint2 = CollectionPointService.Instance.GetCollectionPointById(2);
            cpdetails.CollectionPoint3 = CollectionPointService.Instance.GetCollectionPointById(3);
            cpdetails.CollectionPoint4 = CollectionPointService.Instance.GetCollectionPointById(4);
            cpdetails.CollectionPoint5 = CollectionPointService.Instance.GetCollectionPointById(5);
            cpdetails.CollectionPoint6 = CollectionPointService.Instance.GetCollectionPointById(6);
            return View(cpdetails);
        }

        [HttpPost]
        public ActionResult AssignCollectionPoint(DepartmentCollectionPointDTO cpdetails)
        {
            //int eid = cpdetails.LoginDTO.EmployeeId;
            //to change 1 to eid
            Department department = CollectionPointService.Instance.GetDepartmentByEmployeeId(1);
            department.CollectionPointId = cpdetails.DepartmentCollectionPointId;
            CollectionPointService.Instance.UpdateDepartmentCollectionPoint(department);

            return RedirectToAction("AssignCollectionPoint", "CollectionPoint");
        }

        //Assign Clerks CollectionPoints
        public ActionResult AssignClerk(LoginDTO loginDTO)
        {
            ClerkCollectionPointDTO cpdetails = new ClerkCollectionPointDTO();
            cpdetails.LoginDTO = loginDTO;
            cpdetails.Clerks = CollectionPointService.Instance.GetAllClerks();
            cpdetails.CollectionPoints = CollectionPointService.Instance.GetAllCollectionPoints();
            cpdetails.CollectionPoint1 = CollectionPointService.Instance.GetCollectionPointById(1);
            cpdetails.CollectionPoint2 = CollectionPointService.Instance.GetCollectionPointById(2);
            cpdetails.CollectionPoint3 = CollectionPointService.Instance.GetCollectionPointById(3);
            cpdetails.CollectionPoint4 = CollectionPointService.Instance.GetCollectionPointById(4);
            cpdetails.CollectionPoint5 = CollectionPointService.Instance.GetCollectionPointById(5);
            cpdetails.CollectionPoint6 = CollectionPointService.Instance.GetCollectionPointById(6);
            return View(cpdetails);
        }

        [HttpPost]
        public ActionResult AssignClerk(ClerkCollectionPointDTO cpdetails)
        {
            CollectionPoint cp1 = CollectionPointService.Instance.GetCollectionPointById(1);
            cp1.EmployeeId = cpdetails.Employee1;
            CollectionPointService.Instance.UpdateCollectionPoint(cp1);

            CollectionPoint cp2 = CollectionPointService.Instance.GetCollectionPointById(2);
            cp2.EmployeeId = cpdetails.Employee2;
            CollectionPointService.Instance.UpdateCollectionPoint(cp2);

            CollectionPoint cp3 = CollectionPointService.Instance.GetCollectionPointById(3);
            cp3.EmployeeId = cpdetails.Employee3;
            CollectionPointService.Instance.UpdateCollectionPoint(cp3);

            CollectionPoint cp4 = CollectionPointService.Instance.GetCollectionPointById(4);
            cp4.EmployeeId = cpdetails.Employee4;
            CollectionPointService.Instance.UpdateCollectionPoint(cp4);

            CollectionPoint cp5 = CollectionPointService.Instance.GetCollectionPointById(5);
            cp5.EmployeeId = cpdetails.Employee5;
            CollectionPointService.Instance.UpdateCollectionPoint(cp5);

            CollectionPoint cp6 = CollectionPointService.Instance.GetCollectionPointById(6);
            cp6.EmployeeId = cpdetails.Employee6;
            CollectionPointService.Instance.UpdateCollectionPoint(cp6);


            return RedirectToAction("AssignClerk", "CollectionPoint");
        }

    }
}
