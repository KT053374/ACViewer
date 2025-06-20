namespace ACViewer.Config
{
    public class Config
    {
        public string ACFolder { get; set; }
        public bool AutomaticallyLoadDATsOnStartup { get; set; }
        public Database Database { get; set; } = new Database();
        public Toggles Toggles { get; set; } = new Toggles();
        public MapViewerOptions MapViewer { get; set; } = new MapViewerOptions();
        public BackgroundColors BackgroundColors { get; set; } = new BackgroundColors();
        public string Theme { get; set; }
        public Mouse Mouse { get; set; } = new Mouse();
        public WindowPos WindowPos { get; set; } = new WindowPos();
    }
}
