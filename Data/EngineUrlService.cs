using GodotMirrorManager.Models;

namespace GodotMirrorManager.Data;


public class EngineUrlService : IEngineUrlService, IDisposable
{
	private readonly ILiteDatabase _db;
	private readonly ILiteCollection<EngineUrl> _engines;
	private readonly ILogger<EngineUrlService> _logger;

	public EngineUrlService(IOptions<GmmDatabaseSettings> options, ILogger<EngineUrlService> logger)
	{
		_logger = logger;
		_logger.LogInformation("Initializing Engine URL Services.");
		_db = Database.CreateDatabase(options.Value.ConnectionString);
		_engines = _db.GetCollection<EngineUrl>(options.Value.EngineUrlCollectionName);
	}

	//public List<EngineUrl> Get() => _engines.FindAll().ToList();
	public List<EngineUrl> GetAll(int mirrorId) => _engines.FindAll().Where(eurl => eurl.MirrorId == mirrorId).ToList();
	public EngineUrl? Get(int id) => _engines.FindById(id);
	// public void Create(EngineUrl url) => _engines.Insert(url);
	// public bool Update(EngineUrl url) => _engines.Update(url);
	// public bool Remove(ObjectId id) => _engines.Delete(id);
	public List<EngineUrl> GetByVersion(string vers) => _engines.Find(eurl => eurl.Version == vers).ToList();
	public List<EngineUrl> GetByVersion(string vers, MirrorSite mirror) => _engines.Find(eurl => eurl.Version == vers).Where(eurl => eurl.MirrorId == mirror.Id).ToList();
	public List<EngineUrl> GetByVersion(string vers, int mirror) => _engines.Find(eurl => eurl.Version.Contains(vers)).Where(eurl => eurl.MirrorId == mirror).ToList();
	public List<EngineUrl> GetByTag(string tag) => _engines.Find(eurl => eurl.Tags.Contains(tag)).ToList();
	public List<EngineUrl> GetByTag(string tag, MirrorSite mirror) => _engines.Find(eurl => eurl.Tags.Contains(tag)).Where(eurl => eurl.MirrorId == mirror.Id).ToList();
	public List<EngineUrl> GetByTag(string tag, int mirror) => _engines.Find(eurl => eurl.Tags.Contains(tag)).Where(eurl => eurl.MirrorId == mirror).ToList();
	public List<EngineUrl> GetByTags(List<string> tags) => _engines.Find(eurl => eurl.Tags.Intersect(tags).Any()).ToList();
	public List<EngineUrl> GetByTags(List<string> tags, MirrorSite mirror) => _engines.Find(eurl => eurl.Tags.Intersect(tags).Any()).Where(eurl => eurl.MirrorId == mirror.Id).ToList();
	public List<EngineUrl> GetByTags(List<string> tags, int mirror) => _engines.Find(eurl => eurl.Tags.Intersect(tags).Any()).Where(eurl => eurl.MirrorId == mirror).ToList();


	public void Dispose()
	{
		Database.Dispose(_db);
	}
}
