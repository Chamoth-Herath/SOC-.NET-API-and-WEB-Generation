using AutoMapper;
using SOC_API.DTO;
using SOC_API.Model;

public class EventProfile : Profile
{
	public EventProfile()
	{
		CreateMap<Event, EventReadDTO>()
			.ForMember(dest => dest.OrganizerName,
				opt => opt.MapFrom(src => src.Organizer!.FullName))
			.ForMember(dest => dest.FilterPlatform,
				opt => opt.MapFrom(src => src.FilterPlatform)); 
	}
}