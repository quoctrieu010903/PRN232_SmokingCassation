using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmokingCessation.Application.DTOs.Request;
using SmokingCessation.Application.DTOs.Response;
using SmokingCessation.Core.Response;

namespace SmokingCessation.Application.Service.Interface
{
    public interface IUserPackageService
    {
        Task<BaseResponseModel<VNPayReturnLink>> RegisterPackage(UserPackageRequest request);
        Task<BaseResponseModel<UserPackageResponse>> GetCurrentPackage();
        Task<BaseResponseModel<UserPackageResponse>> CancelCurrentPackage();
        Task<BaseResponseModel<List<UserPackageResponse>>> GetPackageHistory();
        Task<BaseResponseModel<UserPackageResponse>> GetPackageById(Guid id);


    }
}
