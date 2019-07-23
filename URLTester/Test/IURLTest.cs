using UrlTester.Output;

namespace UrlTester.Test
{
    /// <summary>
    /// Interface that all URL test must inherit from
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IUrlTest<T>
    {
        bool LoadFile();
        bool TestLinks(OutputProgressHandler handle);
        bool OutputResults(OutputHandler handler);
        void OutputErrorMessages(OutputHandler handler);
    }
    
}
