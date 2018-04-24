using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace Aquarium
{
    public class Rock : AquariumObject
    {
        public Rock(Aquarium containingAquarium, int x, int y) : base(containingAquarium, x, y)
        {

        }

        public override void NextIteration()
        {
            
        }

        public override Bitmap Image
        {
            get
            {
                return Properties.Resources.rock;
            }
        }
    }
}
