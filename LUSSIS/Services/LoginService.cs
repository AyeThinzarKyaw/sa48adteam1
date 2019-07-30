using LUSSIS.Models;
using LUSSIS.Models.DTOs;
using LUSSIS.Repositories.Interfaces;
using LUSSIS.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LUSSIS.Services
{
    public class LoginService : ILoginService
    {
        private IEmployeeRepo employeeRepo;
        private ISessionRepo sessionRepo;
        private ICartDetailRepo cartDetailRepo;

        public LoginService(IEmployeeRepo employeeRepo, ISessionRepo sessionRepo, ICartDetailRepo cartDetailRepo)
        {
            this.employeeRepo = employeeRepo;
            this.sessionRepo = sessionRepo;
            this.cartDetailRepo = cartDetailRepo;
        }

        public LoginDTO GetEmployeeLoginByUsernameAndPassword(string username, string password)
        {
            //return null if not valid employee
            //else return a loginDTO with required details
            Employee e = employeeRepo.FindBy(x => x.Username == username && x.Password == password).SingleOrDefault();
            if (e == null) // no such user
            {
                return null;
            }
            else
            {
                LoginDTO logignDTO = new LoginDTO();
                Session newSession = new Session();
                sessionRepo.Create(newSession);
                //set attributes
                List<CartDetail> existingCartDetails = cartDetailRepo.FindBy(x => x.Employee.Equals(e)).ToList();
                if(existingCartDetails != null)
                {
                    //add list to DTO
                }

                return logignDTO;
            }
        }

    }

}