using LUSSIS.Models;
using LUSSIS.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LUSSIS.Repositories
{
    public class SessionRepo : GenericRepo<Session, int>, ISessionRepo
    {
        private SessionRepo() { }

        private static SessionRepo instance = new SessionRepo();

        public static ISessionRepo Instance
        {
            get { return instance; }
        }
    }
}