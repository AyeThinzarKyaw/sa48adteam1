using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LUSSIS.Models;
using LUSSIS.Models.DTOs;

namespace LUSSIS.Services.Interfaces
{
    public interface IRetrievalService
    {
        RetrievalDTO constructRetrievalDTO(LoginDTO loginDTO);
    }
}
