using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Azure.Core;
using Microsoft.AspNetCore.Http;
using SmokingCessation.Application.DTOs.Request;
using SmokingCessation.Application.DTOs.Response;
using SmokingCessation.Application.Service.Interface;
using SmokingCessation.Core.Constants;
using SmokingCessation.Core.CustomExceptionss;
using SmokingCessation.Core.Response;
using SmokingCessation.Core.Utils;
using SmokingCessation.Domain.Entities;
using SmokingCessation.Domain.Interfaces;
using SmokingCessation.Domain.Specifications;

namespace SmokingCessation.Application.Service.Implementations
{
    public class BlogService : IBlogService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IUserContext _userContext;

        public BlogService(IUnitOfWork unitOfWork, IMapper mapper, IUserContext userContext)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _userContext = userContext;
        }

        public async Task<BaseResponseModel> Create(BlogRequest request)
        {
            var userId = _userContext.GetUserId();
            var blog = _mapper.Map<Blog>(request);
            blog.AuthorId = Guid.Parse(userId);
            blog.PublishedDate = CoreHelper.SystemTimeNow;
            blog.CreatedTime = CoreHelper.SystemTimeNow;

            await _unitOfWork.Repository<Blog, Guid>().AddAsync(blog);
            await _unitOfWork.SaveChangesAsync();
            return new BaseResponseModel(StatusCodes.Status200OK, ResponseCodeConstants.SUCCESS, MessageConstants.CREATE_SUCCESS);

        }

        public async Task<BaseResponseModel> Delete(Guid id)
        {
            var userId = _userContext.GetUserId();
            var repo = _unitOfWork.Repository<Blog, Guid>();
            var blog = await repo.GetByIdAsync(id);
            if (blog == null)
                return new BaseResponseModel(StatusCodes.Status404NotFound,ResponseCodeConstants.NOT_FOUND, MessageConstants.NOT_FOUND);

            blog.DeletedBy = userId;
            blog.DeletedTime = CoreHelper.SystemTimeNow;
            blog.LastUpdatedBy = userId;
            blog.LastUpdatedTime = CoreHelper.SystemTimeNow;
            await _unitOfWork.Repository<Blog, Guid>().UpdateAsync(blog);

            await _unitOfWork.SaveChangesAsync();
            return new BaseResponseModel(StatusCodes.Status200OK, ResponseCodeConstants.SUCCESS, MessageConstants.CREATE_SUCCESS);

        }

        public async Task<PaginatedList<BlogResponse>> GetAll(PagingRequestModel model)
        {
            var blogs = await _unitOfWork.Repository<Blog, Blog>().GetAllAsync();
            var result = _mapper.Map<List<BlogResponse>>(blogs.ToList());
            return PaginatedList<BlogResponse>.Create(result, model.PageNumber, model.PageSize);

        }

        public async Task<BaseResponseModel<BlogResponse>> GetBlogsDetails(Guid id)
        {
            var repo = _unitOfWork.Repository<Blog, Guid>();
            var blog = await repo.GetByIdAsync(id);
            if (blog == null)
                return new BaseResponseModel<BlogResponse>(StatusCodes.Status404NotFound,ResponseCodeConstants.NOT_FOUND, MessageConstants.NOT_FOUND);

            var result = _mapper.Map<BlogResponse>(blog);
            return new BaseResponseModel<BlogResponse>(StatusCodes.Status200OK, ResponseCodeConstants.SUCCESS, MessageConstants.CREATE_SUCCESS);

        }

        public async Task<BaseResponseModel> Update(Guid id, BlogRequest request)
        {
            var repo = _unitOfWork.Repository<Blog, Guid>();
            var blog = await repo.GetByIdAsync(id);
            if (blog == null)
                return new BaseResponseModel(StatusCodes.Status404NotFound,ResponseCodeConstants.NOT_FOUND, MessageConstants.NOT_FOUND);

            _mapper.Map(request, blog);
            await repo.UpdateAsync(blog);
            await _unitOfWork.SaveChangesAsync();
            return new BaseResponseModel(StatusCodes.Status200OK, ResponseCodeConstants.SUCCESS, MessageConstants.CREATE_SUCCESS);

        }
    }
}
