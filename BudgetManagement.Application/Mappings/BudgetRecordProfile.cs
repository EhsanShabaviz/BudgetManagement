using AutoMapper;
using BudgetManagement.Application.DTOs.Budget;
using BudgetManagement.Domain.Entities;

namespace BudgetManagement.Application.Mappings
{
    public class BudgetRecordProfile : Profile
    {
        public BudgetRecordProfile()
        {
            // Entity → DTO
            CreateMap<BudgetRecord, BudgetRecordEditDto>();

            // DTO → Entity
            CreateMap<BudgetRecordEditDto, BudgetRecord>()
                .ForMember(dest => dest.SubProjectCode, opt => opt.Ignore());
            // چون کلید اصلی هست و نباید تغییر کند

            // Entity → DTO (برای گزارش‌گیری)
            CreateMap<BudgetRecord, BudgetReportDto>();
        }
    }
}
