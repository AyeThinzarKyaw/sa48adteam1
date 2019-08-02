using LUSSIS.Models;
using LUSSIS.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LUSSIS.Services.Interfaces
{
    public interface IRequisitionCatalogueService
    {

        List<CatalogueItemDTO> GetCatalogueItems(int employeeId);

        CatalogueItemDTO AddCartDetail(int employeeId, int stationeryId, int inputQty);

        CatalogueItemDTO RemoveCartDetail(int employeeId, int stationeryId);

        CatalogueItemDTO UpdateCartDetail(int employeeId, int stationeryId, int inputQty);

        List<Requisition> GetPersonalRequisitionHistory(int employeeId);

        RequisitionDetailsDTO GetRequisitionDetailsForPersonalRequisition(int requisitionId, int employeeId);

        Requisition ConvertCartDetailsToRequisitionDetails(int employeeId);
    }
}
