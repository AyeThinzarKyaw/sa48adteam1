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
        // GET: AssignDeptRep
        public ActionResult AssignDeptRep(LoginDTO loginDTO)
        {
            int eId = loginDTO.EmployeeId;
            AssignDeptRepDTO assignstaff = new AssignDeptRepDTO();
            assignstaff.LoginDTO = loginDTO;
            //to change 1 to eId
            Employee deptrep = AssignStaffService.Instance.GetDeptRep(1);
            assignstaff.DeptRep = deptrep;
            assignstaff.StaffAndDeptRep = AssignStaffService.Instance.GetAllStaffAndRepInDept(deptrep.DepartmentId);
            return View(assignstaff);
        }

        [HttpPost]
        public ActionResult AssignDeptRep(AssignDeptRepDTO assignstaff)
        {
            //int eId = assignstaff.LoginDTO.EmployeeId;
            //to change 1 to eId
            Employee oldrep = AssignStaffService.Instance.GetDeptRep(1);
            AssignStaffService.Instance.UpdateStaff(oldrep);
            Employee newrep = AssignStaffService.Instance.GetStaff(assignstaff.NewDeptRepId);
            AssignStaffService.Instance.UpdateDeptRep(newrep);

            return RedirectToAction("AssignDeptRep", "AssignStaff");
        }

        // GET: AssignCoverStaff
        public ActionResult AssignCoverStaff(LoginDTO loginDTO)
        {
            int eId = loginDTO.EmployeeId;
            AssignCoverDTO assignstaff = new AssignCoverDTO();
            assignstaff.LoginDTO = loginDTO;
            //to change 1 to eId
            Employee e = AssignStaffService.Instance.GetStaff(1);
            assignstaff.StaffAndCoverHead = AssignStaffService.Instance.GetAllStaffAndCoverHeadInDept(e.DepartmentId);
            assignstaff.ActiveCoverHeadDetails = AssignStaffService.Instance.GetCurrentDepartmentCoverEmployeesByDepartmentId(e.DepartmentId);
            
            return View(assignstaff);
        }

        [HttpPost]
        public ActionResult AssignCoverStaff(AssignCoverDTO assignstaff)
        {
            IEnumerable<DepartmentCoverEmployee> existing = AssignStaffService.Instance.GetExistingDepartmentCoverEmployeesWithinDateRange(assignstaff.FromDate, assignstaff.ToDate);
            DepartmentCoverEmployee newcoverdetails = generateCoverEmployeeDetails(assignstaff);

           //int eId = assignstaff.LoginDTO.EmployeeId;
            //to change 1 to eId
            Employee e = AssignStaffService.Instance.GetStaff(1);
            assignstaff.StaffAndCoverHead = AssignStaffService.Instance.GetAllStaffAndCoverHeadInDept(e.DepartmentId);
            assignstaff.ActiveCoverHeadDetails = AssignStaffService.Instance.GetCurrentDepartmentCoverEmployeesByDepartmentId(e.DepartmentId);
            

            if (existing.Count() > 0 || assignstaff.ToDate < assignstaff.FromDate || assignstaff.FromDate < DateTime.Now || assignstaff.ToDate < DateTime.Now || assignstaff.ToDate == assignstaff.FromDate)
            {
                assignstaff.Error = new ErrorDTO();
                assignstaff.Error.HasError = true;
                assignstaff.Error.Message = "";

                if (assignstaff.ToDate < assignstaff.FromDate || assignstaff.FromDate < DateTime.Now || assignstaff.ToDate < DateTime.Now || assignstaff.ToDate == assignstaff.FromDate)
                {
                    assignstaff.Error.Message += "Valid From and To Dates required. ";
                }
                if (existing.Count() > 0 && assignstaff.ToDate != assignstaff.FromDate)
                {
                    assignstaff.Error.Message += "There is already a cover staff assigned within this date range.";
                }


                return View(assignstaff);
            }

            else
            {
                AssignStaffService.Instance.CreateDepartmentCoverEmployee(newcoverdetails);

                return RedirectToAction("AssignCoverStaff", "AssignStaff");

            }


        }

        public ActionResult CancelCoverStaff(int coverId)
        {
            DepartmentCoverEmployee cover = AssignStaffService.Instance.GetDepartmentCoverEmployeeById(coverId);
            AssignStaffService.Instance.CancelDepartmentCoverEmployee(cover);

            return RedirectToAction("AssignCoverStaff", "AssignStaff");
        }

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