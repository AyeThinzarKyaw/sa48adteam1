﻿using LUSSIS.Models;
using LUSSIS.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LUSSIS.Repositories
{
    public class DepartmentCoverEmployeeRepo : GenericRepo<DepartmentCoverEmployee, int>, IDepartmentCoverEmployeeRepo
    {
        public DepartmentCoverEmployee GetDepartmentCoverEmployeeByDepartmentIdAndDate(int departmentId, DateTime date)
        {
            var result = from d in Context.DepartmentCoverEmployees
                         join e in Context.Employees on d.EmployeeId equals e.Id
                         where e.DepartmentId == departmentId
                         where date >= d.FromDate
                         where date <= d.ToDate
                         select d;
            return result.SingleOrDefault<DepartmentCoverEmployee>();
        }

        public IEnumerable<DepartmentCoverEmployee> GetDepartmentCoverEmployeesByDepartmentId(int departmentId)
        {
            var result = from d in Context.DepartmentCoverEmployees
                         join e in Context.Employees on d.EmployeeId equals e.Id
                         where e.DepartmentId == departmentId
                         select d;
            return result.ToList();
        }
    }
}