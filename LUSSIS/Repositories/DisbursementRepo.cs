﻿using LUSSIS.Models;
using LUSSIS.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LUSSIS.Repositories
{
    public class DisbursementRepo : GenericRepo<Disbursement, int>, IDisbursementRepo
    {
        private DisbursementRepo() { }

        private static DisbursementRepo instance = new DisbursementRepo();

        public static IDisbursementRepo Instance
        {
            get { return instance; }
        }

        public IEnumerable<Disbursement> GetDisbursementsByDepartmentId(int depId)
        {
            var result = from d in Context.Disbursements
                         join rd in Context.RequisitionDetails on d.Id equals rd.DisbursementId
                         join r in Context.Requisitions on rd.RequisitionId equals r.Id
                         join e in Context.Employees on r.EmployeeId equals e.Id
                         join dep in Context.Departments on e.DepartmentId equals dep.Id
                         where dep.Id == depId
                         select d;
            return result.ToList();
        }

        public IEnumerable<Disbursement> GetDisbursementsByDeptRepId(int deptRepId)
        {
            var result = from d in Context.Disbursements
                         where d.ReceivedEmployeeId == deptRepId
                         select d;
            return result.ToList();
        }

        public IEnumerable<Disbursement> GetDisbursementsByClerkId(int clerkId)
        {
            var result = from d in Context.Disbursements
                         where d.DeliveredEmployeeId == clerkId && d.Signature.Equals(null)
                         select d;
            return result.ToList();
        }
    }
}