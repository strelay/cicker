using Godot;

public class ConfigSystem : Node
{
    private string cfgPath = "res://gameconfig.cfg";
    private ConfigFile cfgFile = new ConfigFile();

    public override void _Ready()
    {
        bool isDesktop =
			(OS.GetName() == "Windows") ||
			(OS.GetName() == "X11") ||
			(OS.GetName() == "OSX");
        
        cfgPath = isDesktop ? "res://gameconfig.cfg" : "user://gameconfig.cfg";
        cfgFile.Load(cfgPath);
    }

    public void SaveValue(string section, string key, string value)
    {
        cfgFile.SetValue(section, key, value);
        cfgFile.Save(cfgPath);
    }
    
    public string LoadValue(string section, string key)
    {
        return cfgFile.GetValue(section, key, "").ToString();
    }
}
