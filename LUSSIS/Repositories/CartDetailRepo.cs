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
    }
}