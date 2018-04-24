using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace Aquarium
{
    public class HerbivoreFish : Fish
    {
        protected virtual int MaxFood { get { return 100; } }
        protected virtual int FoodIncrease { get { return 5; } }
        protected override int TermOfPregnancy => 10;
        protected virtual PredatorFish Hunter { get; set; }

        protected int _Food;
        public int Food
        {
            get { return _Food; }
            protected set
            {
                _Food = value > MaxFood ? MaxFood : value;
            }
        }

        public HerbivoreFish(Aquarium containingAquarium, int x, int y, bool gender, int age, int satiety, int food=5, int pregnancyTime = 0) : base(containingAquarium, x, y, gender, age, satiety, pregnancyTime)
        {
            Food = food;
            CanMate = new Func<AquariumObject, bool>((AquariumObject obj) =>
            {
                Fish found = obj as HerbivoreFish;
                if (found != null && (GetType() == obj.GetType()) && found.Age >= AdulthoodBound && found.Gender != this.Gender && found.CurrentPregnancy == 0)
                {
                    return true;
                }
                return false;
            });
            CanBeEaten = new Func<AquariumObject, bool>((AquariumObject obj) =>
            {
                Seaweed found = obj as Seaweed;
                if (found != null)
                {
                    return true;
                }
                return false;
            });
        }

        public override void NextIteration()
        {
            if (IsDead) return;
            CurrentPregnancy = CurrentPregnancy == 0 ? 0 : CurrentPregnancy + 1;
            Satiety -= SatietyDecrease;
            if (Satiety == 0)
                CurrentMovesInHunger++;
            if (CurrentMovesInHunger > MovesInHunger)
            {
                Die();
                return;
            }
            Food += FoodIncrease;
            Age++;
            if (Hunter != null)
            {
                try
                {
                    Escape(Hunter);
                    Hunter = null;
                    return;
                }
                catch { }
            }
            if (Satiety < 30)
            {
                try
                {
                    Eat();
                    return;
                }
                catch { }
            }
            if (CurrentPregnancy == 0)
            {
                try
                {
                    Mate();
                    return;
                }
                catch { }
            }
            try
            {
                AvoidHunters();
                return;
            }
            catch { }
        }

        protected override void Eat()
        {
            Seaweed food = GetClosestSuitableObject(CanBeEaten) as Seaweed;
            if (food == null)
            {
                throw new Exception("Food not found");
            }
            if (Math.Abs(this.X - food.X) <= 1 && Math.Abs(this.Y - food.Y) <= 1)
            {
                Satiety += food.Feed(MaxSatiety - Satiety);
            }
            else
            {
                MakePath(food.X, food.Y);
                ContainingAquarium.ObjectIsMovingTo(this, Path[0].x, Path[0].y);
                this.X = Path[0].x; this.Y = Path[0].y;
            }
        }

        public int Feed()
        {
            Die();
            return Food;
        }

        public void BeingHuntedBy(PredatorFish hunter)
        {
            Hunter = hunter;
        }

        public void Escape(AquariumObject Hunter)
        {
            Pathfinder pathfinder = new Pathfinder(ContainingAquarium.Territory);
            int[,] WaveMap = pathfinder.GetWavemap(Hunter.X, Hunter.Y);
            int xMax = 0, yMax = 0, MaxWave = 0;
            for (int i = 0 > X - 1 ? 0 : X - 1, iMax = X + 1 < xField ? X + 1 : xField - 1; i <= iMax; i++)
            {
                for (int j = 0 > Y - 1 ? 0 : Y - 1, jMax = Y + 1 < yField ? Y + 1 : yField - 1; j <= jMax; j++)
                {
                    if (MaxWave < WaveMap[i, j] && ContainingAquarium.Territory[i, j]==null)
                    {
                        MaxWave = WaveMap[i, j];
                        xMax = i;
                        yMax = j;
                    }
                }
            }
            if (MaxWave == 0)
            {
                throw new Exception("Cannot escape death.");
            }
            try
            {
                ContainingAquarium.ObjectIsMovingTo(this, xMax, yMax);
                this.X = xMax;
                this.Y = yMax;
            }
            catch { }
        }

        protected void AvoidHunters()
        {
            PredatorFish ClosestHunter = GetClosestSuitableObject(obj => obj is PredatorFish) as PredatorFish;
            if (ClosestHunter == null || Math.Sqrt(Math.Pow(this.X - ClosestHunter.X, 2) + Math.Pow(this.X - ClosestHunter.X, 2)) > 10)
            {
                throw new Exception("No close hunters");
            }
            else Escape(ClosestHunter);
        }

        protected override void GiveBirth()
        {
            Point place = GetFreePlace(X, Y);
            ContainingAquarium.ObjectCreatingIn(new HerbivoreFish(ContainingAquarium, place.x, place.y, (new Random()).Next(2) == 1, 0, 100), place.x, place.y);
            CurrentPregnancy = 0;
            return;
        }
        public override Bitmap Image
        {
            get
            {
                Bitmap bmp = new Bitmap(Properties.Resources.herbivore);
                using (Graphics gr = Graphics.FromImage(bmp))
                {
                    gr.DrawString(Satiety.ToString(), new Font("Arial Black", 40), Brushes.Black, 5, 5);
                    gr.DrawString(Food.ToString(), new Font("Arial Black", 40), Brushes.Black, 5, 50);
                }
                return bmp;
            }
        }
    }
}
