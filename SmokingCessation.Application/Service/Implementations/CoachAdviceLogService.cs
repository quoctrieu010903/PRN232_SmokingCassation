using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using SmokingCessation.Application.DTOs.Request;
using SmokingCessation.Application.DTOs.Response;
using SmokingCessation.Application.Service.Interface;
using SmokingCessation.Application.Service.Interfaces;
using SmokingCessation.Core.Utils;
using SmokingCessation.Domain.Entities;
using SmokingCessation.Domain.Interfaces;

namespace SmokingCessation.Application.Service.Implementations
{
    public class CoachAdviceLogService : ICoachAdviceLogService
    {

        private readonly IUnitOfWork _unitOfWork;
        private readonly IDeepSeekService _deepSeekService;
        private readonly IMapper _mapper;

        public CoachAdviceLogService(IUnitOfWork unitOfWork, IDeepSeekService deepSeekService, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _deepSeekService = deepSeekService;
            _mapper = mapper;
        }

        /// <summary>
        /// Tạo một log lời khuyên mới cho kế hoạch bỏ thuốc, sinh nội dung bằng AI DeepSeek.
        /// </summary>
        public async Task CreateAdviceLogAsync(Guid quitPlanId)
        {
            var quitPlan = await _unitOfWork.Repository<QuitPlan, Guid>().GetByIdAsync(quitPlanId);
            if (quitPlan == null) throw new Exception("Quit plan not found");

            var prompt = $"User wants to quit smoking for the following reason: {quitPlan.Reason}. " +
                         $"Start date: {quitPlan.StartDate:yyyy-MM-dd}, Target date: {quitPlan.TargetDate:yyyy-MM-dd}. " +
                         $"Please provide a motivational advice to help the user stay on track.";

            var adviceText = await _deepSeekService.GenerateAdviceAsync(prompt);

            var adviceLog = new CoachAdviceLog
            {
                QuitPlanId = quitPlanId,
                AdviceDate = CoreHelper.SystemTimeNow,
                AdviceText = adviceText,
                CreatedTime = CoreHelper.SystemTimeNow,
                LastUpdatedTime = CoreHelper.SystemTimeNow
            };
            await _unitOfWork.Repository<CoachAdviceLog, Guid>().AddAsync(adviceLog);
            await _unitOfWork.SaveChangesAsync();
        }

        /// <summary>
        /// Sinh và lưu lời khuyên hàng ngày cho kế hoạch bỏ thuốc, sử dụng AI DeepSeek dựa trên tiến trình gần nhất.
        /// </summary>
        public async Task<CoachAdviceLogResponse> GenerateAndSaveDailyAdviceAsync(CoachAdviceLogRequest request)
        {
            var quitPlan = await _unitOfWork.Repository<QuitPlan, Guid>().GetByIdAsync(request.QuitPlanId);
            if (quitPlan == null) throw new Exception("Không tìm thấy kế hoạch bỏ thuốc.");

            var progressLogs = (await _unitOfWork.Repository<ProgressLog, ProgressLog>()
                .GetAllWithIncludeAsync(true, p => p.QuitPlan))
                .Where(p => p.QuitPlanId == request.QuitPlanId)
                .OrderByDescending(p => p.LogDate)
                .Take(3)
                .ToList();

            var lastProgress = progressLogs.FirstOrDefault();
            var streak = progressLogs.Count(pl => pl.SmokedToday == 0);

            var lastAdvice = (await _unitOfWork.Repository<CoachAdviceLog, CoachAdviceLog>()
                .GetAllAsync())
                .Where(a => a.QuitPlanId == request.QuitPlanId)
                .OrderByDescending(a => a.AdviceDate)
                .FirstOrDefault();

            var prompt = $@"
            User is trying to quit smoking.
            - Reason: {quitPlan.Reason}
            - Start date: {quitPlan.StartDate:yyyy-MM-dd}
            - Target quit date: {quitPlan.TargetDate:yyyy-MM-dd}
            - Days smoke-free streak: {streak}
            - Yesterday: {(lastProgress != null ? lastProgress.SmokedToday : 0)} cigarettes.
            - User note: ""{lastProgress?.Note ?? "No note."}""
            - Previous advice: ""{lastAdvice?.AdviceText ?? "None yet."}""
            Today is {CoreHelper.SystemTimeNow:yyyy-MM-dd}. Please give a new, motivational, and practical piece of advice to help the user stay on track with their quit plan. Avoid repeating previous advice.
        ";

            var adviceText = await _deepSeekService.GenerateAdviceAsync(prompt);

            var adviceLog = new CoachAdviceLog
            {
                QuitPlanId = request.QuitPlanId,
                AdviceDate = CoreHelper.SystemTimeNow,
                AdviceText = adviceText,
                CreatedTime = CoreHelper.SystemTimeNow,
                LastUpdatedTime = CoreHelper.SystemTimeNow
            };
            await _unitOfWork.Repository<CoachAdviceLog, Guid>().AddAsync(adviceLog);
            await _unitOfWork.SaveChangesAsync();

            // Use AutoMapper
            var response = _mapper.Map<CoachAdviceLogResponse>(adviceLog);
            response.QuitPlanReason = quitPlan.Reason;
            return response;
        }

