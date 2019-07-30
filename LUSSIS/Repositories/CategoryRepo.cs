using LUSSIS.Models;
using LUSSIS.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LUSSIS.Repositories
{
    public class CategoryRepo : GenericRepo<Category, int>, ICategoryRepo
    {
        private CategoryRepo() { }

        private static CategoryRepo instance = new CategoryRepo();
        public static ICategoryRepo Instance
        {
            get { return instance; }
        }
    }
}