using AutoMapper;
using QLSanBong.Application.DTOs.Pitch;
using QLSanBong.Application.DTOs.Service;
using QLSanBong.Domain.Entities;
using QLSanBong.MVC.Models;

namespace QLSanBong.MVC.Mappings;

public class PitchMappingProfile : Profile
{
    public PitchMappingProfile()
    {
        CreateMap<PitchDto, PitchViewModel>().ReverseMap();

       
        CreateMap<PitchBookingDto, BookingViewModel>()
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status != null ? src.Status.ToString() : "Chờ Xác Nhận"))
           
            .ForMember(dest => dest.StartTime, opt => opt.MapFrom(src => src.StartTime.ToString(@"hh\:mm")))
            .ForMember(dest => dest.EndTime, opt => opt.MapFrom(src => src.EndTime.ToString(@"hh\:mm")))
         
            .ForMember(dest => dest.DurationHours, opt => opt.MapFrom(src => (int)(src.EndTime - src.StartTime).TotalHours));

        CreateMap<BookPitchViewModel, CreatePitchBookingDto>();
        CreateMap<PitchBookingDto, MyBookingViewModel>();

        CreateMap<Service, ServiceDto>();
    }
}