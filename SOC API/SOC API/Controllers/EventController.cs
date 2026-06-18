using AutoMapper;
using SOC_API.DTO;
using Microsoft.AspNetCore.Mvc;
using SOC_API.Model;
using SOC_API.Repos;

namespace SOC_API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class EventController : ControllerBase
	{
		private readonly IMapper mapper;
		private readonly EventRepo repo;

		public EventController(IMapper _mapper, EventRepo _repo)
		{
			this.mapper = _mapper;
			this.repo = _repo;
		}

		
		[HttpPost]
		[Consumes("multipart/form-data")]
		public async Task<IActionResult> Create(
			[FromForm] string eventTitle,
			[FromForm] string category,
			[FromForm] int maxCapacity,
			[FromForm] string venue,
			[FromForm] DateTime date,
			[FromForm] DateTime time,
			[FromForm] string? eventDescription = null,
			[FromForm] int organizerId = 0,
			[FromForm] string? eventStatus = null,
			[FromForm] string? filterplatform = null,
			IFormFile? activityImageFile = null)
		{
			
			string? imagePath = null;
			if (activityImageFile != null && activityImageFile.Length > 0)
			{
				var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "events");
				Directory.CreateDirectory(uploadsFolder);

				var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(activityImageFile.FileName);
				var filePath = Path.Combine(uploadsFolder, uniqueFileName);

				using (var stream = new FileStream(filePath, FileMode.Create))
				{
					await activityImageFile.CopyToAsync(stream);
				}

				imagePath = "/images/events/" + uniqueFileName;
			}

			
			var newEvent = new Event
			{
				EventTitle = eventTitle,
				EventDescription = eventDescription,
				Category = category,
				Venue = venue,
				Date = date,
				Time = time,
				MaxCapacity = maxCapacity,
				ActivityImage = imagePath,
				EventStatus = eventStatus ?? "Active",
				RegisteredCitizens = 0,
				OrganizerId = organizerId,
				FilterPlatform = filterplatform ?? "PrivateWEB"
			};

			if (repo.Create(newEvent))
				return Ok(newEvent);

			return BadRequest();
		}

		
		[HttpGet]
		public ActionResult<List<EventReadDTO>> GetEvents()
		{
			var events = repo.GetEvents();
			return Ok(mapper.Map<List<EventReadDTO>>(events));
		}

		
		[HttpGet("{id}")]
		public ActionResult<EventReadDTO> GetEvent(int id)
		{
			var ev = repo.GetEventById(id);
			if (ev == null) return NotFound();

			return Ok(mapper.Map<EventReadDTO>(ev));
		}

		
		[HttpGet("organizer/{organizerId}")]
		public ActionResult<IEnumerable<EventReadDTO>> GetEventsByOrganizer(int organizerId)
		{
			var events = repo.GetEventsByOrganizer(organizerId);
			return Ok(mapper.Map<List<EventReadDTO>>(events));
		}

		
		[HttpGet("public/kmc")]
		public ActionResult<List<EventReadDTO>> GetPublicEventsForKMC()
		{
			var events = repo.GetPublicEventsForKMC();
			return Ok(mapper.Map<List<EventReadDTO>>(events));
		}

		
		[HttpGet("search")]
		public ActionResult<List<EventReadDTO>> Search(
			string? type, string? location, DateTime? date)
		{
			var events = repo.Search(type, location, date);
			return Ok(mapper.Map<List<EventReadDTO>>(events));
		}

		
		[HttpPut("{id}")]
		[Consumes("multipart/form-data")]
		public async Task<IActionResult> UpdateEvent(
			int id,
			[FromForm] string eventTitle,
			[FromForm] string category,
			[FromForm] int maxCapacity,
			[FromForm] string venue,
			[FromForm] DateTime date,
			[FromForm] DateTime time,
			[FromForm] string? eventDescription = null,
			[FromForm] int organizerId = 0,
			[FromForm] string? eventStatus = null,
			[FromForm] string? activityPlatform = null,
			IFormFile? activityImageFile = null)
		{
			var existing = repo.GetEventById(id);
			if (existing == null) return NotFound();

			
			if (existing.OrganizerId != organizerId)
				return Unauthorized("Only the creator can update this event!");

			
			if (activityImageFile != null && activityImageFile.Length > 0)
			{
				var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "events");
				Directory.CreateDirectory(uploadsFolder);

				var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(activityImageFile.FileName);
				var filePath = Path.Combine(uploadsFolder, uniqueFileName);

				using (var stream = new FileStream(filePath, FileMode.Create))
				{
					await activityImageFile.CopyToAsync(stream);
				}

				existing.ActivityImage = "/images/events/" + uniqueFileName;
			}

			
			existing.EventTitle = eventTitle;
			existing.EventDescription = eventDescription;
			existing.Category = category;
			existing.Venue = venue;
			existing.Date = date;
			existing.Time = time;
			existing.MaxCapacity = maxCapacity;
			existing.EventStatus = eventStatus ?? existing.EventStatus;

			if (!string.IsNullOrEmpty(activityPlatform))
				existing.FilterPlatform = activityPlatform;

			if (repo.Update(existing))
				return Ok(existing);

			return BadRequest();
		}

		
		[HttpDelete("{id}")]
		public ActionResult DeleteEvent(int id)
		{
			var ev = repo.GetEventById(id);
			if (ev == null) return NotFound();

			if (repo.Remove(ev))
				return Ok();

			return BadRequest();
		}
	}
}