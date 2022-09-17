using HtmlAgilityPack;
using System.Text.RegularExpressions;
using GodotMirrorManager.Models;
using Hangfire;

namespace GodotMirrorManager.Data;

public class ScraperService : IScraperService
{
	private readonly ILiteDatabase _db;
	private readonly ILiteCollection<EngineUrl> _engineTable;
	private readonly ILiteCollection<MirrorSite> _mirrorTable;
	private readonly ILogger<ScraperService> _logger;
	private readonly IFSScraperService _fsScraper;
	private readonly Regex VersionMatch = new Regex(@"\d+(?:\.\d+)+");

	public ScraperService(IOptions<GmmDatabaseSettings> options, IFSScraperService fsScraper, ILogger<ScraperService> logger)
	{
		_logger = logger;
		_logger.LogInformation("Initializing Scraper Services.");
		_db = Database.CreateDatabase(options.Value.ConnectionString);
		_engineTable = _db.GetCollection<EngineUrl>(options.Value.EngineUrlCollectionName);
		_mirrorTable = _db.GetCollection<MirrorSite>(options.Value.MirrorSiteCollectionName);
		_fsScraper = fsScraper;
	}

	bool IsVersionStored(string version)
	{
		return _engineTable.Exists(Query.EQ("Version", version));
	}

	internal List<EngineUrl>? GetUrls(MirrorSite site, string version, string href, List<string> tags)
	{
		List<EngineUrl> urls = new List<EngineUrl>();
		EngineUrl current = new EngineUrl();
		HtmlWeb web = new HtmlWeb();
		HtmlDocument doc = web.Load(site.BaseUrl + href);

		var links = doc.DocumentNode.SelectNodes("//tr/td/a");

		if (links == null)
			return null;

		var zips = links.Where(link => link.InnerText.EndsWith(".zip")).Select(link => link.Attributes["href"].Value);

		current.Version = version;
		current.BaseLocation = $"{site.BaseUrl}{href}";
		current.MirrorId = site.Id;
		current.Tags = tags;

		current.OSX32 = zips.Where(href => href.EndsWith("osx32.zip")).FirstOrDefault("");
		current.OSX64 = zips.Where(href => href.EndsWith("osx.universal.zip") ||
		                            href.EndsWith("macos.universal.zip") ||
									href.EndsWith("osx64.zip") ||
									href.EndsWith("osx.64.zip") ||
									href.EndsWith("osx.fat.zip")).FirstOrDefault("");
		current.OSXarm64 = current.OSX64;
		current.Win32 = zips.Where(href => href.EndsWith("win_32.zip") || href.EndsWith("win32.zip") || href.EndsWith("win32.exe.zip")).FirstOrDefault("");
		current.Win64 = zips.Where(href => href.EndsWith("win_64.zip") || href.EndsWith("win64.zip") || href.EndsWith("win64.exe.zip")).FirstOrDefault("");
		current.Linux32 = zips.Where(href => href.EndsWith("linux_x86_32.zip") || href.EndsWith("linux.x86_32.zip") ||
											 href.EndsWith("x11_32.zip") || href.EndsWith("linux_32.zip") ||
											 href.EndsWith("x11.32.zip") || href.EndsWith("linux.32.zip")).FirstOrDefault("");
											 
		current.Linux64 = zips.Where(href => href.EndsWith("linux.x86_64.zip") || href.EndsWith("linux_x86_64.zip") ||
											 href.EndsWith("x11_64.zip") || href.EndsWith("x11.64.zip") ||
											 href.EndsWith("linux_64.zip") || href.EndsWith("linux.64.zip")).FirstOrDefault("");

		current.Source = links.Where(link => link.InnerText.EndsWith(".tar.xz")).Select(links => links.Attributes["href"].Value).FirstOrDefault("");

		if (!(current.OSX64 == "" && current.Win64 == "" && current.Linux64 == "") && !IsVersionStored(version))
			urls.Insert(0, current);

		var tlinks = links.Where(link => link.InnerText.StartsWith("rc") ||
								link.InnerText.StartsWith("beta") ||
								link.InnerText.StartsWith("alpha") ||
								link.InnerText.StartsWith("mono")).Select(link => new { link.InnerText, link.Attributes["href"].Value });

		foreach (var link in tlinks)
		{
			List<string> ltags = new List<string>();
			if (link.InnerText.StartsWith("rc"))
				ltags = new List<string>(tags) { "rc" };
			if (link.InnerText.StartsWith("beta"))
				ltags = new List<string>(tags) { "beta" };
			if (link.InnerText.StartsWith("alpha"))
				ltags = new List<string>(tags) { "alpha" };
			if (link.InnerText.StartsWith("mono"))
				ltags = new List<string>(tags) { "mono" };
			List<EngineUrl>? url = GetUrls(site, $"{version}-{link.InnerText}", $"{href}{link.Value}", ltags);
			if (url == null)
				continue;
			urls.AddRange(url);
		}

		return urls;
	}

	internal Dictionary<string, string>? GatherVersions(MirrorSite url)
	{
		Dictionary<string, string> urls = new Dictionary<string, string>();
		HtmlWeb web = new HtmlWeb();
		HtmlDocument doc = web.Load(url.BaseUrl);

		var links = doc.DocumentNode.SelectNodes("//tr/td/a");

		if (links == null)
			return null;

		var found = links.Where(link => VersionMatch.IsMatch(link.InnerText)).Select(link => new { link.InnerText, link.Attributes["href"].Value });
		foreach (var link in found)
		{
			urls[link.InnerText] = link.Value;
		}

		return urls;
	}

	public void ScrapeSites()
	{
		List<EngineUrl> found = new List<EngineUrl>();
		int mirrorScrapped = 0;

		_logger.LogInformation("Starting scraping...");

		foreach (MirrorSite site in _mirrorTable.FindAll())
		{
			if (site.LastUpdated + TimeSpan.FromHours(site.UpdateInterval) <= DateTime.UtcNow)
			{
				mirrorScrapped++;
				_logger.LogInformation($"Scrapping site {site.Name} [{site.BaseUrl}]");
				int nurls = 0;
				Dictionary<string, string>? urls = GatherVersions(site);

				if (urls == null)
				{
					_logger.LogWarning($"No Version information found for {site.Name}");
					continue;
				}

				foreach (string vers in urls.Keys)
				{
					_logger.LogDebug($"[{site.Name}] Found Version: {vers}");
					var eurls = GetUrls(site, vers, urls[vers], new List<string>());
					if (eurls != null)
					{
						nurls += eurls.Count;
						found.AddRange(eurls);
						_logger.LogDebug($"[{site.Name}] Found {eurls.Count} new links.");
					}
				}
				_logger.LogInformation($"[{site.Name}] Found {nurls} new links total.");
				var time = DateTime.UtcNow;
				_logger.LogInformation($"[{site.Name}] Last Update Check: {time}");
				site.LastUpdated = time;
				_mirrorTable.Update(site);
			}
		}

		if (found.Count() > 0)
		{
			_logger.LogInformation($"Found {found.Count()} new links from {mirrorScrapped} mirrors.");
			_engineTable.InsertBulk(found);
			_db.Checkpoint();
		}

		_logger.LogInformation("Scheduling Filesize Scraper in 2 minutes...");
		BackgroundJob.Schedule(
			() => _fsScraper.UpdateFileSizes(),
			TimeSpan.FromMinutes(2)
		);

		_logger.LogInformation("Scraping completed.");
	}
}
