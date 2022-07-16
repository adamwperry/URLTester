using System;
using System.Threading;

namespace _URLTester.Output
{

    /// <summary>
    /// Contains the progress bar colors and sizes so they can be overridden. 
    /// </summary>
    public struct ConsoleProgressBarArgs
    {
        ConsoleColor? progressBarColor;
        public ConsoleColor ProgressBarColor { get { return progressBarColor ?? ConsoleColor.Blue; } set { progressBarColor = value; } }

        ConsoleColor? progressBarUnfilledColor;
        public ConsoleColor ProgressBarUnfilledColor { get { return progressBarUnfilledColor ?? ConsoleColor.Gray; } set { progressBarUnfilledColor = value; } }

        ConsoleColor? consoleBackgroundColor;
        public ConsoleColor ConsoleBackgroundColor { get { return consoleBackgroundColor ?? ConsoleColor.Black; } set { consoleBackgroundColor = value; } }

        ConsoleColor? progressCurrentItemColor;
        public ConsoleColor ProgressCurrentItemColor { get { return progressCurrentItemColor ?? ConsoleColor.Cyan; } set { progressCurrentItemColor = value; } }

        ConsoleColor? consoleFontColor;
        public ConsoleColor ConsoleFontColor { get { return consoleFontColor ?? ConsoleColor.Gray; } set { consoleFontColor = value; } }

        int? progressBarBlockSize;
        public int ProgressBarBlockSize { get { return progressBarBlockSize ?? 30; } set { progressBarBlockSize = value; } }
    }

    public class ConsoleProgressBar : IProgressBar
    {
        private readonly int TotalBlocksWidth, BlockIncrement, TotalCount;

        private readonly ConsoleProgressBarArgs barArgs;

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

        /// <summary>
        /// Sets up the progress bar by accepting the total items count, and the ConsoleProgressBarArgs. 
        /// </summary>
        /// <param name="totalCount">required total count of items.</param>
        /// <param name="args">This provides a default setup and is only required when a modification is need to the bar.</param>
        public ConsoleProgressBar(int totalCount, ConsoleProgressBarArgs args = new ConsoleProgressBarArgs())
        {
            TotalCount = totalCount;

            barArgs = args;
 
            TotalBlocksWidth = args.ProgressBarBlockSize;
            BlockIncrement = TotalBlocksWidth / 6;

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


        /// <summary>
        /// Updates the text on the screen, uses the current item and current index.
        /// </summary>
        /// <param name="curItem"></param>
        /// <param name="curIndex"></param>
        private void UpdateText(string curItem, double curIndex)
        {
            CreateBar();
            UpdateBar(curIndex);
            DrawOutput(curItem);
        }

        /// <summary>
        /// Creates the bar container
        /// </summary>
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

        /// <summary>
        /// Updates the bar using the current index.
        /// </summary>
        /// <param name="curIndex"></param>
        private void UpdateBar(double curIndex)
        {
            var chunksComplete = (int)(TotalBlocksWidth * (Convert.ToDouble(curIndex) / TotalCount));

            Console.BackgroundColor = barArgs.ProgressBarColor;
            Console.Write("".PadRight(chunksComplete));

            Console.BackgroundColor = barArgs.ProgressBarUnfilledColor;
            Console.Write("".PadRight(TotalBlocksWidth - chunksComplete));

        }
        
        /// <summary>
        /// Creates the blocks in the bar and creates the n out of n string.
        /// </summary>
        /// <param name="curItem"></param>
        private void DrawOutput(string curItem)
        {
            Console.CursorLeft = TotalBlocksWidth + BlockIncrement;
            Console.BackgroundColor = barArgs.ConsoleBackgroundColor;

            string output = currentIndex.ToString() + " of " + TotalCount.ToString();

            var percentageComplete = (int)(0.5f + ((100f * currentIndex) / TotalCount));

            Console.Write(output.PadRight(15) + $"%{percentageComplete}");
            Console.WriteLine("\n \n");


            ClearCurrentConsoleLine();
            Console.ForegroundColor = barArgs.ProgressCurrentItemColor;
            Console.WriteLine($"{curItem}");
            Console.ForegroundColor = barArgs.ConsoleFontColor;
        }
        
        /// <summary>
        /// Used to clear the current console line from the previous string.
        /// </summary>
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
                timer.Dispose();
            }
        }

        public void Report(double value, string text)
        {
            Interlocked.Exchange(ref currentIndex, value);

            Interlocked.Exchange(ref currentItem, text);
        }
    }
}
