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
    public class AssignStaffController : Controller
    {
        [Authorizer]
        // GET: AssignDeptRep
        public ActionResult AssignDeptRep()
        {
            if (Session["existinguser"] != null)
            {
                LoginDTO currentUser = (LoginDTO)Session["existinguser"];
                if (currentUser.RoleId != (int)Enums.Roles.DepartmentHead && currentUser.RoleId == (int)Enums.Roles.DepartmentCoverHead)
                {
                    return RedirectToAction("RedirectToClerkOrDepartmentView", "Login");
                }

                int eId = currentUser.EmployeeId;
                AssignDeptRepDTO assignstaff = new AssignDeptRepDTO();
                //assignstaff.LoginDTO = loginDTO;
                //to change 1 to eId
                Employee deptrep = AssignStaffService.Instance.GetDeptRep(eId);
                assignstaff.DeptRep = deptrep;
                assignstaff.StaffAndDeptRep = AssignStaffService.Instance.GetAllStaffAndRepInDept(deptrep.DepartmentId);
                return View(assignstaff);
            }
            return RedirectToAction("Index", "Login");
        }

        [HttpPost]
        public ActionResult AssignDeptRep(AssignDeptRepDTO assignstaff)
        {
            if (Session["existinguser"] != null)
            {
                LoginDTO currentUser = (LoginDTO)Session["existinguser"];
                if (currentUser.RoleId != (int)Enums.Roles.DepartmentHead || currentUser.RoleId==(int)Enums.Roles.DepartmentCoverHead)
                {
                    return RedirectToAction("RedirectToClerkOrDepartmentView", "Login");
                }
                int eId = currentUser.EmployeeId;
                Employee oldrep = AssignStaffService.Instance.GetDeptRep(eId);
                AssignStaffService.Instance.UpdateStaff(oldrep);
                Employee newrep = AssignStaffService.Instance.GetStaff(assignstaff.NewDeptRepId);
                AssignStaffService.Instance.UpdateDeptRep(newrep);

                return RedirectToAction("AssignDeptRep", "AssignStaff");
            }
            return RedirectToAction("Index", "Login");
        }

        [Authorizer]
        // GET: AssignCoverStaff
        public ActionResult AssignCoverStaff()
        {
            if (Session["existinguser"] != null)
            {
                LoginDTO currentUser = (LoginDTO)Session["existinguser"];
                if (currentUser.RoleId != (int)Enums.Roles.DepartmentHead)
                {
                    return RedirectToAction("RedirectToClerkOrDepartmentView", "Login");
                }
                int eId = currentUser.EmployeeId;
                AssignCoverDTO assignstaff = new AssignCoverDTO();

                Employee e = AssignStaffService.Instance.GetStaff(eId);
                assignstaff.StaffAndCoverHead = AssignStaffService.Instance.GetAllStaffAndCoverHeadInDept(e.DepartmentId);
                assignstaff.ActiveCoverHeadDetails = AssignStaffService.Instance.GetCurrentDepartmentCoverEmployeesByDepartmentId(e.DepartmentId);

                return View(assignstaff);
            }
            return RedirectToAction("Index", "Login");
        }

        [HttpPost]
        public ActionResult AssignCoverStaff(AssignCoverDTO assignstaff)
        {
            if (Session["existinguser"] != null)
            {
                LoginDTO currentUser = (LoginDTO)Session["existinguser"];
                if (currentUser.RoleId != (int)Enums.Roles.DepartmentHead)
                {
                    return RedirectToAction("RedirectToClerkOrDepartmentView", "Login");
                }
                IEnumerable<DepartmentCoverEmployee> existing = AssignStaffService.Instance.GetExistingDepartmentCoverEmployeesWithinDateRange(assignstaff.FromDate, assignstaff.ToDate);
                DepartmentCoverEmployee newcoverdetails = generateCoverEmployeeDetails(assignstaff);

                Employee e = AssignStaffService.Instance.GetStaff(currentUser.EmployeeId);
                assignstaff.StaffAndCoverHead = AssignStaffService.Instance.GetAllStaffAndCoverHeadInDept(e.DepartmentId);
                assignstaff.ActiveCoverHeadDetails = AssignStaffService.Instance.GetCurrentDepartmentCoverEmployeesByDepartmentId(e.DepartmentId);

                //assuming Head can only assign earliest fromdate from next day
                if (existing.Count() > 0 || assignstaff.ToDate < assignstaff.FromDate || assignstaff.FromDate < DateTime.Now || assignstaff.ToDate < DateTime.Now)
                {
                    assignstaff.Error = new ErrorDTO();
                    assignstaff.Error.HasError = true;
                    assignstaff.Error.Message = "";

                    if (assignstaff.ToDate < assignstaff.FromDate || assignstaff.FromDate < DateTime.Now || assignstaff.ToDate < DateTime.Now)
                    {
                        assignstaff.Error.Message += "Valid From and To Dates required. ";
                    }
                    if (existing.Count() > 0 && assignstaff.ToDate != assignstaff.FromDate && assignstaff.FromDate < assignstaff.ToDate && assignstaff.FromDate > DateTime.Now)
                    {
                        assignstaff.Error.Message += "There is already a cover staff assigned within this date range.";
                    }
                    return View(assignstaff);
                }
                else
                {
                    AssignStaffService.Instance.CreateDepartmentCoverEmployee(newcoverdetails);
                    newcoverdetails.Employee = AssignStaffService.Instance.GetStaff(newcoverdetails.EmployeeId);
                    assignstaff.StaffAndCoverHead = AssignStaffService.Instance.GetAllStaffAndCoverHeadInDept(e.DepartmentId);
                    assignstaff.ActiveCoverHeadDetails = AssignStaffService.Instance.GetCurrentDepartmentCoverEmployeesByDepartmentId(e.DepartmentId);

                    return View(assignstaff);
                }
            }
            return RedirectToAction("Index", "Login");
        }

        [Authorizer]
        public ActionResult CancelCoverStaff(int coverId)
        {
            if (Session["existinguser"] != null)
            {
                LoginDTO currentUser = (LoginDTO)Session["existinguser"];
                if (currentUser.RoleId != (int)Enums.Roles.DepartmentHead)
                {
                    return RedirectToAction("RedirectToClerkOrDepartmentView", "Login");
                }
                DepartmentCoverEmployee cover = AssignStaffService.Instance.GetDepartmentCoverEmployeeById(coverId);
                AssignStaffService.Instance.CancelDepartmentCoverEmployee(cover);

                return RedirectToAction("AssignCoverStaff", "AssignStaff");
            }
            return RedirectToAction("Index", "Login");
        }
        [Authorizer]
        private DepartmentCoverEmployee generateCoverEmployeeDetails(AssignCoverDTO assignstaff)
        {
            DepartmentCoverEmployee coverdetails = new DepartmentCoverEmployee();
            coverdetails.EmployeeId = assignstaff.NewCoverHeadId;
            coverdetails.FromDate = assignstaff.FromDate;
            coverdetails.ToDate = assignstaff.ToDate;
            coverdetails.Status = Enum.GetName(typeof(Enums.ActiveStatus), Enums.ActiveStatus.ACTIVE);

            return coverdetails;
        }
    }
}