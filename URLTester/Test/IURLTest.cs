
namespace URLTester.Test
{
    /// <summary>
    /// Interface that all URL test must be built fromm
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IURLTest<T>
    {

        bool LoadFile();
        bool TestLinks();
        bool OutPutResults();
        void OutPutErrorMessages();
    }

}
