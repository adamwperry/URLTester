using System;

namespace _URLTester.Output
{
    public interface IProgressBar : IDisposable
    {
        void Report(double value, string text);
    }
}