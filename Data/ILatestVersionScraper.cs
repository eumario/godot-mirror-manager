using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GodotMirrorManager.Data;

public interface ILatestVersionScraper
{
    void UpdateLatestVersions();
}
