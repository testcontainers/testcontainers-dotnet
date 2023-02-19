namespace WebDriver
{
    public class  BrowserType
    {
        private BrowserType(string value)
        {
            Value = value;
        }
        
        public string Value { get; private set; }
        
        public static BrowserType Chrome => new BrowserType("chrome");
        public static BrowserType Firefox => new BrowserType("firefox");
        public static BrowserType MicrosoftEdge => new BrowserType("MicrosoftEdge");
        public static BrowserType Opera => new BrowserType("opera");
        
        public override string ToString()
        {
            return Value;
        }
    }
}