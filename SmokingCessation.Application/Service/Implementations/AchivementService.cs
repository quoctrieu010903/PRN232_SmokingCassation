using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SmokingCessation.Application.Service.Implementations
{
    public class AchivementService : IAchivementServie
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IPhotoService _photoService;


        public AchivementService(IUnitOfWork unitOfWork, IMapper mapper , IPhotoService photoService, IHttpContextAccessor httpContextAccessor)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _photoService = photoService;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<BaseResponseModel<AchievementResponse>> CreateAsync(AchievementCreateRequest request)
        {
            var userId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var achievement = _mapper.Map<Achievement>(request);
            // Handle image upload
            if (request.iconUrl != null)
            {
                var imageUrl = await _photoService.UploadPhotoAsync(request.iconUrl);
                achievement.IconUrl = imageUrl;
            }
            achievement.LastUpdatedBy = userId;
            achievement.LastUpdatedTime = DateTime.UtcNow;
            achievement.CreatedBy = userId;
            achievement.CreatedTime = DateTime.UtcNow; ;
            await _unitOfWork.Repository<Achievement, Achievement>().AddAsync(achievement);
            await _unitOfWork.SaveChangesAsync();
            var response = _mapper.Map<AchievementResponse>(achievement);
            return new BaseResponseModel<AchievementResponse>(200, "Created successfully", response);
        }

        public async Task<BaseResponseModel<AchievementResponse?>> GetByIdAsync(Guid id)
        {
            var achievement = await _unitOfWork.Repository<Achievement, Guid>().GetByIdAsync(id);
            if (achievement == null)
                throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, MessageConstants.NOT_FOUND);

            var response = _mapper.Map<AchievementResponse>(achievement);
            return new BaseResponseModel<AchievementResponse?>(
                StatusCodes.Status200OK,
                ResponseCodeConstants.SUCCESS,
                response
            );
        }

        public async Task<BaseResponseModel<IEnumerable<AchievementResponse>>> GetAllAsync( PagingRequestModel paging)
        {
            var baseSpeci = new BaseSpecification<Achievement>(mp => !mp.DeletedTime.HasValue);

            var response = await _unitOfWork.Repository<Achievement, Achievement>().GetAllWithSpecAsync(baseSpeci);
            var result = _mapper.Map<List<AchievementResponse>>(response);

            var paginatedList = PaginatedList<AchievementResponse>.Create(result, paging.PageNumber, paging.PageSize);

            return new BaseResponseModel<IEnumerable<AchievementResponse>>(
                StatusCodes.Status200OK,
                ResponseCodeConstants.SUCCESS,
                paginatedList.Items
            );
        }

        public async Task<BaseResponseModel<AchievementResponse?>> UpdateAsync(AchievementUpdateRequest request, Guid Id)
        {
            var userId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var achievement = await _unitOfWork.Repository<Achievement, Guid>().GetByIdAsync(Id);
            if (achievement == null)
                throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, MessageConstants.NOT_FOUND);
            _mapper.Map(request, achievement);
            achievement.LastUpdatedBy = userId;
            achievement.LastUpdatedTime = DateTime.UtcNow;
            await _unitOfWork.SaveChangesAsync();
            var response = _mapper.Map<AchievementResponse>(achievement);
            return new BaseResponseModel<AchievementResponse?>(200, "Updated successfully", response);
        }
    }
}
