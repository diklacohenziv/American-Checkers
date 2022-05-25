using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmericanCheackers
{
    internal class Player
    {
        private readonly string r_PlayerName;
        private int m_NumberOfActiveCheckers;
        private int m_score;

        internal Player(string i_PlayerName, int i_NumberOfCheckers)
        {
            this.r_PlayerName = i_PlayerName;
            this.m_NumberOfActiveCheckers = i_NumberOfCheckers;
        }

        public string Name
        {
            get { return r_PlayerName; }
        }

        public int Score
        {
            get { return m_score; }
            set { m_score = value; }
        }

        public int NumberOfActiveCheckes
        {
            get { return m_NumberOfActiveCheckers; }
            set { m_NumberOfActiveCheckers = value; }
        }

        public static bool CheckNameValidation(string i_PlayerName) // move this function to Player
        {
            bool isValidName = true;
            int lenOfString = i_PlayerName.Length;

            if (lenOfString <= 10)
            {
                foreach (char c in i_PlayerName)
                {
                    isValidName = !c.Equals(' ');
                }
            }

            return isValidName && (lenOfString <= 10);
        }
    }
}
