using SOC_API.Data;
using SOC_API.Model;

namespace SOC_API.Repos
{
	public class OrganizerRepo
	{
		private AppDbContext db;
		public OrganizerRepo(AppDbContext _db)
		{
			this.db = _db;
		}

		public bool Create(Organizer organizer)
		{
			db.Organizers.Add(organizer);
			return db.SaveChanges() > 0;
		}

		public Organizer? GetByEmail(string email)
		{
			return db.Organizers.FirstOrDefault(o => o.Email == email);
		}

		public Organizer? GetById(int id)
		{
			return db.Organizers.FirstOrDefault(o => o.Id == id);
		}

		public List<Organizer> GetAll()
		{
			return db.Organizers.ToList();
		}

		public bool EmailExists(string email)
		{
			return db.Organizers.Any(o => o.Email == email);
		}
	}
}