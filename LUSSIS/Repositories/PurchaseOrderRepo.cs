﻿using LUSSIS.Models;
using LUSSIS.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LUSSIS.Repositories
{
    public class PurchaseOrderRepo : GenericRepo<PurchaseOrder, int>, IPurchaseOrderRepo
    {
        private PurchaseOrderRepo() { }

        private static PurchaseOrderRepo instance = new PurchaseOrderRepo();
        public static IPurchaseOrderRepo Instance
        {
            get { return instance; }
        }

        public IEnumerable<PurchaseOrder> GetPurchaseOrderByStationeryId(int stationeryId)
        {
           var result = from po in Context.PurchaseOrders
                        join pod in Context.PurchaseOrderDetails on po.Id equals pod.PurchaseOrderId
                        join s in Context.Stationeries on pod.StationeryId equals s.Id
                        where s.Id == stationeryId
                        select po;
            return result.ToList();
        }
    }
}