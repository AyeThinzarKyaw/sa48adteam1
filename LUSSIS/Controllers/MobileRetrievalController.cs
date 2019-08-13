using LUSSIS.Models.MobileDTOs;
using LUSSIS.Services;
using LUSSIS.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace LUSSIS.Controllers
{
    public class MobileRetrievalController : ApiController
    {

        IRetrievalService retrievalService;    

        public MobileRetrievalController()
        {
            retrievalService = RetrievalService.Instance;
        }

        // GET: api/MobileRetrieval/5
        public RetrievalDTO Get(int id)
        {
            Models.DTOs.LoginDTO loginDTO = new Models.DTOs.LoginDTO
            {
                EmployeeId = id
            };
            Models.DTOs.RetrievalDTO tempModel = retrievalService.constructRetrievalDTO(loginDTO);
            RetrievalDTO model = new RetrievalDTO
            {
                LoginDTO = loginDTO,
                RetrievalDate = tempModel.RetrievalDate,
                RetrievalItem = new List<RetrievalItemDTO>()
            };

            foreach (Models.DTOs.RetrievalItemDTO ri in tempModel.RetrievalItem)
            {
                RetrievalItemDTO ri2 = new RetrievalItemDTO
                {
                    Description = ri.Description,
                    Location = ri.Location,
                    NeededQuantity = ri.NeededQuantity,
                    RetrievedQty = ri.RetrievedQty,
                    StationeryId = ri.StationeryId,
                    RetrievalPrepItemList = new List<RetrievalPrepItemDTO>()
            };

                foreach (Models.DTOs.RetrievalPrepItemDTO rpi in ri.RetrievalPrepItemList)
                {
                    RetrievalPrepItemDTO rpi2 = new RetrievalPrepItemDTO
                    {
                        RequisitionDetail = new RequisitionDetailDTO
                        {
                            Id = rpi.ReqDetail.Id,
                            RequisitionId = rpi.ReqDetail.RequisitionId
                        }
                    };

                    ri2.RetrievalPrepItemList.Add(rpi2);
                }

                model.RetrievalItem.Add(ri2);
            }

            return model;
        }

        // POST: api/MobileRetrieval
        public void Post([FromBody]RetrievalDTO mR)
        {
            Models.DTOs.RetrievalDTO r = new Models.DTOs.RetrievalDTO();
            r.LoginDTO = new Models.DTOs.LoginDTO
            {
                EmployeeId = mR.LoginDTO.EmployeeId
            };
            r.RetrievalDate = mR.RetrievalDate;
            r.RetrievalItem = new List<Models.DTOs.RetrievalItemDTO>();

            foreach(RetrievalItemDTO mRi in mR.RetrievalItem)
            {
                Models.DTOs.RetrievalItemDTO ri = new Models.DTOs.RetrievalItemDTO
                {
                    Description = mRi.Description,
                    Location = mRi.Location,
                    NeededQuantity = mRi.NeededQuantity,
                    RetrievedQty = mRi.RetrievedQty,
                    StationeryId = mRi.StationeryId,
                    RetrievalPrepItemList = new List<Models.DTOs.RetrievalPrepItemDTO>()
                };

                foreach (RetrievalPrepItemDTO mRpi in mRi.RetrievalPrepItemList)
                {
                    Models.DTOs.RetrievalPrepItemDTO rpi = new Models.DTOs.RetrievalPrepItemDTO
                    {
                        ReqDetail = new Models.RequisitionDetail
                        {
                            Id = mRpi.RequisitionDetail.Id,
                            RequisitionId = mRpi.RequisitionDetail.RequisitionId
                        }
                    };

                    ri.RetrievalPrepItemList.Add(rpi);
                }

                r.RetrievalItem.Add(ri);
            }

            retrievalService.completeRetrievalProcess(r,r.LoginDTO.EmployeeId);
        }

    }
}
