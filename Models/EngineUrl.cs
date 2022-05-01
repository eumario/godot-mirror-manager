/*
 * Structure of Engine URL's for the mirror.  Holds the folder, and file location for
 * a release of Godot Engine.
 *  
 * See below for Structure information 
 */
namespace Gmm.Models;

public class EngineUrl
{
	// Id: This Id for this entry
	public Guid Id { get; set; } = Guid.NewGuid();

	// VersionId: Id for the Parent Version structure for the release of Godot, EG: 3.4, 3.4.1, 3.4.2, etc, etc
	public Guid VersionId { get; set; } = Guid.NewGuid();

	// List of URL parts that will get the OS Specific release of the Godot Engine
	// Example: "3.4.1/rc2/Godot_v3.4.1-rc2_osx.universal.zip" <-- OSX_64/OSX_arm64
	public string OSX_64 { get; set; } = "";
	public string OSX_arm64 { get; set; } = "";
	public string Windows_32 { get; set; } = "";
	public string Windows_64 { get; set; } = "";
	public string X11_32 { get; set; } = "";
	public string X11_64 { get; set; } = "";
	public string Source { get; set; } = "";

	// List of Tags associated with this release, such as "alpha", "beta", "rc", "mono" and empty means Official
	public List<string> Tags { get; set; } = new List<string>();
}