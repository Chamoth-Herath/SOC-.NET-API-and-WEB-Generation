using AutoMapper;
using SOC_API.DTO;
using SOC_API.Model;

namespace SOC_API.Profiles
{
	public class PublicCitizenProfile : Profile
	{
		public PublicCitizenProfile()
		{
			CreateMap<PublicCitizenWriteDTO, PublicCitizen>();
			CreateMap<PublicCitizen, PublicCitizenReadDTO>();
		}
	}
}