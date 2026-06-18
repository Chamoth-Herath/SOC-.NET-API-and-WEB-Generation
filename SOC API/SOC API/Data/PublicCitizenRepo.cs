using SOC_API.Data;
using SOC_API.Model;


namespace SOC_API.Repos
{
	public class PublicCitizenRepo
	{
		private readonly AppDbContext db;

		public PublicCitizenRepo(AppDbContext _db)
		{
			this.db = _db;
		}

		public bool Create(PublicCitizen publiccitizen)
		{
			db.PublicCitizens.Add(publiccitizen);        
			return db.SaveChanges() > 0;
		}

		public PublicCitizen? GetByEmail(string email)
		{
			return db.PublicCitizens.FirstOrDefault(p => p.Email == email);
		}

		public PublicCitizen? GetById(int id)
		{
			return db.PublicCitizens.FirstOrDefault(p => p.Id == id);
		}

		public List<PublicCitizen> GetAll()
		{
			return db.PublicCitizens.ToList();
		}

		public bool EmailExists(string email)
		{
			return db.PublicCitizens.Any(p => p.Email == email);
		}
	}
}