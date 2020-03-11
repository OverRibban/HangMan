using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HangMan
{
    class GuestPlayer : Player
    {
        private int triesLeft;
        public void SetTries(int amount)
        {
            triesLeft = amount;
        }
        public int GetTries()
        {
            return triesLeft;
        }
    }
}
