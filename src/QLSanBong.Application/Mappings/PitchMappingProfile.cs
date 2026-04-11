using AutoMapper;
using QLSanBong.Application.DTOs.Pitch;
using QLSanBong.Domain.Entities;

namespace QLSanBong.Application.Mappings;

public class PitchMappingProfile : Profile
{
    public PitchMappingProfile()
    {
        CreateMap<Pitch, PitchDto>().ReverseMap();

        CreateMap<PitchBooking, PitchBookingDto>()
            .ForMember(d => d.CustomerName, o => o.MapFrom(s => s.User.FullName))
            .ForMember(d => d.CustomerPhone, o => o.MapFrom(s => s.User.PhoneNumber))
            .ForMember(d => d.PitchName, o => o.MapFrom(s => s.Pitch.Name))
            .ForMember(d => d.PitchType, o => o.MapFrom(s => s.Pitch.PitchType))
            .ForMember(d => d.Status, o => o.MapFrom(s => s.Status.ToString()));

        CreateMap<PitchBooking, PitchBookingDetailDto>()
            .IncludeBase<PitchBooking, PitchBookingDto>();
    }
}