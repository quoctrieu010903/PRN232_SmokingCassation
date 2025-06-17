using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmokingCessation.Application.DTOs.Request;
using SmokingCessation.Core.Response;

namespace SmokingCessation.Application.Service.Interface
{
    public interface IUserPackageService
    {
        Task<BaseResponseModel> RegisterPackage(UserPackageRequest request);
        Task<BaseResponseModel> GetCurrentPackage();
        Task<BaseResponseModel> CancelCurrentPackage();
        Task<BaseResponseModel> GetPackageHistory();
        Task<BaseResponseModel> GetPackageById(Guid id);

    }
}
