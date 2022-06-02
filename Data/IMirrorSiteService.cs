using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GodotMirrorManager.Models;

namespace GodotMirrorManager.Data;

public interface IMirrorSiteService
{
	void Create(MirrorSite url);
	bool Exists(MirrorSite url);
	bool Exists(string name);
	bool Exists(int id);
	IEnumerable<MirrorSite> FindByName(string name);
	List<MirrorSite> Get();
	MirrorSite? Get(int id);
	bool Remove(int id);
	bool Update(MirrorSite url);
}
