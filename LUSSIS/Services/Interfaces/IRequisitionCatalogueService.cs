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

        CatalogueItemDTO AddCartDetail(CatalogueItemDTO catalogueItemDTO);

        void RemoveCartDetail(CatalogueItemDTO catalogueItemDTO);

        CatalogueItemDTO UpdateCartDetail(CatalogueItemDTO catalogueItemDTO);
    }
}
