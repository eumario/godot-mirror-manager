namespace GodotMirrorManager.Models;

public class EngineUrl
{
	[BsonId]
    public int Id { get; set; } = 0;

	public int MirrorId { get; set; } = 0;

	public string Version { get; set; } = string.Empty;
	public string BaseLocation { get; set; } = string.Empty;

	public string OSX32 { get; set; } = string.Empty;
	public string OSX64 { get; set; } = string.Empty;
	public string OSXarm64 { get; set; } = string.Empty;
	public string Win32 { get; set; } = string.Empty;
	public string Win64 { get; set; } = string.Empty;
	public string Linux32 { get; set; } = string.Empty;
	public string Linux64 { get; set; } = string.Empty;
	public string Source { get; set; } = string.Empty;

	public List<string> Tags { get; set; } = new();
}
