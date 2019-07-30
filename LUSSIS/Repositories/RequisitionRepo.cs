using LUSSIS.Models;
using LUSSIS.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LUSSIS.Repositories
{
    public class RequisitionRepo : GenericRepo<Requisition, int> , IRequisitionRepo
    {
        private RequisitionRepo() { }

        private static RequisitionRepo instance = new RequisitionRepo();
        public static IRequisitionRepo Instance
        {
            get { return instance; }
        }
    }
}