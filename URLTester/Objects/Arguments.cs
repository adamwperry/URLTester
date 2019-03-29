namespace _URLTester.Objects
{
    /// <summary>
    /// A list of application arguments that can be provided at runtime.
    /// </summary>
    public class Arguments
    {
        public string filePath { get; set; }
        public string domain { get; set; }
        public string outputText { get; set; }
        public bool mutlithreaded { get; set; }
        public bool help { get; set; }

        public Arguments()
        {
            help = false;
        }
    }
}
