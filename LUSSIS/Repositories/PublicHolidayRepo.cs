using LUSSIS.Models;
using LUSSIS.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LUSSIS.Repositories
{
    public class PublicHolidayRepo : GenericRepo<PublicHoliday, int>, IPublicHolidayRepo
    {
        private PublicHolidayRepo() { }

        private static PublicHolidayRepo instance = new PublicHolidayRepo();

        public static IPublicHolidayRepo Instance
        {
            get { return instance; }
        }
    }
}