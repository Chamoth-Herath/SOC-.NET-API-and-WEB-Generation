using AutoMapper;
using SOC_API.DTO;
using Microsoft.AspNetCore.Mvc;
using SOC_API.Model;
using SOC_API.Repos;

namespace SOC_API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class OrganizerController : ControllerBase
	{
		private readonly IMapper mapper;
		private readonly OrganizerRepo repo;

		public OrganizerController(IMapper _mapper, OrganizerRepo _repo)
		{
			this.mapper = _mapper;
			this.repo = _repo;
		}

		[HttpPost("register")]
		public ActionResult Register(OrganizerWriteDTO dtoOrganizer)
		{
			if (repo.EmailExists(dtoOrganizer.Email!))
				return BadRequest("Email already registered!");

			var model = mapper.Map<Organizer>(dtoOrganizer);

			if (repo.Create(model))
				return Ok(model);

			return BadRequest();
		}

		[HttpPost("login")]
		public ActionResult Login(string email, string password)
		{
			var organizer = repo.GetByEmail(email);
			if (organizer == null || organizer.Password != password)
				return Unauthorized("Invalid email or password!");

			return Ok(organizer);
		}

		[HttpGet]
		public ActionResult<List<OrganizerReadDTO>> GetOrganizers()
		{
			return Ok(mapper.Map<List<OrganizerReadDTO>>(repo.GetAll()));
		}

		[HttpGet("{id}")]
		public ActionResult<OrganizerReadDTO> GetOrganizer(int id)
		{
			var organizer = repo.GetById(id);
			if (organizer == null) return NotFound();

			return Ok(mapper.Map<OrganizerReadDTO>(organizer));
		}
	}
}