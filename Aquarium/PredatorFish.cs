using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace Aquarium
{
    public class PredatorFish : Fish
    {
        override protected int MovesPerIteration { get { return 2; } }
        protected override int MaxAge => 200;
        protected override int SatietyDecrease => 2;
        public PredatorFish(Aquarium containingAquarium, int x, int y, bool gender, int age, int satiety, int pregnancyTime = 0) : base(containingAquarium, x, y, gender, age, satiety, pregnancyTime)
        {
            CanMate = new Func<AquariumObject, bool>((AquariumObject obj) =>
            {
                Fish found = obj as PredatorFish;
                if (found != null && (GetType() == obj.GetType()) && found.Age >= AdulthoodBound && found.Gender != this.Gender && found.CurrentPregnancy == 0)
                {
                    return true;
                }
                return false;
            });
            CanBeEaten = new Func<AquariumObject, bool>((AquariumObject obj) =>
            {
                if (obj is HerbivoreFish)
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
            Age++;
            for (int i=0; i<MovesPerIteration; i++)
            {
                if (Satiety == 0)
                    CurrentMovesInHunger++;
                if (CurrentMovesInHunger > MovesInHunger)
                {
                    Die();
                }
                if (Satiety < 30)
                {
                    try
                    {
                        Eat();
                        continue;
                    }
                    catch { }
                }
                if (CurrentPregnancy == 0)
                {
                    try
                    {
                        Mate();
                        continue;
                    }
                    catch { }
                }
            }
        }

        protected override void Eat()
        {
            HerbivoreFish food = GetClosestSuitableObject(CanBeEaten) as HerbivoreFish;
            if (food == null)
            {
                throw new Exception("Food not found");
            }
            if (Math.Abs(this.X - food.X) <= 1 && Math.Abs(this.Y - food.Y) <= 1)
            {
                Satiety += food.Feed();
            }
            else
            {
                MakePath(food.X, food.Y);
                try
                {
                    ContainingAquarium.Territory[Path[0].x, Path[0].y] = this;
                    ContainingAquarium.Territory[X, Y] = null;
                    this.X = Path[0].x;
                    this.Y = Path[0].y;
                }
                catch { }
                food.BeingHuntedBy(this);
            }
        }

        protected override void GiveBirth()
        {
            try
            {
                Point place = GetFreePlace(X, Y);
                ContainingAquarium.Territory[place.x, place.y] = new PredatorFish(ContainingAquarium, place.x, place.y, (new Random()).Next(2) == 1, 0, 100);
            }
            catch { }
            finally
            {
                CurrentPregnancy = 0;
            }
            return;
        }

        public override Bitmap Image
        {
            get
            {
                Bitmap bmp = new Bitmap(Properties.Resources.predator);
                using (Graphics gr = Graphics.FromImage(bmp))
                {
                    gr.DrawString(Satiety.ToString(), new Font("Arial Black", 40), Brushes.Black, 5, 5);
                }
                return bmp;
            }
        }
    }
}
