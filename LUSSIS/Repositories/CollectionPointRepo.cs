using LUSSIS.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LUSSIS.Repositories
{
    public class CollectionPointRepo : GenericRepo<CollectionPointRepo, int> , ICollectionPointRepo
    {
        private CollectionPointRepo() { }

        private static CollectionPointRepo instance = new CollectionPointRepo();
        public static ICollectionPointRepo Instance
        {
            get { return instance; }
        }
    }
}