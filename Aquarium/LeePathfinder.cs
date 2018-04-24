using System.Linq;
using System.Runtime.CompilerServices;

namespace Aquarium
{
    public class Pathfinder
    {
        private AquariumObject[,] Territory;
        private int[,] WaveMap;
        private int xField, yField, xStart, yStart, xFinish, yFinish;
        private System.Collections.Generic.List<Point> Path = new System.Collections.Generic.List<Point>();
        private struct Point
        {
            public Point(int x, int y)
            {
                this.x = x;
                this.y = y;
            }
            public int x, y;
        }

        public Pathfinder(AquariumObject[,] territory)
        {
            Territory = territory;
            xField = territory.GetLength(0); yField = territory.GetLength(1);
            WaveMap = new int[xField, yField];
        }

        private void InitializeWaveMap(ref int[,] waveMap)
        {
            for (int i = 0; i < xField; i++)
            {
                for (int j = 0; j < yField; j++)
                {
                    waveMap[i, j] = -1;
                }
            }
        }

        private void SetupPathmaking(int xStart, int yStart, int xFinish=0, int yFinish=0)
        {
            this.xStart = xStart;
            this.yStart = yStart;
            this.xFinish = xFinish;
            this.yFinish = yFinish;
            Path.Clear();
            InitializeWaveMap(ref WaveMap);
            WaveMap[xStart, yStart] = 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool CreateWave(ref int [,] waveMap, int WaveNumber)
        {
            bool SomethingMarked = false;
            for (int i = 0; i < xField; i++)
            {
                for (int j = 0; j < yField; j++)
                {
                    if (waveMap[i, j] == WaveNumber - 1)
                    {
                        SomethingMarked = MarkNearbyPoints(ref waveMap, WaveNumber, i, j) || SomethingMarked;
                    }
                }
            }
            return SomethingMarked;
        }

        public void MakePath(int xStart, int yStart, int xFinish, int yFinish)
        {
            SetupPathmaking(xStart, yStart, xFinish, yFinish);
            int CurrentWave = 1;
            bool SomethingMarked, FinishFound = false;
            //marking neighbour points (creating waves)
            do
            {
                SomethingMarked = CreateWave(ref WaveMap, CurrentWave);
                FinishFound = WaveMap[xFinish, yFinish] != -1;
                CurrentWave++;
            }
            while (SomethingMarked && !FinishFound);
            if (!FinishFound)
                throw new System.Exception("Route isn't found.");
            BuildRoute(CurrentWave);
        }

        public System.Tuple<int, int> GetStep(int Step)
        {
            return new System.Tuple<int, int>(Path[Step].x, Path[Step].y);
        }

        public bool IsPath(int x, int y)
        {
            //search coordinate pair in list
            for (int i=0; i<Path.Count; i++)
            {
                if (Path[i].x == x && Path[i].y == y)
                    return true;
            }
            return false;
        }

        public int Steps { get { return Path.Count; } }

        private void BuildRoute(int WavesCount)
        {
            Point CurrentBacktrace = new Point(xFinish, yFinish);
            int CurrentWave = WavesCount;
            //adding backtrace points into path
            do
            {
                Path.Add(CurrentBacktrace);
                CurrentBacktrace = Backtrace(CurrentBacktrace);
            }
            while (CurrentWave > 0);
            //reversing path so it starts from start, not end
            Path.Reverse();
        }

        private Point Backtrace(Point point)
        {
            //searching neighbour with lower wave index
            for (int i = 0 > point.x - 1 ? 0 : point.x - 1, iMax = point.x + 1 < xField ? point.x + 1 : xField - 1; i <= iMax; i++)
            {
                for (int j = 0 > point.y - 1 ? 0 : point.y - 1, jMax = point.y + 1 < yField ? point.y + 1 : yField - 1; j <= jMax; j++)
                {
                    //returning coordinates of point with lower wave index
                    if (WaveMap[i, j] == WaveMap[point.x, point.y] - 1)
                    {
                        return new Point(i, j);
                    }
                }
            }
            //this should not happen but in case
            throw new System.Exception("Error during backtracing");
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool MarkNearbyPoints(ref int[,] waveMap, int marker, int x, int y)
        {
            //if any neighbour was marked
            bool SomethingMarked = false;
            //mark neighbours 
            for (int i = 0 > x - 1 ? 0 : x - 1, iMax = x + 1 < xField ? x + 1 : xField-1; i <= iMax; i++)
            {
                for (int j = 0 > y - 1 ? 0 : y - 1, jMax = y + 1 < yField ? y + 1 : yField-1; j <= jMax; j++)
                {
                    if (waveMap[i, j] == -1)
                    {
                        waveMap[i, j] = marker;
                        SomethingMarked = true;
                    }
                }
            }
            return SomethingMarked;
        }

        public int[,] GetWavemap(int X, int Y)
        {
            int[,] WaveMap = new int[xField, yField];
            InitializeWaveMap(ref WaveMap);
            WaveMap[X, Y] = 0;
            int CurrentWave = 1;
            bool SomethingMarked;
            //marking neighbour points (creating waves)
            do
            {
                SomethingMarked = CreateWave(ref WaveMap, CurrentWave);
                CurrentWave++;
            }
            while (SomethingMarked);
            return WaveMap;
        }
    }
}
