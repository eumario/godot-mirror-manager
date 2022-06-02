using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GodotMirrorManager.Models;

namespace GodotMirrorManager.Data;

public interface IEngineUrlService
{
	EngineUrl? Get(int id);
	List<EngineUrl> GetAll(int mirrorId);
	List<EngineUrl> GetByTag(string tag);
	List<EngineUrl> GetByTag(string tag, MirrorSite mirror);
	List<EngineUrl> GetByTag(string tag, int mirror);
	List<EngineUrl> GetByTags(List<string> tags);
	List<EngineUrl> GetByTags(List<string> tags, MirrorSite mirror);
	List<EngineUrl> GetByTags(List<string> tags, int mirror);
	List<EngineUrl> GetByVersion(string vers);
	List<EngineUrl> GetByVersion(string vers, MirrorSite mirror);
	List<EngineUrl> GetByVersion(string vers, int mirror);
}