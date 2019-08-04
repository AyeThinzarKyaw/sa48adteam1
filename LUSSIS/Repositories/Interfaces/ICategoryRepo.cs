using LUSSIS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LUSSIS.Repositories.Interfaces
{
    public interface ICategoryRepo: IGenericRepo<Category, int>
    {
        String getCategoryType(int categoryId);
    }
}