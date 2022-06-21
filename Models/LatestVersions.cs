using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GodotMirrorManager.Models;

public class LatestVersion
{
    [BsonId]
	public int Id { get; set; } = 0;

	public int MirrorId { get; set; } = 0;

	public DateTime LastUpdated { get; set; } = DateTime.UtcNow.AddDays(-1);

	public EngineUrl? Stable { get; set; }
	public EngineUrl? Alpha { get; set; }
	public EngineUrl? Beta { get; set; }
	public EngineUrl? ReleaseCandidate { get; set; }
	public EngineUrl? MonoStable { get; set; }
	public EngineUrl? MonoAlpha { get; set; }
	public EngineUrl? MonoBeta { get; set; }
	public EngineUrl? MonoReleaseCandidate { get; set; }

	public Version GetVersion(List<string> tags) {
		Version? vers = null;
		EngineUrl? matchUrl = null;

		if (tags.Count == 0) { // Standard Version
			matchUrl = Stable;
		} else if (tags.Count == 1) { // Mono, or Alpha/Beta/RC
			// TODO: Need to check for increment release for tags, EG: alpha1, alpha2, beta1, beta2, rc1, rc2, etc, etc
			if (tags[0] == "mono")
				matchUrl = MonoStable;
			else if (tags[0] == "alpha")
				matchUrl = Alpha;
			else if (tags[0] == "beta")
				matchUrl = Beta;
			else if (tags[0] == "rc")
				matchUrl = ReleaseCandidate;
		} else { // Mono + Alpha/Beta/RC
			// TODO: Need to check for increment release for tags, EG: alpha1, alpha2, beta1, beta2, rc1, rc2, etc, etc
			if (tags[1] == "alpha")
				matchUrl = MonoAlpha;
			else if (tags[1] == "beta")
				matchUrl = MonoBeta;
			else if (tags[1] == "rc")
				matchUrl = MonoReleaseCandidate;
		}
		if (matchUrl == null)
			vers = new Version("0.0");
		else if (matchUrl.Version.Contains("-"))
			vers = new Version(matchUrl.Version.Split("-")[0]);
		else
			vers = new Version(matchUrl.Version);

		return vers;
	}

	public void SetVersion(EngineUrl url) {
		if (url.Tags.Count == 0) {
			Stable = url;
		} else if (url.Tags.Count == 1) {
			if (url.Tags[0] == "mono")
				MonoStable = url;
			else if (url.Tags[0] == "alpha")
				Alpha = url;
			else if (url.Tags[0] == "beta")
				Beta = url;
			else if (url.Tags[0] == "rc")
				ReleaseCandidate = url;
		} else {
			if (url.Tags[1] == "alpha")
				MonoAlpha = url;
			else if (url.Tags[1] == "beta")
				MonoBeta = url;
			else if (url.Tags[1] == "rc")
				MonoReleaseCandidate = url;
		}
	}
}
