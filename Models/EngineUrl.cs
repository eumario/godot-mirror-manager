/*
 * Structure of Engine URL's for the mirror.  Holds the folder, and file location for
 * a release of Godot Engine.
 *  
 * See below for Structure information 
 */
namespace Gmm.Models;

public class EngineUrl
{
	public string Version { get; set; } = "";
	public string BaseLocation { get; set; } = "";

	// List of URL parts that will get the OS Specific release of the Godot Engine
	// Example: "3.4.1/rc2/Godot_v3.4.1-rc2_osx.universal.zip" <-- OSX_64/OSX_arm64
	public string OSX32 { get; set; } = "";
	public string OSX64 { get; set; } = "";
	public string OSXarm64 { get; set; } = "";
	public string Win32 { get; set; } = "";
	public string Win64 { get; set; } = "";
	public string X1132 { get; set; } = "";
	public string X1164 { get; set; } = "";
	public string Source { get; set; } = "";

	// List of Tags associated with this release, such as "alpha", "beta", "rc", "mono" and empty means Official
	public List<string> Tags { get; set; } = new List<string>();
}