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
        List<DisbursementDetailsDTO> GetDepRepDisbursementsDetails(int EmployeeId);
        List<DisbursementDetailsDTO> GetClerkDisbursementsDetails(int EmployeeId);
        Models.MobileDTOs.DisbursementListDTO GetDeptRepDisbursements(int EmployeeId);
        Models.MobileDTOs.DisbursementListDTO GetClerkDisbursements(int EmployeeId);
    }
}
