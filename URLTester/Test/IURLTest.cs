using Core.Objects;
using System.Collections.Generic;

namespace UrlTester.Test
{
    /// <summary>
    /// Interface that all URL test must inherit from
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IUrlTest<T>
    {
        bool LoadFile();
        bool TestLinks();
        bool OutputResults();
        void OutputErrorMessages(OutputHandler handler);
    }
    
}
