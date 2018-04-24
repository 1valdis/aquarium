using System.Drawing;

namespace Aquarium
{
    public class Seaweed : AquariumObject
    {
        protected static readonly int MaxFood = 100;
        protected static readonly int PeriodOfGrowth = 4;
        protected static readonly int FoodIncrease = 10;

        protected int CurrentPeriodOfGrowth=0;

        protected int _CurrentFood;
        public int CurrentFood
        {
            get { return _CurrentFood; }
            protected set
            {
                _CurrentFood = value > MaxFood ? MaxFood : value;
            }
        }

        public Seaweed(Aquarium containingAquarium, int x, int y, int startingFood=50) : base(containingAquarium, x, y)
        {
            CurrentFood = startingFood;
        }

        public override void NextIteration()
        {
            if (IsDead) return;
            if (CurrentPeriodOfGrowth == PeriodOfGrowth)
            {
                CurrentPeriodOfGrowth = 0;
                CurrentFood += FoodIncrease;
                if (CurrentFood == MaxFood)
                    Expand();
            }
            CurrentPeriodOfGrowth++;
        }

        protected void Expand()
        {
            AquariumObject[,] territory = ContainingAquarium.Territory;
            try
            {
                Point place = GetFreePlace(X, Y);
                ContainingAquarium.ObjectCreatingIn(new Seaweed(ContainingAquarium, place.x, place.y, 0), place.x, place.y);
            }
            catch { }
            finally
            {
                CurrentPeriodOfGrowth = 0;
            }
        }

        public int Feed(int foodNeeded)
        {
            if (foodNeeded >= CurrentFood)
            {
                IsDead = true;
                foodNeeded = CurrentFood;
                CurrentFood = 0;
                return foodNeeded;
            }
            else
            {
                CurrentFood -= foodNeeded;
                return foodNeeded;
            }
        }

        public override Bitmap Image
        {
            get
            {
                Bitmap bmp = new Bitmap(Properties.Resources.seaweed);
                using (Graphics gr = Graphics.FromImage(bmp))
                {
                    gr.DrawString(CurrentFood.ToString(), new Font("Arial Black", 40), Brushes.Black, 5, 5);
                }
                return bmp;
            }
        }
    }
}
