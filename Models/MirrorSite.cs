using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LiteDB;

namespace GodotMirrorManager.Models;

public class MirrorSite
{
	[BsonId]
    public int Id { get; set; } = 0;
	public string Name { get; set; } = string.Empty;
	public string BaseUrl { get; set; } = string.Empty;
	public int UpdateInterval { get; set; } = 24;
	public DateTime LastUpdated { get; set; } = DateTime.UtcNow;
}
