


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
            CreateMap<ApplicationUser, UserResponse>()
                .ForMember(dest => dest.UserImage ,opt => opt.MapFrom(src => src.ImageUrl));
            CreateMap<ApplicationUser, CurrentUserResponse>()
                .ForMember(dest => dest.UserImage ,opt => opt.MapFrom(src => src.ImageUrl)); 
            CreateMap<ApplicationUser, UserCurrenResponse>()
                .ForMember(dest => dest.ImageUrl ,opt => opt.MapFrom(src => src.ImageUrl)).ReverseMap();
            
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
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
                .ForMember(dest => dest.PackageName, opt => opt.MapFrom(src => src.MembershipPackage.Name.ToString()));
                
            //.ForMember(dest => dest.PackageName, opt => opt.MapFrom(src => src.MembershipPackage.Name)).ReverseMap();


            #endregion

            #region ProgressLogs
            CreateMap<ProgressLogsRequest, ProgressLog>();
            CreateMap<ProgressLog, ProgressLogsResponse>()
                .ForMember(dest => dest.QuitPlanName, opt => opt.MapFrom(src => src.QuitPlan.Reason));
            #endregion

            #region Blog 
            CreateMap<BlogRequest, Blog>();
            CreateMap<Blog, BlogResponse>()
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
                .ForMember(dest => dest.AuthorName, opt => opt.MapFrom(src => src.Author.FullName))
                .ForMember(dest => dest.CommentsCount, opt => opt.MapFrom(src => src.Feedbacks.Count))
                .ForMember(dest => dest.Views , opt => opt.MapFrom(src => src.ViewCount));


            #endregion

            #region Feedback
            CreateMap<Feedback, FeedbackRequest>();
            CreateMap<Feedback, FeedbackResponse>()
                .ForMember(dest => dest.BlogTitle, opt => opt.MapFrom(src => src.Blog.Title))
                .ForMember(dest => dest.UserImage, opt => opt.MapFrom(src => src.User.ImageUrl))
                .ForMember(dest => dest.UserName , opt => opt.MapFrom(src => src.User.UserName));
            #endregion

            #region Rating mapper
            CreateMap<RatingRequest, Rating>();
            CreateMap<Rating, RatingResponse>()
                .ForMember(dest => dest.BlogTitle, opt => opt.MapFrom(src => src.Blog.Title))
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User.FullName))
                .ForMember(dest => dest.Value , opt => opt.MapFrom(src => src.Start));
            #endregion

            #region User Package 
                CreateMap<UserPackage, UserPackageResponse>()
               .ForMember(dest => dest.PackageId, opt => opt.MapFrom(src => src.PackageId))
               .ForMember(dest => dest.PackageName, opt => opt.MapFrom(src => src.Package != null ? src.Package.Name : string.Empty))
               .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.Package != null ? src.Package.Price : 0))
               .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Package != null ? src.Package.Type.ToString() : string.Empty))
               .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Package != null ? src.Package.Description : string.Empty))
               .ForMember(dest => dest.Features, opt => opt.MapFrom(src => src.Package != null ? src.Package.Features : null))
               .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.User != null ? src.User.FullName : string.Empty))
             // Additional mapping
                .ForMember(dest => dest.RemainingDays, opt => opt.MapFrom(src => (src.EndDate - DateTimeOffset.UtcNow).Days > 0 ? (src.EndDate - DateTimeOffset.UtcNow).Days : 0))
                .ForMember(dest => dest.IsExpired, opt => opt.MapFrom(src => src.EndDate < DateTimeOffset.UtcNow));

            #endregion

            #region CoachAdvice Log
                CreateMap<CoachAdviceLog, CoachAdviceLogResponse>().ReverseMap();
            #endregion

            #region Achivement 
                CreateMap<AchievementCreateRequest , Achievement>().ReverseMap();
                CreateMap<AchievementUpdateRequest, Achievement>().ReverseMap();
                CreateMap<AchievementResponse, Achievement>().ReverseMap();
            #endregion


        }
    }
}
