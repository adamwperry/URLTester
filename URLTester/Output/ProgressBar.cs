using System;
using System.Threading;

namespace _URLTester.Output
{
    public interface IProgressBar
    {
        //ConsoleColor ProgressBarColor { get; set; }
        //ConsoleColor ProgressBarBackgroundColor { get; set; }
        //ConsoleColor ProgressCurrentItemColor { get; set; }

 
        //void Report(int currentIndex, string currentItem);


    }
    
    public class ConsoleProgressBar : IProgressBar, IDisposable
    {
        private const int blockCount = 10;
        private readonly int TotalBlocksWidth, BlockIncrement, TotalCount;

        private ConsoleColor ProgressBarColor { get; set; }
        private ConsoleColor ProgressBarBackgroundColor { get; set; }
        private ConsoleColor ProgressCurrentItemColor { get; set; }
        
        private readonly TimeSpan animationInterval = TimeSpan.FromSeconds(1.0 / 8);
        private readonly Timer timer;

        private double currentIndex = 0;
        private string currentItem = "";

        private bool disposed = false;     

        private BarPoints points;
        private struct BarPoints
        {
            int? y;
            public int Y { get { return y ?? 10; } set { y = value; } }


            int? x;
            public int X { get { return x ?? 0; } set { x = value; } }
        }

        public ConsoleProgressBar(int totalCount, ConsoleColor progressBarColor = ConsoleColor.Blue, ConsoleColor progressBarBackgroundColor = ConsoleColor.Black, ConsoleColor progressCurrentItemColor = ConsoleColor.Cyan, int progressBarBlockSize = 30)
        {
            TotalCount = totalCount;

            ProgressBarColor = progressBarColor;
            ProgressBarBackgroundColor = progressBarBackgroundColor;
            ProgressCurrentItemColor = progressCurrentItemColor;
            
            TotalBlocksWidth = progressBarBlockSize;
            BlockIncrement = progressBarBlockSize / 6;

            points = new BarPoints();

            timer = new Timer(TimerHandler);

            if (!Console.IsOutputRedirected)
            {
                ResetTimer();
            }
        }



        private void TimerHandler(object state)
        {
            lock (timer)
            {
                if (disposed) return;

                UpdateText(currentItem, currentIndex);

                ResetTimer();
            }
        }

        private void UpdateText(string curItem, double curIndex)
        {
            CreateBar();
            UpdateBar(curIndex);
            DrawTotals();
            DrawOutput(curItem);
        }


        private void CreateBar()
        {
            Console.CursorVisible = false;
            Console.CursorTop = points.Y;
            Console.CursorLeft = points.X;
            Console.Write("[");
            Console.CursorLeft = TotalBlocksWidth + 1;
            Console.Write("]");
            Console.CursorLeft = 1;
        }

        private void UpdateBar(double curIndex)
        {
            var chunksComplete = (int)(TotalBlocksWidth * (Convert.ToDouble(curIndex) / TotalCount));

            Console.BackgroundColor = ProgressBarColor;
            Console.Write("".PadRight(chunksComplete));

            Console.BackgroundColor = ConsoleColor.Gray;
            Console.Write("".PadRight(TotalBlocksWidth - chunksComplete));

        }

        private void DrawTotals()
        {
            Console.CursorLeft = TotalBlocksWidth + BlockIncrement;
            Console.BackgroundColor = ProgressBarBackgroundColor;
        }

        private void DrawOutput(string curItem)
        {
            string output = currentIndex.ToString() + " of " + TotalCount.ToString();

            var percentageComplete = CalculatePercentageComplete();

            Console.Write(output.PadRight(15) + $"%{percentageComplete}");
            Console.WriteLine("\n \n");


            ClearCurrentConsoleLine();
            Console.ForegroundColor = ProgressCurrentItemColor;
            Console.WriteLine($"{curItem}");
            Console.ForegroundColor = ConsoleColor.Gray;
        }

        private int CalculatePercentageComplete()
        {
            return (int)(0.5f + ((100f * currentIndex) / TotalCount));
        }

        private void ClearCurrentConsoleLine()
        {
            int currentLineCursor = Console.CursorTop;
            Console.SetCursorPosition(0, Console.CursorTop);
            for (int i = 0; i < Console.WindowWidth; i++)
            {
                Console.Write(" ");
            }

            Console.SetCursorPosition(0, currentLineCursor);
        }

        private void ResetTimer()
        {
            timer.Change(animationInterval, TimeSpan.FromMilliseconds(-1));
        }

        public void Dispose()
        {
            lock (timer)
            {
                disposed = true;
                UpdateText(string.Empty, 0);
                Console.CursorVisible = true;
            }
        }

        public void Report(double value, string text)
        {
            Interlocked.Exchange(ref currentIndex, value);

            Interlocked.Exchange(ref currentItem, text);
        }
    }
}
