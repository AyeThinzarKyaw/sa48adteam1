using LUSSIS.Filters;
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
        [Authorizer]
        public ActionResult AssignCollectionPoint()
        {
            if (Session["existinguser"] != null)
            {
                LoginDTO currentUser = (LoginDTO)Session["existinguser"];
                if (currentUser.RoleId != (int)Enums.Roles.StoreManager)
                {
                    return RedirectToAction("RedirectToClerkOrDepartmentView", currentUser);
                }

                DepartmentCollectionPointDTO cpdetails = new DepartmentCollectionPointDTO();

                cpdetails.DepartmentCollectionPoint = CollectionPointService.Instance.GetDepartmentCollectionPointByEmployeeId(currentUser.EmployeeId);
                cpdetails.CollectionPoints = CollectionPointService.Instance.GetAllCollectionPoints();
                cpdetails.CollectionPoint1 = cpdetails.CollectionPoints.Single(x => x.Id == 1);
                cpdetails.CollectionPoint2 = cpdetails.CollectionPoints.Single(x => x.Id == 2);
                cpdetails.CollectionPoint3 = cpdetails.CollectionPoints.Single(x => x.Id == 3);
                cpdetails.CollectionPoint4 = cpdetails.CollectionPoints.Single(x => x.Id == 4);
                cpdetails.CollectionPoint5 = cpdetails.CollectionPoints.Single(x => x.Id == 5);
                cpdetails.CollectionPoint6 = cpdetails.CollectionPoints.Single(x => x.Id == 6);
                return View(cpdetails);
            }
            return RedirectToAction("Index", "Login");
        }

        [Authorizer]
        [HttpPost]
        public ActionResult AssignCollectionPoint(DepartmentCollectionPointDTO cpdetails)
        {
            if (Session["existinguser"] != null)
            {
                LoginDTO currentUser = (LoginDTO)Session["existinguser"];
                if (currentUser.RoleId != (int)Enums.Roles.StoreManager)
                {
                    return RedirectToAction("RedirectToClerkOrDepartmentView", currentUser);
                }
                Department department = CollectionPointService.Instance.GetDepartmentByEmployeeId(currentUser.EmployeeId);
                department.CollectionPointId = cpdetails.DepartmentCollectionPointId;
                CollectionPointService.Instance.UpdateDepartmentCollectionPoint(department);

                return RedirectToAction("AssignCollectionPoint", "CollectionPoint");
            }
            return RedirectToAction("Index", "Login");
        }

        //Assign Clerks CollectionPoints
        [Authorizer]
        public ActionResult AssignClerk()
        {
            if (Session["existinguser"] != null)
            {
                LoginDTO currentUser = (LoginDTO)Session["existinguser"];
                if (currentUser.RoleId != (int)Enums.Roles.StoreManager)
                {
                    return RedirectToAction("RedirectToClerkOrDepartmentView", currentUser);
                }
                ClerkCollectionPointDTO cpdetails = new ClerkCollectionPointDTO();

                cpdetails.Clerks = CollectionPointService.Instance.GetAllClerks();
                cpdetails.CollectionPoints = CollectionPointService.Instance.GetAllCollectionPoints();
                cpdetails.CollectionPoint1 = cpdetails.CollectionPoints.Single(x => x.Id == 1);
                cpdetails.CollectionPoint2 = cpdetails.CollectionPoints.Single(x => x.Id == 2);
                cpdetails.CollectionPoint3 = cpdetails.CollectionPoints.Single(x => x.Id == 3);
                cpdetails.CollectionPoint4 = cpdetails.CollectionPoints.Single(x => x.Id == 4);
                cpdetails.CollectionPoint5 = cpdetails.CollectionPoints.Single(x => x.Id == 5);
                cpdetails.CollectionPoint6 = cpdetails.CollectionPoints.Single(x => x.Id == 6);
                return View(cpdetails);
            }
            return RedirectToAction("Index", "Login");
        }

        [Authorizer]
        [HttpPost]
        public ActionResult AssignClerk(ClerkCollectionPointDTO cpdetails)
        {
            if (Session["existinguser"] != null)
            {
                LoginDTO currentUser = (LoginDTO)Session["existinguser"];
                if (currentUser.RoleId != (int)Enums.Roles.StoreManager)
                {
                    return RedirectToAction("RedirectToClerkOrDepartmentView", currentUser);
                }
                IEnumerable<CollectionPoint> collectionPoints = CollectionPointService.Instance.GetAllCollectionPoints();
                CollectionPoint cp1 = collectionPoints.Single(x => x.Id == 1);
                cp1.EmployeeId = cpdetails.Employee1;
                CollectionPointService.Instance.UpdateCollectionPoint(cp1);

                CollectionPoint cp2 = collectionPoints.Single(x => x.Id == 2);
                cp2.EmployeeId = cpdetails.Employee2;
                CollectionPointService.Instance.UpdateCollectionPoint(cp2);

                CollectionPoint cp3 = collectionPoints.Single(x => x.Id == 3);
                cp3.EmployeeId = cpdetails.Employee3;
                CollectionPointService.Instance.UpdateCollectionPoint(cp3);

                CollectionPoint cp4 = collectionPoints.Single(x => x.Id == 4);
                cp4.EmployeeId = cpdetails.Employee4;
                CollectionPointService.Instance.UpdateCollectionPoint(cp4);

                CollectionPoint cp5 = collectionPoints.Single(x => x.Id == 5);
                cp5.EmployeeId = cpdetails.Employee5;
                CollectionPointService.Instance.UpdateCollectionPoint(cp5);

                CollectionPoint cp6 = collectionPoints.Single(x => x.Id == 6);
                cp6.EmployeeId = cpdetails.Employee6;
                CollectionPointService.Instance.UpdateCollectionPoint(cp6);


                return RedirectToAction("AssignClerk", "CollectionPoint");
            }
            return RedirectToAction("Index", "Login");
        }

    }
}
