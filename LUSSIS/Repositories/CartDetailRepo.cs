using LUSSIS.Models;
using LUSSIS.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LUSSIS.Repositories
{
    public class CartDetailRepo : GenericRepo<CartDetail, int>, ICartDetailRepo
    {
        private CartDetailRepo() { }

        private static CartDetailRepo instance = new CartDetailRepo();
        public static ICartDetailRepo Instance
        {
            get { return instance; }
        }

        public int GetCountOnHoldForStationery(int stationeryId)
        {
            Context.CartDetail_releaseCartData();//Note:release cart detail which are more than 1hour existed by last item of an employee
            if (Context.CartDetails.Any(x => x.StationeryId == stationeryId))
            {
                return (int)(from cd in Context.CartDetails
                             where cd.StationeryId == stationeryId
                             select cd.Quantity).Sum();
            }
            else
            {
                return 0;
            }
                   
        }

        public int GetFrontOfQueueCartCountForStationery(int stationeryId, DateTime datetime)
        {
            Context.CartDetail_releaseCartData();//Note:release cart detail which are more than 1hour existed by last item of an employee
            if (Context.CartDetails.Any(x => x.StationeryId == stationeryId && x.DateTime < datetime))
            {
                return (int)(from cd in Context.CartDetails
                             where cd.StationeryId == stationeryId
                             where cd.DateTime < datetime
                             select cd.Quantity).Sum();
            }
            else
            {
                return 0;
            }

        }

        public bool AnyItemInCartByEmployeeId(int employeeId)
        {
            Context.CartDetail_releaseCartData();//Note:release cart detail which are more than 1hour existed by last item of an employee
            return Context.CartDetails.Any(x => x.EmployeeId == employeeId);           
        }
    }
}