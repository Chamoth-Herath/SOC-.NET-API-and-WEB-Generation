using AutoMapper;
using SOC_API.DTO;
using SOC_API.Model;

namespace SOC_API.Profiles
{
	public class OrganizerProfile : Profile
	{
		public OrganizerProfile()
		{
			CreateMap<OrganizerWriteDTO, Organizer>();
			CreateMap<Organizer, OrganizerReadDTO>();
		}
	}
}