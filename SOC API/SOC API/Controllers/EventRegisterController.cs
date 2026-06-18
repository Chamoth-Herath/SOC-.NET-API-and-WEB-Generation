using SOC_API.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SOC_API.Data;
using SOC_API.Model;
using SOC_API.Repos;

namespace SOC_API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class EventRegistrationController : ControllerBase
	{
		private readonly EventRegisterRepo repo;
		private readonly AppDbContext db;

		public EventRegistrationController(EventRegisterRepo _repo, AppDbContext _db)
		{
			this.repo = _repo;
			this.db = _db;
		}

		[HttpPost]
		public ActionResult RegisterForEvent([FromBody] EventRegisterWriteDTO dto)
		{
			if (dto == null || dto.EventId <= 0 || dto.PublicCitizenId <= 0)
				return BadRequest("Invalid registration data");

			var ev = db.Events.FirstOrDefault(e => e.Id == dto.EventId);
			if (ev == null) return NotFound("Event not found!");

			var publicCitizen = db.PublicCitizens.FirstOrDefault(p => p.Id == dto.PublicCitizenId);
			if (publicCitizen == null) return NotFound("Citizen not found!");

			if (repo.AlreadyRegistered(dto.EventId, dto.PublicCitizenId))
				return BadRequest("Already registered for this event!");

			if (repo.GetRegistrationCount(dto.EventId) >= ev.MaxCapacity)
				return BadRequest("Event is fully booked!");

			var registration = new EventRegister
			{
				EventId = dto.EventId,
				PublicCitizenId = dto.PublicCitizenId,
				RegisteredAt = DateTime.Now
			};

			db.EventRegisters.Add(registration);
			ev.RegisteredCitizens += 1;

			if (db.SaveChanges() > 0)
				return Ok("Registered successfully");

			return BadRequest("Failed to create registration");
		}

		[HttpGet("registrations")]
		public ActionResult GetAllRegistrations()
		{
			var registrations = db.EventRegisters
				.Include(r => r.Event)
				.Include(r => r.PublicCitizen)
				.Select(r => new
				{
					id = r.Id,
					registeredAt = r.RegisteredAt,
					eventId = r.EventId,
					publicCitizenId = r.PublicCitizenId,
					eventTitle = r.Event!.EventTitle,
					eventLocation = r.Event.Venue,
					publicCitizenName = r.PublicCitizen!.FullName,
					publicCitizenEmail = r.PublicCitizen.Email
				})
				.ToList();

			return Ok(registrations);
		}

		[HttpGet("organizer/{organizerId}")]
		public ActionResult GetRegistrationsByOrganizer(int organizerId)
		{
			var registrations = db.EventRegisters
				.Include(r => r.Event)
				.Include(r => r.PublicCitizen)
				.Where(r => r.Event.OrganizerId == organizerId)
				.Select(r => new
				{
					id = r.Id,
					registeredAt = r.RegisteredAt,
					eventId = r.EventId,
					publicCitizenId = r.PublicCitizenId,
					eventTitle = r.Event!.EventTitle,
					eventLocation = r.Event.Venue,
					publicCitizenName = r.PublicCitizen!.FullName,
					publicCitizenEmail = r.PublicCitizen.Email
				})
				.OrderByDescending(r => r.registeredAt)
				.ToList();

			return Ok(registrations);
		}

		[HttpGet("event/{eventId}")]
		public ActionResult GetEventRegistrations(int eventId)
		{
			var registrations = db.EventRegisters
				.Include(r => r.PublicCitizen)
				.Where(r => r.EventId == eventId)
				.Select(r => new
				{
					id = r.Id,
					registeredAt = r.RegisteredAt,
					eventId = r.EventId,
					publicCitizenId = r.PublicCitizenId,
					publicCitizenName = r.PublicCitizen!.FullName,
					publicCitizenEmail = r.PublicCitizen.Email
				})
				.ToList();

			return Ok(registrations);
		}

		[HttpGet("participant/{publicCitizenId}")]
		public ActionResult GetPublicCitizenRegistrations(int publicCitizenId)
		{
			var registrations = db.EventRegisters
				.Include(r => r.Event)
				.Where(r => r.PublicCitizenId == publicCitizenId)
				.Select(r => new
				{
					id = r.Id,
					eventId = r.EventId,
					registeredAt = r.RegisteredAt,
					eventTitle = r.Event!.EventTitle,
					venue = r.Event.Venue,
					date = r.Event.Date,
					time = r.Event.Time,
					activityImage = r.Event.ActivityImage,
					category = r.Event.Category
				})
				.OrderBy(r => r.date)
				.ToList();

			return Ok(registrations);
		}
	}
}