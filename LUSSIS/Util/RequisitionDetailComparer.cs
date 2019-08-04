using LUSSIS.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LUSSIS.Util
{
    public class RequisitionDetailComparer : IComparer<RequisitionDetail>
    {
        public int Compare(RequisitionDetail x, RequisitionDetail y)
        {
            if(x.Requisition.DateTime < y.Requisition.DateTime)
            {
                return -1;
            }
            else if (x.Requisition.DateTime > y.Requisition.DateTime)
            {
                return 1;
            }
            else
            {
                return 0;
            }

        }
    }
}