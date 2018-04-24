using System.Linq;
using System.Collections.Generic;

namespace Aquarium
{
    public class Aquarium
    {
        public AquariumObject[,] Territory { get; set; }

        public Aquarium(int X, int Y)
        {
            Territory = new AquariumObject[X, Y];
        }

        public void ObjectIsMovingTo(AquariumObject objectToMove, int newX, int newY)
        {
            if (Territory[newX, newY] != null)
            {
                throw new System.InvalidOperationException("That place is already taken");
            }
            Territory[newX, newY] = objectToMove;
            Territory[objectToMove.X, objectToMove.Y] = null;
        }

        public void ObjectCreatingIn(AquariumObject objectToAppend, int X, int Y)
        {
            if (Territory[X, Y] != null) throw new System.InvalidOperationException("That place is already taken");
            Territory[X, Y] = objectToAppend;
        }

        public void NextIteration()
        {
            List<AquariumObject> objects = new List<AquariumObject>();
            foreach (AquariumObject obj in Territory)
            {
                if (obj is PredatorFish)
                    objects.Add(obj);
            }
            objects.ForEach(p => p.NextIteration());
            objects.Clear();
            foreach (AquariumObject obj in Territory)
            {
                if (obj is HerbivoreFish)
                    objects.Add(obj);
            }
            objects.ForEach(h => h.NextIteration());
            objects.Clear();
            foreach(AquariumObject obj in Territory)
            {
                if (obj is Seaweed)
                    objects.Add(obj);
            }
            objects.ForEach(s => s.NextIteration());
            for (int i=0; i<Territory.GetLength(0); i++)
            {
                for (int j = 0; j < Territory.GetLength(1); j++)
                {
                    if (Territory[i, j]!=null && Territory[i, j].IsDead)
                    {
                        Territory[i, j] = null;
                    }
                }
            }
        }
    }
}
