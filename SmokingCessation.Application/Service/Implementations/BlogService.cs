
using AutoMapper;
using Microsoft.AspNetCore.Http;
using SmokingCessation.Application.DTOs.Fillter;
using SmokingCessation.Application.DTOs.Request;
using SmokingCessation.Application.DTOs.Response;
using SmokingCessation.Application.Service.Interface;
using SmokingCessation.Core.Constants;
using SmokingCessation.Core.CustomExceptionss;
using SmokingCessation.Core.Response;
using SmokingCessation.Core.Utils;
using SmokingCessation.Domain.Entities;
using SmokingCessation.Domain.Enums;
using SmokingCessation.Domain.Interfaces;
using SmokingCessation.Domain.Specifications;

namespace SmokingCessation.Application.Service.Implementations
{
    public class BlogService : IBlogService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IUserContext _userContext;
        private readonly IPhotoService _photoService;

        public BlogService(IUnitOfWork unitOfWork, IMapper mapper, IUserContext userContext, IPhotoService photoService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _userContext = userContext;
            _photoService = photoService;
        }

        public async Task<BaseResponseModel> ChangeStatus(Guid id, BlogStatus status)
        {
            var repo = _unitOfWork.Repository<Blog, Guid>();
            var blog = await repo.GetByIdAsync(id);
            if (blog == null)
                throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, MessageConstants.NOT_FOUND);

