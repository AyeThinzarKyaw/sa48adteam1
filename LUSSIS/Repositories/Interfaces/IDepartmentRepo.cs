﻿using LUSSIS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LUSSIS.Repositories.Interfaces
{
    public interface IDepartmentRepo : IGenericRepo<Department, int>
    {
        Department GetDepartmentByEmployeeId(int employeeId);
    }
}