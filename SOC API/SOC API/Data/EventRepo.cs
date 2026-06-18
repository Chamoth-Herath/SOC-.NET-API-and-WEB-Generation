using SOC_API.Data;
using SOC_API.Model;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace SOC_API.Repos
{
	public class EventRepo
	{
		private readonly AppDbContext db;

		public EventRepo(AppDbContext _db)
		{
			this.db = _db;
		}

		public bool Create(Event ev)
		{
			db.Events.Add(ev);
			return db.SaveChanges() > 0;
		}

		
		public List<Event> GetEvents()
		{
			return db.Events
					 .Include(e => e.Organizer)
					 .Where(e => e.FilterPlatform == "PrivateWEB")
					 .OrderByDescending(e => e.Date)
					 .ToList();
		}

		
		public List<Event> GetPublicEventsForKMC()
		{
			return db.Events
					 .Include(e => e.Organizer)
					 .Where(e => e.FilterPlatform == "PrivateWEB" ||
								 e.FilterPlatform == "KMC_WEB" ||
								 e.FilterPlatform == null)
					 .OrderByDescending(e => e.Date)
					 .ToList();
		}

		public Event? GetEventById(int id)
		{
			return db.Events
					 .Include(e => e.Organizer)
					 .FirstOrDefault(e => e.Id == id);
		}

		public bool Update(Event ev)
		{
			db.Events.Update(ev);
			return db.SaveChanges() > 0;
		}

		public bool Remove(Event ev)
		{
			db.Events.Remove(ev);
			return db.SaveChanges() > 0;
		}


		public IEnumerable<Event> GetEventsByOrganizer(int organizerId)
		{
			return db.Events
					 .Include(e => e.Organizer)
					 .Where(e => e.OrganizerId == organizerId &&
								(e.FilterPlatform == "PrivateWEB" ||
								 e.FilterPlatform == "KMC_WEB" ||  // ← add this
								 e.FilterPlatform == "KMC" ||
								 e.FilterPlatform == null))
					 .OrderByDescending(e => e.Date)
					 .ToList();
		}

		public List<Event> Search(string? type, string? location, DateTime? date)
		{
			var query = db.Events
						  .Include(e => e.Organizer)
						  .Where(e => e.FilterPlatform == "PrivateWEB")
						  .AsQueryable();

			if (!string.IsNullOrEmpty(type))
				query = query.Where(e => e.Category == type);

			if (!string.IsNullOrEmpty(location))
				query = query.Where(e => e.Venue.Contains(location));

			if (date.HasValue)
				query = query.Where(e => e.Date.Date == date.Value.Date);

			return query.ToList();
		}
	}
}