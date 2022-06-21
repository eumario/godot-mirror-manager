using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GodotMirrorManager.Models;

namespace GodotMirrorManager.Data;

public class LatestVersionScraper : ILatestVersionScraper
{
	private readonly ILiteDatabase _db;
	private readonly ILiteCollection<MirrorSite> _mirrorTable;
	private readonly ILiteCollection<LatestVersion> _latestVersionTable;
	private readonly ILiteCollection<EngineUrl> _engineUrlTable;
	private readonly ILogger<LatestVersionScraper> _logger;

	public LatestVersionScraper(IOptions<GmmDatabaseSettings> options, ILogger<LatestVersionScraper> logger)
	{
		_logger = logger;
		_logger.LogInformation("Initializing Latest Version Scraper...");
		_db = Database.CreateDatabase(options.Value.ConnectionString);
		_latestVersionTable = _db.GetCollection<LatestVersion>(options.Value.LatestVersionCollectionName);
		_mirrorTable = _db.GetCollection<MirrorSite>(options.Value.MirrorSiteCollectionName);
		_engineUrlTable = _db.GetCollection<EngineUrl>(options.Value.EngineUrlCollectionName);
	}

	public void UpdateLatestVersions()
	{
		int mirrorLatestVersions = 0;
		int updatedVersions = 0;
		_logger.LogInformation("Starting check for latest versions...");
		foreach(MirrorSite site in _mirrorTable.FindAll()) {
			LatestVersion lv = _latestVersionTable.FindAll().Where(x => x.MirrorId == site.Id).FirstOrDefault(new LatestVersion());

			if (lv.LastUpdated + TimeSpan.FromHours(site.UpdateInterval) <= DateTime.UtcNow) {
				mirrorLatestVersions++;
				_logger.LogInformation($"Checking for latest version for {site.Name} [{site.BaseUrl}]");
				foreach(EngineUrl eurl in _engineUrlTable.FindAll().Where(x => x.MirrorId == site.Id)) {
					Version ever = new Version(eurl.Version.Contains("-") ? eurl.Version.Split("-")[0] : eurl.Version);
					Version lver = lv.GetVersion(eurl.Tags);
					if (ever > lver) {
						lv.SetVersion(eurl);
						updatedVersions++;
					}
				}
				lv.LastUpdated = DateTime.UtcNow;
				if (lv.Id != 0)
					_latestVersionTable.Update(lv);
				else
					_latestVersionTable.Insert(lv);
			}
		}

		_db.Checkpoint();

		_logger.LogInformation("Finished updating {0} mirror sites, with {1} latest version informations.",mirrorLatestVersions, updatedVersions);
	}
}
