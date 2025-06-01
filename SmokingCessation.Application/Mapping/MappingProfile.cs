


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
            // In your AutoMapper profile
            CreateMap<MembershipPackage, MemberShipPackageResponse>();
                




            #endregion
        }
    }
}