            blog.Status = status;
         
           
            blog.LastUpdatedBy = _userContext.GetUserId();
            blog.LastUpdatedTime = CoreHelper.SystemTimeNow;
            await repo.UpdateAsync(blog);
            await _unitOfWork.SaveChangesAsync();
            return new BaseResponseModel(StatusCodes.Status200OK, ResponseCodeConstants.SUCCESS, MessageConstants.BLOG_STATUS_UPDATED);

        }

        public async Task<BaseResponseModel> Create(BlogRequest request)
        {
            string? imageUrl = null;

            if (request.FeaturedImage != null)
            {
                imageUrl = await _photoService.UploadPhotoAsync(request.FeaturedImage);
            }
            var userId = _userContext.GetUserId();
            var blog = _mapper.Map<Blog>(request);
            blog.AuthorId = Guid.Parse(userId);
            blog.Status = BlogStatus.PendingApproval;
            blog.FeaturedImageUrl = imageUrl;
            blog.PublishedDate = CoreHelper.SystemTimeNow;
            blog.CreatedTime = CoreHelper.SystemTimeNow;

            await _unitOfWork.Repository<Blog, Guid>().AddAsync(blog);
            await _unitOfWork.SaveChangesAsync();
            return new BaseResponseModel(StatusCodes.Status200OK, ResponseCodeConstants.SUCCESS, MessageConstants.BLOG_CREATE_PENDING_APPROVAL);

        }

        public async Task<BaseResponseModel> Delete(Guid id)
        {
            var userId = _userContext.GetUserId();
            var repo = _unitOfWork.Repository<Blog, Guid>();
            var blog = await repo.GetByIdAsync(id);
            if (blog == null)
                throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, MessageConstants.NOT_FOUND);

            blog.DeletedBy = userId;
            blog.DeletedTime = CoreHelper.SystemTimeNow;
            blog.LastUpdatedBy = userId;
            blog.LastUpdatedTime = CoreHelper.SystemTimeNow;
            await _unitOfWork.Repository<Blog, Guid>().UpdateAsync(blog);

            await _unitOfWork.SaveChangesAsync();
            return new BaseResponseModel(StatusCodes.Status200OK, ResponseCodeConstants.SUCCESS, MessageConstants.BLOG_DELETE_SUCCESS);

        }

        public async Task<PaginatedList<BlogResponse>> GetAll(PagingRequestModel model , BlogListFilter filter)
        {
            var blogs = await _unitOfWork.Repository<Blog, Blog>().GetAllWithIncludeAsync(true, p=>p.Ratings , p => p.Feedbacks);

            var currentUser = _userContext.GetCurrentUser();
            bool isAdmin = currentUser != null && currentUser.isInRole(UserRoles.Admin);

            if (!isAdmin)
            {
                // User thường chỉ xem blog đã duyệt
                blogs = blogs.Where(b => b.Status == BlogStatus.Published).ToList();
            }
            else
            {
                // Admin có thể filter theo status nếu truyền lên
                if (filter.Status.HasValue)
                    blogs = blogs.Where(b => b.Status == filter.Status.Value).ToList();
            }

            if (!string.IsNullOrWhiteSpace(filter.Search))
            {
                var searchLower = filter.Search.Trim().ToLower();
                blogs = blogs.Where(b =>
                    (!string.IsNullOrEmpty(b.Title) && b.Title.ToLower().Contains(searchLower)) ||
                    (!string.IsNullOrEmpty(b.Content) && b.Content.ToLower().Contains(searchLower)) ||
                    (b.Author != null && !string.IsNullOrEmpty(b.Author.FullName) && b.Author.FullName.ToLower().Contains(searchLower))
                ).ToList();
            }
            IEnumerable<Blog> sortedBlogs = filter.FilterType switch
            {
                BlogListFilterType.Popular => blogs.OrderByDescending(b => b.ViewCount),
                BlogListFilterType.Newest => blogs.OrderByDescending(b => b.PublishedDate).Take(5),
                BlogListFilterType.All or _ => blogs.OrderByDescending(b => b.PublishedDate)
            };
            // Tính average rating cho tất cả blog một lần
            var avgDict = sortedBlogs
                .Where(b => b.Ratings != null && b.Ratings.Any())
                .ToDictionary(
                    b => b.Id,
                    b => b.Ratings.Average(r => r.Start)
                );

            var result = _mapper.Map<List<BlogResponse>>(sortedBlogs.ToList());
       
            foreach (var blog in result)
            {
                blog.AverageRating = avgDict.TryGetValue(blog.Id, out var avg) ? avg : 0;
            
            }
            return PaginatedList<BlogResponse>.Create(result, model.PageNumber, model.PageSize);

        }

        public async Task<BaseResponseModel<BlogResponse>> GetBlogsDetails(Guid id)
        {
            var repo = _unitOfWork.Repository<Blog, Guid>();
            var blog = await repo.GetByIdAsync(id);
            if (blog == null)
                throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, MessageConstants.NOT_FOUND);


            var result = _mapper.Map<BlogResponse>(blog);
            return new BaseResponseModel<BlogResponse>(StatusCodes.Status200OK, ResponseCodeConstants.SUCCESS,result, MessageConstants.GETALL_SUCCESS);

        }

        public async Task<BaseResponseModel> IncreaseViewCount(Guid id)
        {
            var repo = _unitOfWork.Repository<Blog, Guid>();
            var blog = await repo.GetByIdAsync(id);
            if (blog == null)
                throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, MessageConstants.NOT_FOUND);

            blog.ViewCount += 1;
            
            await repo.UpdateAsync(blog);
            await _unitOfWork.SaveChangesAsync();
            return new BaseResponseModel(StatusCodes.Status200OK, ResponseCodeConstants.SUCCESS, MessageConstants.BLOG_VIEWCOUNT_INCREASED);
        }

        public async Task<BaseResponseModel> Update(Guid id, BlogRequest request)
        {
            var repo = _unitOfWork.Repository<Blog, Guid>();
            var blog = await repo.GetByIdAsync(id);
            if (blog == null)
               throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, MessageConstants.NOT_FOUND);

            _mapper.Map(request, blog);
            await repo.UpdateAsync(blog);
            await _unitOfWork.SaveChangesAsync();
            return new BaseResponseModel(StatusCodes.Status200OK, ResponseCodeConstants.SUCCESS, MessageConstants.BLOG_UPDATE_SUCCESS);

        }
      
    }
}
