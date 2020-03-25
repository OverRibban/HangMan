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
            triesLeft = amount; //set tries amount for gues player
        }
        public int GetTries()
        {
            return triesLeft; //return tries amount for gues player
        }
    }
}
