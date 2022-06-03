using System.Net.Http;
using GodotMirrorManager.Models;

namespace GodotMirrorManager.Data;



public class FSScraperService : IFSScraperService
{
	private readonly ILiteDatabase _db;
	private readonly ILiteCollection<EngineUrl> _engineTable;
	private readonly ILogger<FSScraperService> _logger;

	private int requestCount = 0;

	public FSScraperService(IOptions<GmmDatabaseSettings> options, ILogger<FSScraperService> logger)
	{
		_logger = logger;
		_logger.LogInformation("Initializing FileSize Scraper Service");
		_db = Database.CreateDatabase(options.Value.ConnectionString);
		_engineTable = _db.GetCollection<EngineUrl>(options.Value.EngineUrlCollectionName);
	}

	internal async Task<int> GetFileSize(string url, string file) {
		HttpClient client = new HttpClient();
		var response = await client.SendAsync(new HttpRequestMessage(HttpMethod.Head, Path.Combine(url, file)));
		client.Dispose();
		requestCount += 1;
		if (response is null)
			return -1;
		if (response.StatusCode != System.Net.HttpStatusCode.OK)
			return -1;
		
		if (response.Content.Headers.ContentLength is null)
			return -1;
		
		return (int)response.Content.Headers.ContentLength;
	}

	public async Task UpdateFileSizes()
	{
		_logger.LogInformation("Starting file size update.");
		requestCount = 0;
		List<EngineUrl> updatedUrls = new();
		var results = _engineTable.FindAll().Where(x => 
								(x.OSX32 != "" && x.OSX32_Size == 0) ||
								(x.OSX64 != "" && x.OSX64_Size == 0) ||
								(x.OSXarm64 != "" && x.OSXarm64_Size == 0) ||
								(x.Win32 != "" && x.Win32_Size == 0) ||
								(x.Win64 != "" && x.Win64_Size == 0) ||
								(x.Linux32 != "" && x.Linux32_Size == 0) ||
								(x.Linux64 != "" && x.Linux64_Size == 0) ||
								(x.Source != "" && x.Source_Size == 0)).Select(x => x).ToList<EngineUrl>();
		_logger.LogInformation($"Found {results.Count()} urls that need updated");

		foreach(EngineUrl url in results) {
			bool updated = false;
			Console.WriteLine($"{url.Version} OSX32: {url.OSX32}");
			Console.WriteLine($"{url.Version} OSX64: {url.OSX64}");
			Console.WriteLine($"{url.Version} OSXarm64: {url.OSXarm64}");
			Console.WriteLine($"{url.Version} Win32: {url.Win32}");
			Console.WriteLine($"{url.Version} Win64: {url.Win64}");
			Console.WriteLine($"{url.Version} Linux32: {url.Linux32}");
			Console.WriteLine($"{url.Version} Linux64: {url.Linux64}");
			Console.WriteLine($"{url.Version} Source: {url.Source}");
			if (url.OSX32_Size == 0 && !String.IsNullOrEmpty(url.OSX32)) {
				var size = await GetFileSize(url.BaseLocation, url.OSX32);

				if (size != -1) {
					url.OSX32_Size = size;
					updated = true;
				}
			}

			if (url.OSX64_Size == 0 && !String.IsNullOrEmpty(url.OSX64)) {
				var size = await GetFileSize(url.BaseLocation, url.OSX64);

				if (size != -1) {
					url.OSX64_Size = size;
					url.OSXarm64_Size = size;
					updated = true;
				}
			}

			if (url.Win32_Size == 0 && !String.IsNullOrEmpty(url.Win32)) {
				var size = await GetFileSize(url.BaseLocation, url.Win32);

				if (size != -1) {
					url.Win32_Size = size;
					updated = true;
				}
			}

			if (url.Win64_Size == 0 && !String.IsNullOrEmpty(url.Win64)) {
				var size = await GetFileSize(url.BaseLocation, url.Win64);

				if (size != -1) {
					url.Win64_Size = size;
					updated = true;
				}
			}

			if (url.Linux32_Size == 0 && !String.IsNullOrEmpty(url.Linux32)) {
				var size = await GetFileSize(url.BaseLocation, url.Linux32);

				if (size != -1) {
					url.Linux32_Size = size;
					updated = true;
				}
			}

			if (url.Linux64_Size == 0 && !String.IsNullOrEmpty(url.Linux64)) {
				var size = await GetFileSize(url.BaseLocation, url.Linux64);

				if (size != -1) {
					url.Linux64_Size = size;
					updated = true;
				}
			}

			if (url.Source_Size == 0 && !String.IsNullOrEmpty(url.Source)) {
				var size = await GetFileSize(url.BaseLocation, url.Source);

				if (size != -1) {
					url.Source_Size = size;
					updated = true;
				}
			}
			if (updated) {
				_engineTable.Update(url);
				updatedUrls.Add(url);
			}
		}

		_db.Checkpoint();
		
		_logger.LogInformation($"Updated {updatedUrls.Count()} out of {results.Count()} urls with File Size information. ({requestCount} requests made)");
	}
}
