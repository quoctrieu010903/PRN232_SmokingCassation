


using AutoMapper;
using SmokingCessation.Application.DTOs.Request;
using SmokingCessation.Application.DTOs.Response;
using SmokingCessation.Domain.Entities;
using static SmokingCessation.Application.DTOs.Request.AuthenticationRequest;
using static SmokingCessation.Application.DTOs.Response.AuthenticationResponse;

namespace SmokingCessation.Application.Mapping
{
    public class MappingProfile  : Profile
    {
        public MappingProfile()
        {
            CreateMap<ApplicationUser, UserResponse>();
            CreateMap<ApplicationUser, CurrentUserResponse>();
            CreateMap<UserRegisterRequest, ApplicationUser>();

            #region MemberShipPackage 

            CreateMap<MembershipPackage, MemberShipPackageRequest>();
            CreateMap<MembershipPackage, MemberShipPackageResponse>()
                    .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Type.ToString()));

            CreateMap<MemberShipPackageRequest, MembershipPackage>();

            CreateMap<MembershipPackage, MemberShipPackageResponse>();

            #endregion

            #region QuitPlant
            CreateMap<QuitPlan, QuitPlansRequest>();
          


            CreateMap<QuitPlansRequest, QuitPlan>();

            CreateMap<QuitPlan, QuitPlanResponse>()
                //.ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User.UserName))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()));
            //.ForMember(dest => dest.PackageName, opt => opt.MapFrom(src => src.MembershipPackage.Name)).ReverseMap();


            #endregion

            #region ProgressLogs
            CreateMap<ProgressLogsRequest, ProgressLog>();
            CreateMap<ProgressLog, ProgressLogsResponse>()
                .ForMember(dest => dest.QuitPlanName, opt => opt.MapFrom(src => src.QuitPlan.Reason));
            #endregion

        }
    }
}
