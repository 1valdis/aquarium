using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aquarium
{
    public abstract class AquariumObject
    {
        public int X { get; protected set; }
        public int Y { get; protected set; }
        protected Aquarium ContainingAquarium;
        protected bool _IsDead;
        public bool IsDead
        {
            get
            {
                return _IsDead;
            }
            protected set
            {
                _IsDead = value;
            }
        }
        public abstract void NextIteration();
        public abstract System.Drawing.Bitmap Image { get; }
        public AquariumObject(Aquarium containingAquarium, int x, int y)
        {
            X = x;
            Y = y;
            ContainingAquarium = containingAquarium;
            IsDead = false;
        }
        protected Point GetFreePlace(int x, int y)
        {
            int xField = ContainingAquarium.Territory.GetLength(0);
            int yField = ContainingAquarium.Territory.GetLength(1);
            for (int i = 0 > x - 1 ? 0 : x - 1, iMax = x + 1 < xField ? x + 1 : xField - 1; i <= iMax; i++)
            {
                for (int j = 0 > y - 1 ? 0 : y - 1, jMax = y + 1 < yField ? y + 1 : yField - 1; j <= jMax; j++)
                {
                    if (ContainingAquarium.Territory[i, j] == null)
                    {
                        return new Point(i, j);
                    }
                }
            }
            throw new Exception("No free place found");
        }
    }
}