        /// <summary>
        /// Lấy lời khuyên mới nhất cho một kế hoạch bỏ thuốc.
        /// </summary>
        public async Task<CoachAdviceLogResponse> GetLatestAdviceAsync(Guid quitPlanId)
        {
            var logs = await _unitOfWork.Repository<CoachAdviceLog, CoachAdviceLog>().GetAllAsync();
            var latest = logs
                .Where(x => x.QuitPlanId == quitPlanId)
                .OrderByDescending(x => x.AdviceDate)
                .FirstOrDefault();

            if (latest == null) return null;

            var quitPlan = await _unitOfWork.Repository<QuitPlan, Guid>().GetByIdAsync(quitPlanId);

            var response = _mapper.Map<CoachAdviceLogResponse>(latest);
            response.QuitPlanReason = quitPlan?.Reason ?? "";
            return response;
        }

        /// <summary>
        /// Lấy tất cả lời khuyên của một kế hoạch bỏ thuốc.
        /// </summary>
        public async Task<List<CoachAdviceLogResponse>> GetAllAdvicesByQuitPlanAsync(Guid quitPlanId)
        {
            var logs = await _unitOfWork.Repository<CoachAdviceLog, CoachAdviceLog>().GetAllAsync();
            var quitPlan = await _unitOfWork.Repository<QuitPlan, Guid>().GetByIdAsync(quitPlanId);

            var responses = _mapper.Map<List<CoachAdviceLogResponse>>(logs.Where(x => x.QuitPlanId == quitPlanId)
                .OrderByDescending(x => x.AdviceDate)
                .ToList());

            foreach (var r in responses)
                r.QuitPlanReason = quitPlan?.Reason ?? "";

            return responses;
        }

        /// <summary>
        /// Lấy chi tiết một lời khuyên theo Id.
        /// </summary>
        public async Task<CoachAdviceLogResponse> GetAdviceByIdAsync(Guid adviceLogId)
        {
            var log = await _unitOfWork.Repository<CoachAdviceLog, Guid>().GetByIdAsync(adviceLogId);
            if (log == null) return null;

            var quitPlan = await _unitOfWork.Repository<QuitPlan, Guid>().GetByIdAsync(log.QuitPlanId);

            var response = _mapper.Map<CoachAdviceLogResponse>(log);
            response.QuitPlanReason = quitPlan?.Reason ?? "";
            return response;
        }

        /// <summary>
        /// Xóa một log lời khuyên theo Id.
        /// </summary>
        public async Task<bool> DeleteAdviceAsync(Guid adviceLogId)
        {
            var log = await _unitOfWork.Repository<CoachAdviceLog, Guid>().GetByIdAsync(adviceLogId);
            if (log == null) return false;

            await _unitOfWork.Repository<CoachAdviceLog, Guid>().DeleteAsync(adviceLogId);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        /// <summary>
        /// Cập nhật nội dung lời khuyên.
        /// </summary>
        public async Task<CoachAdviceLogResponse> UpdateAdviceAsync(Guid adviceLogId, string newAdviceText)
        {
            var log = await _unitOfWork.Repository<CoachAdviceLog, Guid>().GetByIdAsync(adviceLogId);
            if (log == null) return null;

            log.AdviceText = newAdviceText;
            log.LastUpdatedTime = CoreHelper.SystemTimeNow;
            await _unitOfWork.Repository<CoachAdviceLog, Guid>().UpdateAsync(log);
            await _unitOfWork.SaveChangesAsync();

            var quitPlan = await _unitOfWork.Repository<QuitPlan, Guid>().GetByIdAsync(log.QuitPlanId);

            var response = _mapper.Map<CoachAdviceLogResponse>(log);
            response.QuitPlanReason = quitPlan?.Reason ?? "";
            return response;
        }

        /// <summary>
        /// Lấy lịch sử tất cả lời khuyên của một user (theo tất cả kế hoạch bỏ thuốc).
        /// </summary>
        public async Task<List<CoachAdviceLogResponse>> GetAdviceHistoryByUserAsync(Guid userId)
        {
            var quitPlans = await _unitOfWork.Repository<QuitPlan, QuitPlan>().GetAllAsync();
            var userQuitPlanIds = quitPlans.Where(q => q.UserId == userId).Select(q => q.Id).ToList();

            var logs = await _unitOfWork.Repository<CoachAdviceLog, CoachAdviceLog>().GetAllAsync();
            var filteredLogs = logs
                .Where(x => userQuitPlanIds.Contains(x.QuitPlanId))
                .OrderByDescending(x => x.AdviceDate)
                .ToList();

            var responses = _mapper.Map<List<CoachAdviceLogResponse>>(filteredLogs);
            foreach (var r in responses)
            {
                var plan = quitPlans.FirstOrDefault(q => q.Id == filteredLogs.FirstOrDefault(l => l.Id == r.Id)?.QuitPlanId);
                r.QuitPlanReason = plan?.Reason ?? "";
            }
            return responses;
        }

        /// <summary>
        /// (Admin) Lấy tất cả lời khuyên trong hệ thống (có thể thêm filter/paging nếu cần).
        /// </summary>
        public async Task<List<CoachAdviceLogResponse>> GetAllAdvicesAsync()
        {
            var logs = await _unitOfWork.Repository<CoachAdviceLog, CoachAdviceLog>().GetAllAsync();
            var quitPlans = await _unitOfWork.Repository<QuitPlan, QuitPlan>().GetAllAsync();

            var responses = _mapper.Map<List<CoachAdviceLogResponse>>(logs.OrderByDescending(x => x.AdviceDate).ToList());
            foreach (var r in responses)
            {
                var plan = quitPlans.FirstOrDefault(q => q.Id == logs.FirstOrDefault(l => l.Id == r.Id)?.QuitPlanId);
                r.QuitPlanReason = plan?.Reason ?? "";
            }
            return responses;
        }

    }
}



