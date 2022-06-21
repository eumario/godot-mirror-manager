namespace GodotMirrorManager.Data;

public class GmmDatabaseSettings
{
	public ObjectId Id { get; set; } = ObjectId.Empty;
    public string ConnectionString { get; set; } = string.Empty;
	public string EngineUrlCollectionName { get; set; } = string.Empty;
	public string MirrorSiteCollectionName { get; set; } = string.Empty;
	public string LatestVersionCollectionName { get; set; } = string.Empty;
}
