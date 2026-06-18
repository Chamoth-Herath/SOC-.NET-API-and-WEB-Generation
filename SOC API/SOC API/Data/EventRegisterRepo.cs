using SOC_API.Data;
using SOC_API.Model;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace SOC_API.Repos
{
	public class EventRegisterRepo
	{
		private readonly AppDbContext db;

		public EventRegisterRepo(AppDbContext _db)
		{
			this.db = _db;
		}

		public bool Create(EventRegister registration)
		{
			db.EventRegisters.Add(registration);
			return db.SaveChanges() > 0;
		}

		public bool AlreadyRegistered(int eventId, int PublicCitizenId)
		{
			return db.EventRegisters
					 .Any(r => r.EventId == eventId && r.PublicCitizenId == PublicCitizenId);
		}

		public int GetRegistrationCount(int eventId)
		{
			return db.EventRegisters
					 .Count(r => r.EventId == eventId);
		}

		public List<EventRegister> GetByEvent(int eventId)
		{
			return db.EventRegisters
					 .Include(r => r.PublicCitizen)
					 .Where(r => r.EventId == eventId)
					 .ToList();
		}

		public List<EventRegister> GetByPublicCitizen(int PublicCitizenId)
		{
			return db.EventRegisters
					 .Include(r => r.Event)
					 .Where(r => r.PublicCitizenId == PublicCitizenId)
					 .ToList();
		}
	}
}