namespace GodotMirrorManager.Data;

public class Database
{
	private static Dictionary<string, ILiteDatabase> dbHandle = new Dictionary<string, ILiteDatabase>();
	private static Dictionary<ILiteDatabase, int> refCount = new Dictionary<ILiteDatabase, int>();

	public static ILiteDatabase CreateDatabase(string ConnectionString) {
		if (!dbHandle.ContainsKey(ConnectionString)) {
			dbHandle[ConnectionString] = new LiteDatabase(ConnectionString);
			refCount[dbHandle[ConnectionString]] = 0;
			dbHandle[ConnectionString].UtcDate = true;
		}
		var _db = dbHandle[ConnectionString];
		refCount[_db] += 1;
		return _db;
	}

	public static void Dispose(ILiteDatabase db)
	{
		if (refCount[db] == 0) {
			db.Checkpoint();
			db.Dispose();
			refCount.Remove(db);
			string? connStr = dbHandle.Where(kv => kv.Value == db).Select(kv => kv.Key).FirstOrDefault();
			if (!(connStr is null))
				dbHandle.Remove(connStr);
		}
	}
}
