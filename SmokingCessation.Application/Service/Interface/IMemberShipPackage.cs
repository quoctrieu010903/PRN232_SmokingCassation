using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmokingCessation.Application.DTOs.Fillter;
using SmokingCessation.Application.DTOs.Request;
using SmokingCessation.Application.DTOs.Response;
using SmokingCessation.Core.Response;
using SmokingCessation.Domain.Specifications;

namespace SmokingCessation.Application.Service.Interface
{
    public interface IMemberShipPackage
    {
        Task<PaginatedList<MemberShipPackageResponse>> getAllPackage(PagingRequestModel model, MemberShipPackageFillter fillter); 
        Task<BaseResponseModel> getPackageById (Guid id);
        Task<BaseResponseModel> Create(MemberShipPackageRequest request);
        Task<BaseResponseModel> Update(MemberShipPackageRequest request, Guid id);
        Task<BaseResponseModel> Delete(Guid id);
    }
}
