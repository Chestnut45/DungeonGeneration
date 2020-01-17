using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DungeonGeneration
{
    public class Physics
    {
        int gravCoefficient = 1;
        int maxYvel = 1;

        public void Update(Player p)
        {
            if (p.yvel != maxYvel)
            {
                p.yvel += gravCoefficient;
            }

            p.y += p.yvel;
        }
    }
}
