using AutoMapper;
using BookingService.Data;
using BookingService.DTOs;
using BookingService.Models;
using BookingService.Repositories;
using BookingService.Specifications;
using BookingService.Utils;

namespace BookingService.Services
{
    /// <summary>
    /// Service responsible for managing both business and employee weekly schedules.
    /// </summary>
    public class ScheduleService : IScheduleService
    {
        private readonly IBusinessRepository _businessRepository;
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IScheduleRepository _scheduleRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<ScheduleService> _logger;

        public ScheduleService(IBusinessRepository businessRepository,  IScheduleRepository scheduleRepository, IMapper mapper, ILogger<ScheduleService> logger, IEmployeeRepository employeeRepository)
        {
            _businessRepository = businessRepository;
            _scheduleRepository = scheduleRepository;
            _mapper = mapper;
            _logger = logger;
            _employeeRepository = employeeRepository;
        }

        /// <summary>
        /// Gets the current weekly schedule for the business.
        /// </summary>
        public async Task<ServiceResult<List<DayScheduleDto>>> GetBusinessScheduleAsync(int businessId)
        {
            var spec = new BusinessSpecifications
            {
                IncludeSchedule = true
            };

            var business = await _businessRepository.GetByIdAsync(businessId, spec);
            if (business == null)
            {
                _logger.LogWarning("Business {BusinessId} not found while retrieving schedule.", businessId);
                return ServiceResult<List<DayScheduleDto>>.Failure("Business not found.", 404);
            }

            var schedule = _mapper.Map<List<DayScheduleDto>>(business.Schedule.OrderBy(d => (int)d.Day));
            return ServiceResult<List<DayScheduleDto>>.SuccessResult(schedule);
        }

        /// <summary>
        /// Replaces the entire weekly schedule for the business with the provided time slots.
        /// </summary>
        public async Task<ServiceResult> UpdateBusinessScheduleAsync(int businessId, List<DayScheduleUpdateDto> scheduleDto)
        {
            var business = await _businessRepository.GetByIdAsync(businessId, new BusinessSpecifications
            {
                IncludeSchedule = true
            });

            if (business == null)
            {
                _logger.LogWarning("Business {BusinessId} not found while updating schedule.", businessId);
                return ServiceResult.Failure("Business not found.", 404);
            }

            // Checking validity of time slots
            foreach (var dayScheduleDto in scheduleDto)
            {
                if (dayScheduleDto.TimeSlots.Any(ts => ts.StartTime >= ts.EndTime))
                {
                    _logger.LogWarning("Invalid time slot in schedule for business {BusinessId}.", businessId);
                    return ServiceResult.Failure("Incorrect time slots in the work schedule.", 400);
                }

            }

            await _scheduleRepository.DeleteByBusinessIdAsync(businessId);

            var updatedSchedule = _mapper.Map<List<DaySchedule>>(scheduleDto);
            foreach (var schedule in updatedSchedule)
            {
                schedule.BusinessId = businessId;
            }

            business.Schedule = updatedSchedule;

            await _businessRepository.UpdateAsync(business);

            _logger.LogInformation("Schedule successfully updated for business {BusinessId}", businessId);
            return ServiceResult.SuccessResult();
        }

        /// <summary>
        /// Gets the current weekly schedule for the employee.
        /// </summary>
        public async Task<ServiceResult<List<DayScheduleDto>>> GetEmployeeScheduleAsync(int employeeId, int businessId)
        {
            var spec = new BusinessSpecifications
            {
                IncludeSchedule = true,
                IncludeEmployees = true,
            };

            var business = await _businessRepository.GetByIdAsync(businessId, spec);
            if (business == null)
            {
                _logger.LogWarning("Business {BusinessId} not found while retrieving employee schedule.", businessId);
                return ServiceResult<List<DayScheduleDto>>.Failure("Business not found.", 404);
            }

            var employee = business.Employees.FirstOrDefault(e => e.Id == employeeId);
            if (employee == null)
            {
                _logger.LogWarning("Employee {EmployeeId} not found while retrieving employee schedule.", employeeId);
                return ServiceResult<List<DayScheduleDto>>.Failure("Employee not found.", 404);
            }

            var schedule = _mapper.Map<List<DayScheduleDto>>(employee.Schedule.OrderBy(d => (int)d.Day));
            return ServiceResult<List<DayScheduleDto>>.SuccessResult(schedule);
        }

        /// <summary>
        /// Replaces the entrire weekly schedule for employee with the provided time slots.
        /// </summary>
        public async Task<ServiceResult> UpdateEmployeeScheduleAsync(int employeeId, int businessId, List<DayScheduleUpdateDto> scheduleDto)
        {
            var business = await _businessRepository.GetByIdAsync(businessId, new BusinessSpecifications
            {
                IncludeSchedule = true,
                IncludeEmployees = true,
            });

            if (business == null)
            {
                _logger.LogWarning("Business {BusinessId} not found while updating employee schedule.", businessId);
                return ServiceResult.Failure("Business not found.", 404);
            }

            var employee = business.Employees.FirstOrDefault(e => e.Id == employeeId);
            if (employee == null)
            {
                _logger.LogWarning("Employee {EmployeeId} not found while updating employee schedule.", employeeId);
                return ServiceResult.Failure("Employee not found.", 404);
            }

            var businessTimeSlots = business.Schedule.ToDictionary(s => s.Day, s => s.TimeSlots);

            // Checking validity of time slots
            foreach (var dayScheduleDto in scheduleDto)
            {
                if (dayScheduleDto.TimeSlots.Any(ts => ts.StartTime >= ts.EndTime))
                {
                    _logger.LogWarning("Invalid time slot in schedule for employee {EmployeeId}.", employeeId);
                    return ServiceResult.Failure("Incorrect time slots in the work schedule.", 400);
                }

                if(!businessTimeSlots.TryGetValue(dayScheduleDto.Day, out var businessSlots))
                {
                    _logger.LogWarning("Attempt to set employee working hours on closed business day: {Day}.", dayScheduleDto.Day);
                    return ServiceResult.Failure($"Business is closed on {dayScheduleDto.Day}.", 400);
                }

                foreach (var timeSlot in dayScheduleDto.TimeSlots)
                {
                    bool fitsInBusiness = businessSlots.Any(bs =>
                        bs.StartTime <= timeSlot.StartTime && bs.EndTime >= timeSlot.EndTime);

                    if (!fitsInBusiness)
                    {
                        _logger.LogWarning("Employee {EmployeeId} time slot {Start}-{End} exceeds business hours on {Day}.", employeeId, timeSlot.StartTime, timeSlot.EndTime, dayScheduleDto.Day);
                        return ServiceResult.Failure("Employee's working hours exceed business working hours.", 400);
                    }
                }
            }

            await _scheduleRepository.DeleteByEmployeeIdAsync(employeeId);

            var updatedSchedule = _mapper.Map<List<DaySchedule>>(scheduleDto);
            foreach (var schedule in updatedSchedule)
            {
                schedule.EmployeeId = employeeId;
            }

            employee.Schedule = updatedSchedule;

            await _employeeRepository.UpdateAsync(employee);

            _logger.LogInformation("Schedule successfully updated for employee {EmployeeId}", employeeId);
            return ServiceResult.SuccessResult();
        }
    }
}
