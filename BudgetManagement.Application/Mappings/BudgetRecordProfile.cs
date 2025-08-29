using AutoMapper;
using BudgetManagement.Application.DTOs;
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
        }
    }
}
