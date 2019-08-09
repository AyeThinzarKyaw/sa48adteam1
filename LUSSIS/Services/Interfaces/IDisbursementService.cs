using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LUSSIS.Models;
using LUSSIS.Models.DTOs;

namespace LUSSIS.Services.Interfaces
{
    public interface IDisbursementService
    {
        List<DisbursementListDTO> GetDepRepDisbursementsDetails(int EmployeeId);
        List<DisbursementListDTO> GetClerkDisbursementsDetails(int EmployeeId);
    }
}
