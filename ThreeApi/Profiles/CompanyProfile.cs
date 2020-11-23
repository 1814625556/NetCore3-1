using AutoMapper;
using ThreeApi.Entities;
using ThreeApi.Helpers;
using ThreeApi.Models;

namespace ThreeApi.Profiles
{
    public class CompanyProfile : Profile
    {
        public CompanyProfile()
        {
            CreateMap<Company, CompanyDto>()
                .ForMember(
                    dest => dest.CompanyName,
                    opt => opt.MapFrom(src => src.Name));
            CreateMap<CompanyAddDto, Company>();
            CreateMap<Company, CompanyFullDto>();
            CreateMap<CompanyAddWithBankruptTimeDto, Company>();
        }

    }
}
