using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using SmokingCessation.Application.DTOs.Request;
using SmokingCessation.Application.DTOs.Response;
using SmokingCessation.Application.Service.Interface;
using SmokingCessation.Core.Constants;
using SmokingCessation.Core.CustomExceptionss;
using SmokingCessation.Core.Response;
using SmokingCessation.Domain.Entities;
using SmokingCessation.Domain.Interfaces;
using SmokingCessation.Domain.Specifications;

namespace SmokingCessation.Application.Service.Implementations
{
    public class RatingService : IRatingService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IUserContext _userContext;

        public RatingService(IUnitOfWork unitOfWork, IMapper mapper, IUserContext userContext)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _userContext = userContext;
        }

        public async Task<BaseResponseModel> Create(RatingRequest request)
        {
            var userId = _userContext.GetUserId();
            var rating = new Rating
            {
                BlogId = request.BlogId,
                UserId = Guid.Parse(userId),
                Start = request.Value,
               
            };
            await _unitOfWork.Repository<Rating, Guid>().AddAsync(rating);
            await _unitOfWork.SaveChangesAsync();
            return new BaseResponseModel(200, "SUCCESS", "Rating created");
        }

        public async Task<BaseResponseModel> Update(Guid id, RatingRequest request)
        {
            if (request.Value < 1 || request.Value > 5)
                throw new ErrorException(400, "INVALID_VALUE", "Rating value must be between 1 and 5");

            var repo = _unitOfWork.Repository<Rating, Guid>();
            var rating = await repo.GetByIdAsync(id);
            if (rating == null)
                throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, MessageConstants.NOT_FOUND);


            rating.Start = request.Value;
           
            await repo.UpdateAsync(rating);
            await _unitOfWork.SaveChangesAsync();
            return new BaseResponseModel(200, "SUCCESS", "Rating updated");
        }

        public async Task<BaseResponseModel> Delete(Guid id)
        {
            var repo = _unitOfWork.Repository<Rating, Guid>();
            var rating = await repo.GetByIdAsync(id);
            if (rating == null)
                throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, MessageConstants.NOT_FOUND);


            await repo.DeleteAsync(id);
            await _unitOfWork.SaveChangesAsync();
            return new BaseResponseModel(200, "SUCCESS", "Rating deleted");
        }

        public async Task<PaginatedList<RatingResponse>> GetByBlogId(Guid blogId, PagingRequestModel paging)
        {
            var baseSpeci = new BaseSpecification<Rating>(r => r.BlogId == blogId);
            var ratings = await _unitOfWork.Repository<Rating, Rating>().GetAllWithSpecAsync(baseSpeci);
            var result = _mapper.Map<List<RatingResponse>>(ratings);
            return PaginatedList<RatingResponse>.Create(result, paging.PageNumber, paging.PageSize);
        }

        public async Task<PaginatedList<RatingResponse>> GetByUserId(Guid userId, PagingRequestModel paging)
        {
            var baseSpeci = new BaseSpecification<Rating>(r => r.UserId == userId);
            var ratings = await _unitOfWork.Repository<Rating, Rating>().GetAllWithSpecAsync(baseSpeci);
            var result = _mapper.Map<List<RatingResponse>>(ratings);
            return PaginatedList<RatingResponse>.Create(result, paging.PageNumber, paging.PageSize);
        }

        public async Task<BaseResponseModel<RatingResponse>> GetById(Guid id)
        {
            var repo = _unitOfWork.Repository<Rating, Guid>();
            var rating = await repo.GetByIdAsync(id);
            if (rating == null)
                throw new ErrorException(StatusCodes.Status404NotFound, ResponseCodeConstants.NOT_FOUND, MessageConstants.NOT_FOUND);

            var result = _mapper.Map<RatingResponse>(rating);
            return new BaseResponseModel<RatingResponse>(200, "SUCCESS", result);
        }
    }
}
