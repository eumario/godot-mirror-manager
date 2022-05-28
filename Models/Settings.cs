using System;
using LiteDB;

namespace Gmm.Models;

public class Settings {

	[BsonId]
	public ObjectId Id { get; set; }
	public List<string> Urls { get; set; } = new List<string>();
	public Dictionary<string,DateTime> LastUpdate { get; set; } = new Dictionary<string, DateTime>();

	static ILiteCollection<Settings> Table;

	public static Settings GetSettings(LiteDatabase db) {
		Table = db.GetCollection<Settings>("Settings");
		if (Table.Count(Query.All()) == 0) {
			var sett = new Settings();
			sett.SaveData();
			return sett;
		}
		return Table.FindAll().FirstOrDefault<Settings>(new Settings());
	}

	public Settings() {
		Id = ObjectId.NewObjectId();
		Urls = new List<string>() {
			"https://downloads.tuxfamily.org/godotengine/"
		};
		LastUpdate = new Dictionary<string, DateTime>() {
			{ "https://downloads.tuxfamily.org/godotengine/", DateTime.Now - TimeSpan.FromDays(1) }
		};
	}

	[BsonCtor]
	public Settings(ObjectId _id, List<string> urls, Dictionary<string, DateTime> lastUpdate) {
		Id = _id;
		Urls = urls;
		LastUpdate = lastUpdate;
	}

	public void SaveData() {
		if (Table.Count(Query.All()) == 0)
			Table.Insert(this);
		else
			Table.Update(this);
	}
}