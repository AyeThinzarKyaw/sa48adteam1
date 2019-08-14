using LUSSIS.Models;
using LUSSIS.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LUSSIS.Services.Interfaces
{
    public interface ILoginService
    {
        LoginDTO GetEmployeeLoginByUsernameAndPassword(string username, string password);
        Models.MobileDTOs.LoginDTO GetEmployeeLoginByUsernameAndPassword2(string username, string password);
        string HashPassword(string password);

        Session GetExistingSessionFromGUID(string GUID);

        void LogoutUser(string GUID);

    }
}
