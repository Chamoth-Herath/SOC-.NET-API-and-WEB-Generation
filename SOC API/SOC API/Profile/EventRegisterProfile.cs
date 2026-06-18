using AutoMapper;
using SOC_API.DTO;
using SOC_API.Model;

namespace SOC_API.Profiles
{
	public class EventRegisterProfile : Profile
	{
		public EventRegisterProfile()
		{
			CreateMap<EventRegister, EventRegisterReadDTO>()
				.ForMember(dest => dest.EventTitle,
					opt => opt.MapFrom(src => src.Event!.EventTitle))
				.ForMember(dest => dest.Venue,
					opt => opt.MapFrom(src => src.Event!.Venue))
				.ForMember(dest => dest.Date,
					opt => opt.MapFrom(src => src.Event!.Date))
				.ForMember(dest => dest.PublicCitizenName,
					opt => opt.MapFrom(src => src.PublicCitizen!.FullName))
				.ForMember(dest => dest.PublicCitizenEmail,
					opt => opt.MapFrom(src => src.PublicCitizen!.Email));
		}
	}
}