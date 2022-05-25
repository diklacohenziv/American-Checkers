using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmericanCheackers
{
    internal class Position
    {
        private eSquareStatus m_squareStatus;
        private int m_Row;
        private int m_Column;

        internal enum eSquareStatus
        {
            EmptySquere = ' ',
            PlayerOneChecker = 'O',
            PlayerTwoChecker = 'X',
            PlayerOneKing = 'Q',
            PlayerTwoKing = 'Z',
        }

        internal enum eTypeOfMove
        {
            SimpleMove = 0,
            EatingMove = 1,
            IllegalMove = 2,
        }

        public Position(int i_Row, int i_Column)
        {
            this.m_Row = i_Row;
            this.m_Column = i_Column;
        }

        public int Row
        {
            get { return m_Row; }
        }

        public int Column
        {
            get { return m_Column; }
        }

        public eSquareStatus SquareStatus
        {
            get { return m_squareStatus; }
            set { m_squareStatus = value; }
        }

        public override string ToString()
        {
            return ((char)SquareStatus).ToString();
        }
    }
}
