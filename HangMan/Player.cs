using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HangMan
{
    class Player
    {
        private string name;
        private int wins;
        private int participatedGames;
        public string GetName()
        {
            return name;
        }
        public void SetName(string name)
        {
            this.name = name;
        }
        public int GetWins()
        {
            return wins;
        }
        public void AddWin()
        {
            wins++;
        }
        public int GetParticipatedGames()
        {
            return participatedGames;
        }
        public void AddParticipatedGame()
        {
            participatedGames++;
        }
    }
}
