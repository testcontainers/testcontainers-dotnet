namespace WebDriver
{
    public class  BrowserType
    {
        private BrowserType(string value, string imageName)
        {
            Value = value;
            ImageName = imageName;
        }
        
        public string Value { get; }
        public string ImageName { get; }

        public static BrowserType Chrome => new BrowserType("chrome", "selenium/standalone-chrome");
        public static BrowserType Firefox => new BrowserType("firefox", "selenium/standalone-firefox");
        public static BrowserType MicrosoftEdge => new BrowserType("MicrosoftEdge", "selenium/standalone-edge");
        public static BrowserType Opera => new BrowserType("opera", "selenium/standalone-opera");
        
        public override string ToString()
        {
            return Value;
        }
    }
}