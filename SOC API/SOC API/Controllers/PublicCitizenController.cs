using AutoMapper;
using SOC_API.DTO;
using Microsoft.AspNetCore.Mvc;
using SOC_API.Model;
using SOC_API.Repos;

namespace SOC_API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class PublicCitizenController : ControllerBase
	{
		private readonly IMapper mapper;
		private readonly PublicCitizenRepo repo;

		public PublicCitizenController(IMapper _mapper, PublicCitizenRepo _repo)
		{
			this.mapper = _mapper;
			this.repo = _repo;
		}

		[HttpPost("register")]
		public ActionResult Register(PublicCitizenWriteDTO dtoPublicCitizen)
		{
			if (repo.EmailExists(dtoPublicCitizen.Email!))
				return BadRequest("Email already registered!");

			var model = mapper.Map<PublicCitizen>(dtoPublicCitizen);

			if (repo.Create(model))
				return Ok(model);

			return BadRequest();
		}

		[HttpPost("login")]
		public ActionResult Login(string email, string password)
		{
			var publicitizen = repo.GetByEmail(email);
			if (publicitizen == null || publicitizen.Password != password)
				return Unauthorized("Invalid email or password!");

			return Ok(publicitizen);
		}

		[HttpGet]
		public ActionResult<List<PublicCitizenReadDTO>> GetPublicCitizens()
		{
			return Ok(mapper.Map<List<PublicCitizenReadDTO>>(repo.GetAll()));
		}

		[HttpGet("{id}")]
		public ActionResult<PublicCitizenReadDTO> GetPublicCitizen(int id)
		{
			var PublicCitizen = repo.GetById(id);
			if (PublicCitizen == null) return NotFound();

			return Ok(mapper.Map<PublicCitizenReadDTO>(PublicCitizen));
		}
	}
}