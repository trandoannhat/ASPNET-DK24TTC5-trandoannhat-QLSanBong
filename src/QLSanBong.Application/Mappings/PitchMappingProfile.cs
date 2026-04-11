using AutoMapper;
using QLSanBong.Application.DTOs.Pitch;
using QLSanBong.Domain.Entities;

namespace QLSanBong.Application.Mappings;

public class PitchMappingProfile : Profile
{
    public PitchMappingProfile()
    {
        // Ánh xạ Entity Pitch <-> PitchDto
        CreateMap<Pitch, PitchDto>().ReverseMap();

        // Ánh xạ Entity PitchBooking <-> PitchBookingDto
        CreateMap<PitchBooking, PitchBookingDto>()
            .ForMember(dest => dest.CustomerName, opt => opt.MapFrom(src => src.User.FullName))
            .ForMember(dest => dest.CustomerPhone, opt => opt.MapFrom(src => src.User.PhoneNumber))
            .ForMember(dest => dest.PitchName, opt => opt.MapFrom(src => src.Pitch.Name))
            .ForMember(dest => dest.PitchType, opt => opt.MapFrom(src => src.Pitch.PitchType))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
            .ReverseMap();
    }
}