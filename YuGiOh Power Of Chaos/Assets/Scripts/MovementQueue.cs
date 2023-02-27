using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts
{
    internal class MovementQueue
    {
        List<Movement> Queue;

        public MovementQueue()
        {
            Queue = new List<Movement>();
        }

        public void Add(Movement movement)
        {
            Queue.Add(movement);
        }
    }
}
