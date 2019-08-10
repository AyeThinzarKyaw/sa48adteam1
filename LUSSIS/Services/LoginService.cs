using LUSSIS.Models;
using LUSSIS.Models.DTOs;
using LUSSIS.Repositories;
using LUSSIS.Repositories.Interfaces;
using LUSSIS.Services.Interfaces;
using LUSSIS.Util;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Security.Cryptography;
using System.Web;

namespace LUSSIS.Services
{
    public class LoginService : ILoginService
    {
        private IEmployeeRepo employeeRepo;
        private ISessionRepo sessionRepo;
        private IDepartmentCoverEmployeeRepo departmentCoverEmployeeRepo;
        private static LoginService instance = new LoginService();

        private LoginService()
        {
            employeeRepo = EmployeeRepo.Instance;
            sessionRepo = SessionRepo.Instance;
            departmentCoverEmployeeRepo = DepartmentCoverEmployeeRepo.Instance;
        }

        //returns single instance
        public static ILoginService Instance
        {
            get { return instance; }
        }

        public Session GetExistingSessionFromGUID(string GUID)
        {
            Session s = sessionRepo.FindOneBy(x => x.GUID.Equals(GUID)); // AND ALSO if logouttime == null right??
            if (s != null)
            {
                Employee e = employeeRepo.FindById(s.EmployeeId);
                s.Employee = e;
            }
            return s;
        }

        public LoginDTO GetEmployeeLoginByUsernameAndPassword(string username, string password)
        {
            //return null if not valid employee
            //else return a loginDTO with required details

            //Hash password first
            if (password == null)
                return null;

            string hashedPassword = HashPassword(password);

            Employee e = employeeRepo.FindBy(x => x.Username == username && x.Password == hashedPassword).SingleOrDefault();
            if (e == null) // no such user
            {
                return null;
            }
            else
            {                
                Session newSession = new Session() { EmployeeId = e.Id, GUID = Guid.NewGuid().ToString(), LogInDateTime = DateTime.Now };
                sessionRepo.Create(newSession);

                //check if cover head?
                int role = departmentCoverEmployeeRepo.FindOneBy(x => x.EmployeeId == e.Id && DbFunctions.TruncateTime(x.FromDate) <= DateTime.Now && DbFunctions.TruncateTime(x.ToDate) >= DateTime.Now) != null ? (int)Enums.Roles.DepartmentCoverHead : e.RoleId;

                //set attributes
                LoginDTO loginDTO = new LoginDTO() { EmployeeId = e.Id, RoleId = role, SessionGuid = newSession.GUID };
                return loginDTO;
            }
        }

        public string HashPassword(string password)
        {
            using (MD5 md5Hash = MD5.Create())
            {
                return MD5Hash.GetMd5Hash(md5Hash, password);
            }
        }

        public void LogoutUser(string GUID)
        {
            Session s = sessionRepo.FindOneBy(x=>x.GUID.Equals(GUID));
            s.LogOutDateTime = DateTime.Now;
            sessionRepo.Update(s);

        }
    }

}