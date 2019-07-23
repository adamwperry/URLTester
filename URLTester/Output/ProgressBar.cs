using System;




namespace _URLTester.Output
{
    public interface IProgressBar
    {
        void UpdateProgressBar(int currentIndex, string currentItem);
    }

    public class ProgessBar : IProgressBar
    {
        const int totalBlocks = 30;
        const int blockIncrement = 5;
        const ConsoleColor blueConsole = ConsoleColor.Blue;
        const ConsoleColor blackConsole = ConsoleColor.Black;

        private readonly int _totalCount;

        private int _currerntIndex;
        private string _currentItem;

        public ProgessBar(int totalCount)
        {
            _totalCount = totalCount;
        }

        public void UpdateProgressBar(int currentIndex, string currentItem)
        {
            _currerntIndex = currentIndex;
            _currentItem = currentItem;


            CreateBar();
            UpdateBar();
            DrawTotals();
            DrawOutput();
        }

        private void DrawTotals()
        {
            Console.CursorLeft = totalBlocks + blockIncrement;
            Console.BackgroundColor = blackConsole;
        }

        private void DrawOutput()
        {
            string output = _currerntIndex.ToString() + " of " + _totalCount.ToString();

            var percentageComplete = CalculatePercenageComplete();
            Console.Write(output.PadRight(15) + $"%{percentageComplete} \n {_currentItem}");
        }

        private int CalculatePercenageComplete()
        {
            return (int)(0.5f + ((100f * _currerntIndex) / _totalCount));
        }

        private void UpdateBar()
        {
            double pctComplete = Convert.ToDouble(_currerntIndex) / _totalCount;

            int numChunksComplete = Convert.ToInt16(totalBlocks * pctComplete);

            Console.BackgroundColor = blueConsole;
            Console.Write("".PadRight(numChunksComplete));

            Console.BackgroundColor = ConsoleColor.Gray;
            Console.Write("".PadRight(totalBlocks - numChunksComplete));

        }

        private void CreateBar()
        {
            Console.CursorTop = 10;

            Console.CursorLeft = 0;
            Console.Write("[");
            Console.CursorLeft = totalBlocks + 1;
            Console.Write("]");
            Console.CursorLeft = 1;
        }

    }
}
