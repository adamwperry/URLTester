namespace UrlTester.Objects
{
    /// <summary>
    /// A list of application arguments that can be provided at runtime.
    /// </summary>
    public class Arguments
    {
        public string FilePath { get; set; }
        public string Domain { get; set; }
        public string OutputText { get; set; }
        public bool Mutlithreaded { get; set; }
        public bool Help { get; set; }

        public Arguments()
        {
            Help = false;
        }
    }
}
