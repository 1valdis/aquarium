using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Aquarium
{
    public abstract class Fish : AquariumObject
    {
        protected virtual int MaxAge { get { return 300; } }
        protected virtual int MaxSatiety { get { return 100; } }
        protected virtual int TermOfPregnancy { get { return 25; } }
        protected virtual int AdulthoodBound { get { return 100; } }
        protected virtual int SatietyDecrease { get { return 3; } }
        protected virtual int MovesInHunger { get { return 10; } }
        protected virtual int MovesPerIteration { get { return 1; } }

        static bool currentGender = false;

        int[,] WaveMap;
        protected int xField, yField, xFinish, yFinish;
        protected System.Collections.Generic.List<Point> Path = new System.Collections.Generic.List<Point>();

        public bool Gender { get; }

        protected int _Age;
        public int Age
        {
            get { return _Age; }
            protected set
            {
                if (MaxAge < value)
                {
                    Die();
                }
                _Age = value;
            }
        }

        protected int _CurrentPregnancy;
        public int CurrentPregnancy
        {
            get { return _CurrentPregnancy; }
            protected set
            {
                if (value > TermOfPregnancy)
                {
                    GiveBirth();
                }
                else
                {
                    _CurrentPregnancy = value;
                }
            }
        }

        protected int _Satiety;
        public int Satiety
        {
            get { return _Satiety; }
            protected set
            {
                if (value < 0)
                {
                    _Satiety = 0;
                    return;
                }
                if (value > MaxSatiety)
                {
                    _Satiety = MaxSatiety;
                    return;
                }
                _Satiety = value;
            }
        }

        protected int _CurrentMovesInHunger;
        public int CurrentMovesInHunger
        {
            get { return _CurrentMovesInHunger; }
            protected set
            {
                if (value > MovesInHunger)
                {
                    Die();
                }
                _CurrentMovesInHunger = value;
            }
        }

        protected Func<AquariumObject, bool> CanBeEaten;
        protected Func<AquariumObject, bool> CanMate;

        public Fish(Aquarium containingAquarium, int x, int y, bool gender, int age = 0, int satiety = 100, int pregnancyTime = 0) : base(containingAquarium, x, y)
        {
            xField = containingAquarium.Territory.GetLength(0);
            yField = containingAquarium.Territory.GetLength(1);
            WaveMap = new int[xField, yField];
            Gender = gender;
            Age = age;
            Satiety = satiety;
            CurrentMovesInHunger = 0;
            CurrentPregnancy = pregnancyTime;
        }

        abstract public override void NextIteration();

        protected AquariumObject GetClosestSuitableObject(Func<AquariumObject, bool> testFor)
        {
            int[,] ClosestWaveMap = new int[xField, yField];
            InitializeWaveMap(ref ClosestWaveMap);
            ClosestWaveMap[X, Y] = 0;
            bool SomethingMarked;
            int CurrentWave = 1;
            do
            {
                SomethingMarked = CreateWave(ref ClosestWaveMap, CurrentWave);
                for (int i = 0; i < xField; i++)
                {
                    for (int j = 0; j < yField; j++)
                    {
                        if ((ClosestWaveMap[i, j] == CurrentWave) && testFor(ContainingAquarium.Territory[i, j]))
                        {
                            return ContainingAquarium.Territory[i, j];
                        }
                    }
                }
                CurrentWave++;
            }
            while (SomethingMarked);
            return null;
        }

        abstract protected void Eat();

        public virtual void Mate()
        {
            Fish couple = GetClosestSuitableObject(CanMate) as Fish;
            if (couple == null)
            {
                throw new Exception("Couple not found");
            }
            if (Math.Abs(this.X-couple.X)==1 && Math.Abs(this.Y - couple.Y) == 1)
            {
                if (Gender == false)
                    Impregnant();
                else
                    couple.Impregnant();
            }
            else
            {
                MakePath(couple.X, couple.Y);
                try
                {
                    ContainingAquarium.ObjectIsMovingTo(this, Path[0].x, Path[0].y);
                    this.X = Path[0].x;
                    this.Y = Path[0].y;
                }
                catch { }
            }
            return;
        }

        protected void Die()
        {
            IsDead = true;
        }

        protected void Impregnant()
        {
            CurrentPregnancy = 1;
        }

        abstract protected void GiveBirth();

        protected void InitializeWaveMap(ref int[,] waveMap)
        {
            for (int i = 0; i < xField; i++)
            {
                for (int j = 0; j < yField; j++)
                {
                    waveMap[i, j] = -1;
                }
            }
        }

        protected void SetupPathmaking(int xFinish, int yFinish)
        {
            this.xFinish = xFinish;
            this.yFinish = yFinish;
            Path.Clear();
            InitializeWaveMap(ref WaveMap);
            WaveMap[X, Y] = 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected bool CreateWave(ref int[,] waveMap, int WaveNumber)
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

        protected void MakePath(int xFinish, int yFinish)
        {
            SetupPathmaking(xFinish, yFinish);
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

        protected void BuildRoute(int WavesCount)
        {
            Point CurrentBacktrace = new Point(xFinish, yFinish);
            int CurrentWave = WavesCount;
            //adding backtrace points into path
            do
            {
                CurrentBacktrace = Backtrace(CurrentBacktrace);
                Path.Add(CurrentBacktrace);
                CurrentWave--;
            }
            while (CurrentWave > 2);
            //reversing path so it starts from start, not end
            Path.Reverse();
        }

        protected Point Backtrace(Point point)
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
        protected bool MarkNearbyPoints(ref int[,] waveMap, int marker, int x, int y)
        {
            //if any neighbour was marked
            bool SomethingMarked = false;
            //mark neighbours 
            for (int i = 0 > x - 1 ? 0 : x - 1, iMax = x + 1 < xField ? x + 1 : xField - 1; i <= iMax; i++)
            {
                for (int j = 0 > y - 1 ? 0 : y - 1, jMax = y + 1 < yField ? y + 1 : yField - 1; j <= jMax; j++)
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
    }
}
