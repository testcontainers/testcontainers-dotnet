namespace WebDriver
{
    public readonly struct BrowserType
    {
        private BrowserType(string browserName)
        {
            BrowserName = browserName;
        }
        
        public string BrowserName { get; }

        public static BrowserType Chrome => new BrowserType("selenium/standalone-chrome");
        public static BrowserType Firefox => new BrowserType("selenium/standalone-firefox");
        public static BrowserType MicrosoftEdge => new BrowserType("selenium/standalone-edge");
        public static BrowserType Opera => new BrowserType("selenium/standalone-opera");
        
        public override string ToString()
        {
            return BrowserName;
        }
    }
}