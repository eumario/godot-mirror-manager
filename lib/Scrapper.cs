using System;
using System.IO;
using System.Reflection;
using System.Collections.Generic;
using HtmlAgilityPack;
using System.Text.RegularExpressions;
using LiteDB;
using Gmm.Models;

namespace Gmm.Web;

public class Scrapper
{
	LiteDatabase DB;
	ILiteCollection<EngineUrl> Versions;
	Regex VersionMatch = new Regex(@"\d+(?:\.\d+)+");
	public Settings Settings;

	public Scrapper(LiteDatabase db = null) {
		if (db == null)
			DB = new LiteDatabase("./EngineURL.db");
		else
			DB = db;
		DB.UtcDate = true;
		Versions = DB.GetCollection<EngineUrl>("Versions");
		Settings = Settings.GetSettings(DB);
		foreach(string key in Settings.LastUpdate.Keys)
		{
			Settings.LastUpdate[key] = Settings.LastUpdate[key].ToLocalTime();
		}
	}

	bool IsVersionStored(string version) {
		return Versions.Exists(Query.EQ("Version", version));
	}

	public List<EngineUrl> GetUrls(string site, string version, string href, List<string> tags) {
		List<EngineUrl> urls = new List<EngineUrl>();
		EngineUrl current = new EngineUrl();
		HtmlWeb web = new HtmlWeb();
		HtmlDocument doc = web.Load(site + href);

		var links = doc.DocumentNode.SelectNodes("//tr/td/a");

		if (links == null)
			return null;

		var zips = links.Where(link => link.InnerText.EndsWith(".zip")).Select(link => link.Attributes["href"].Value);

		current.Version = version;
		current.BaseLocation = $"{site}{href}";
		current.Tags = tags;

		current.OSX32 = zips.Where(href => href.EndsWith("osx32.zip")).FirstOrDefault("");
		current.OSX64 = zips.Where(href => href.EndsWith("osx.universal.zip") ||
									href.EndsWith("osx64.zip") ||
									href.EndsWith("osx.64.zip") ||
									href.EndsWith("osx.fat.zip")).FirstOrDefault("");
		current.OSXarm64 = current.OSX64;
		current.Win32 = zips.Where(href => href.EndsWith("win_32.zip") ||
									href.EndsWith("win32.zip") ||
									href.EndsWith("win32.exe.zip")).FirstOrDefault("");
		current.Win64 = zips.Where(href => href.EndsWith("win_64.zip") ||
									href.EndsWith("win64.zip") ||
									href.EndsWith("win64.exe.zip")).FirstOrDefault("");

		current.X1132 = zips.Where(href => href.EndsWith("x11_32.zip") ||
									href.EndsWith("linux_32.zip") ||
									href.EndsWith("x11.32.zip") ||
									href.EndsWith("linux.32.zip")).FirstOrDefault("");
		
		current.X1164 = zips.Where(href => href.EndsWith("x11_64.zip") ||
									href.EndsWith("linux_64.zip") ||
									href.EndsWith("x11.64.zip") ||
									href.EndsWith("linux.64.zip")).FirstOrDefault("");
		
		if (!(current.OSX64 == "" && current.Win64 == "" && current.X1164 == "") && !IsVersionStored(version))
			urls.Insert(0,current);
		
		var tlinks = links.Where(link => link.InnerText.StartsWith("rc") ||
								link.InnerText.StartsWith("beta") ||
								link.InnerText.StartsWith("alpha") ||
								link.InnerText.StartsWith("mono")).Select(link => new { link.InnerText, link.Attributes["href"].Value });
		
		foreach (var link in tlinks) {
			List<string> ltags = new List<string>();
			if (link.InnerText.StartsWith("rc"))
				ltags = new List<string>(tags) { "rc", "beta" };
			if (link.InnerText.StartsWith("beta"))
				ltags = new List<string>(tags) { "beta" };
			if (link.InnerText.StartsWith("mono"))
				ltags = new List<string>(tags) { "mono" };
			List<EngineUrl> url = GetUrls(site, $"{version}-{link.InnerText}", $"{href}{link.Value}", ltags);
			urls.AddRange(url);
		}

		return urls;
	}

	public Dictionary<string, string> GatherVersions(string url) {
		Dictionary<string, string> urls = new Dictionary<string, string>();
		HtmlWeb web = new HtmlWeb();
		HtmlDocument doc = web.Load(url);

		var links = doc.DocumentNode.SelectNodes("//tr/td/a");

		if (links == null)
			return null;

		var found = links.Where(link => VersionMatch.IsMatch(link.InnerText)).Select(link => new { link.InnerText, link.Attributes["href"].Value });
		foreach(var link in found) {
			urls[link.InnerText] = link.Value;
		}

		return urls;
	}

	public void ScrapeSites() {
		List<EngineUrl> found = new List<EngineUrl>();
		// Log starting scrape
		foreach(String url in Settings.Urls) {
			Console.WriteLine($"Scrapping URL: {url}");
			int nurls = 0;
			// Log scrapping URL
			Dictionary<string, string> urls = GatherVersions(url);
			foreach(string vers in urls.Keys) {
				Console.WriteLine($"Found Version: {vers}");
				// Log each version check.
				var eurls = GetUrls(url, vers, urls[vers], new List<string>());
				if (eurls != null) {
					nurls += eurls.Count;
					found.AddRange(eurls);
				}
			}
			Console.WriteLine($"Found URLs: {nurls}");
			var time = DateTime.Now;
			Console.WriteLine($"Last Update Check: {time}");
			Settings.LastUpdate[url] = time;
		}
		Versions.InsertBulk(found);
		Settings.SaveData();
		DB.Checkpoint();
	}
}