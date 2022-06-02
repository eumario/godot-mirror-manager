using GodotMirrorManager.Models;

namespace GodotMirrorManager.Data;


public class MirrorSiteService : IMirrorSiteService, IDisposable
{
	private readonly ILiteDatabase _db;
	private readonly ILiteCollection<MirrorSite> _mirrors;
	private readonly ILogger<MirrorSiteService> _logger;

	public MirrorSiteService(IOptions<GmmDatabaseSettings> options, ILogger<MirrorSiteService> logger)
	{
		_logger = logger;
		_logger.LogInformation("Initializing Mirror Site Services.");
		_db = Database.CreateDatabase(options.Value.ConnectionString);
		_mirrors = _db.GetCollection<MirrorSite>(options.Value.MirrorSiteCollectionName);
	}

	public List<MirrorSite> Get() => _mirrors.FindAll().ToList();
	public MirrorSite? Get(int id) => _mirrors.FindById(id);
	public void Create(MirrorSite url) => _mirrors.Insert(url);
	public bool Update(MirrorSite url) => _mirrors.Update(url);
	public bool Remove(int id) => _mirrors.Delete(id);
	public IEnumerable<MirrorSite> FindByName(string name) => _mirrors.Find(ms => ms.Name == name);
	public bool Exists(MirrorSite url) => _mirrors.Exists(iurl => iurl.Id == url.Id);
	public bool Exists(string name) => _mirrors.Exists(url => url.Name == name || url.BaseUrl == name);
	public bool Exists(int id) => _mirrors.Exists(url => url.Id == id);

	public void Dispose()
	{
		Database.Dispose(_db);
	}
}