using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmericanCheackers
{
    internal class Board
    {
        private readonly int r_BoardSize;
        private readonly List<Position> r_PlayerOneCheckerList;
        private readonly List<Position> r_PlayerTwoCheckerList;
        internal Position.eSquareStatus[,] m_GameBoard;

        internal Board(Game.eBoardSize i_BoardSize)
        {
            this.r_BoardSize = (int)i_BoardSize;
            this.m_GameBoard = new Position.eSquareStatus[(int)i_BoardSize, (int)i_BoardSize];
            this.r_PlayerOneCheckerList = new List<Position>();
            this.r_PlayerTwoCheckerList = new List<Position>();
            this.InitializeBoard();
        }

        public int BoardSize
        {
            get { return r_BoardSize; }
        }

        public List<Position> PlayerOneCheckerList
        {
            get { return r_PlayerOneCheckerList; }
        }

        public List<Position> PlayerTwoCheckerList
        {
            get { return r_PlayerTwoCheckerList; }
        }

        public Position.eSquareStatus[,] GameBoard
        {
            get { return m_GameBoard; }
            set { m_GameBoard = value; }
        }

        public enum eLeftRightDirection
        {
            Left = -1,
            Right = 1
        }

        public enum eUpDownDirection
        {
            Up = -1,
            Down = 1
        }

        // the function init the board for start game
        internal void InitializeBoard()
        {
            PlayerOneCheckerList.Clear();
            PlayerTwoCheckerList.Clear();

            for (int i = 0; i < BoardSize; i++)
            {
                for (int j = 0; j < BoardSize; j++)
                {
                    updateCurrentSquere(new Position(i, j));
                }
            }
        }

        // the function updates the value of the cell
        private void updateCurrentSquere(Position i_PositionToUpdate)
        {
            Position.eSquareStatus status = GetStatusForStartByPosition(i_PositionToUpdate);
            GameBoard[i_PositionToUpdate.Row, i_PositionToUpdate.Column] = status;

            if (status == Position.eSquareStatus.PlayerOneChecker)
            {
                PlayerOneCheckerList.Add(i_PositionToUpdate);
            }
            else if (status == Position.eSquareStatus.PlayerTwoChecker)
            {
                PlayerTwoCheckerList.Add(i_PositionToUpdate);
            }
        }

        // the function checks the status of the current cell
        internal Position.eSquareStatus GetStatusForStartByPosition(Position i_PositionToUpdate)
        {
            Position.eSquareStatus status = Position.eSquareStatus.EmptySquere;

            if (isStartPositionForPlayerOne(i_PositionToUpdate))
            {
                status = Position.eSquareStatus.PlayerOneChecker;
            }
            else if (isStartPositionForPlayerTwo(i_PositionToUpdate))
            {
                status = Position.eSquareStatus.PlayerTwoChecker;
            }

            return status;
        }

        // the function checks if its an X player starting place on board (used to init board)
        private bool isStartPositionForPlayerOne(Position i_PositionToCheck)
        {
            int size = BoardSize;
            return ((i_PositionToCheck.Row > (size / 2)) && (i_PositionToCheck.Row % 2 != i_PositionToCheck.Column % 2));
        }

        // the function checks if its an O player starting place on board (used to init board)
        private bool isStartPositionForPlayerTwo(Position i_PositionToCheck)
        {
            int size = BoardSize;
            return ((i_PositionToCheck.Row < (size / 2) - 1) && (i_PositionToCheck.Row % 2 != i_PositionToCheck.Column % 2));
        }

        public bool IsPositionExist(Position i_PositionToCheck)
        {
            return i_PositionToCheck.Row >= 0 && i_PositionToCheck.Row < BoardSize && i_PositionToCheck.Column >= 0 && i_PositionToCheck.Column < BoardSize;
        }

        internal bool CheckIfMiddlePositionIsOppose(Position[] i_sourceAndDestPositions)
        {
            bool isCheckerIsOppose = false;
            Position middlePosition = GetMiddlePosition(i_sourceAndDestPositions);
            Position.eSquareStatus middlePositionStatus = GetMiddlePositionStatus(i_sourceAndDestPositions);
            if (GameBoard[i_sourceAndDestPositions[0].Row, i_sourceAndDestPositions[0].Column] == Position.eSquareStatus.PlayerOneChecker
                || GameBoard[i_sourceAndDestPositions[1].Row, i_sourceAndDestPositions[1].Column] == Position.eSquareStatus.PlayerOneKing)
            {
                if (middlePositionStatus == Position.eSquareStatus.PlayerTwoChecker ||
                    middlePositionStatus == Position.eSquareStatus.PlayerTwoKing)
                {
                    isCheckerIsOppose = true;
                }
            }

            if (GameBoard[i_sourceAndDestPositions[0].Row, i_sourceAndDestPositions[0].Column] == Position.eSquareStatus.PlayerTwoChecker
                || GameBoard[i_sourceAndDestPositions[1].Row, i_sourceAndDestPositions[1].Column] == Position.eSquareStatus.PlayerTwoKing)
            {
                if (middlePositionStatus == Position.eSquareStatus.PlayerOneChecker ||
                    middlePositionStatus == Position.eSquareStatus.PlayerOneKing)
                {
                    isCheckerIsOppose = true;
                }
            }

            return isCheckerIsOppose;
        }

        internal static Position GetMiddlePosition(Position[] i_sourceAndDestPositions)
        {
            int newRow = (i_sourceAndDestPositions[0].Row + i_sourceAndDestPositions[1].Row) / 2;
            int newColumn = (i_sourceAndDestPositions[0].Column + i_sourceAndDestPositions[1].Column) / 2;
            return new Position(newRow, newColumn);
        }

        internal Position.eSquareStatus GetMiddlePositionStatus(Position[] i_sourceAndDestPositions)
        {
            Position middlePosition = GetMiddlePosition(i_sourceAndDestPositions);
            return GameBoard[middlePosition.Row, middlePosition.Column];
        }

        internal void UpdateBoardFromOneMove(Position[] i_SourceAndDestPositions, bool i_isJump)
        {
            Position.eSquareStatus sourceSquareStatus = this.GameBoard[i_SourceAndDestPositions[0].Row, i_SourceAndDestPositions[0].Column];
            if (i_isJump)
            {
                // get middle squere
                int row = (i_SourceAndDestPositions[0].Row + i_SourceAndDestPositions[1].Row) / 2;
                int col = (i_SourceAndDestPositions[0].Column + i_SourceAndDestPositions[1].Column) / 2;
                this.GameBoard[row, col] = Position.eSquareStatus.EmptySquere;
                RemoveEatenPositionFromList(GetMiddlePosition(i_SourceAndDestPositions), GetMiddlePositionStatus(i_SourceAndDestPositions));
            }

            // update source and dest squeres
            this.GameBoard[i_SourceAndDestPositions[0].Row, i_SourceAndDestPositions[0].Column] = Position.eSquareStatus.EmptySquere;
            this.GameBoard[i_SourceAndDestPositions[1].Row, i_SourceAndDestPositions[1].Column] = sourceSquareStatus;
            UpdateCheckersListAfterMove(i_SourceAndDestPositions, sourceSquareStatus);
            UpgradeToKingIfNeeded(i_SourceAndDestPositions[1]);
        }

        internal void UpgradeToKingIfNeeded(Position i_Position)
        {
            if ((i_Position.Row == 0 && (this.GameBoard[i_Position.Row, i_Position.Column] == Position.eSquareStatus.PlayerOneChecker))
               || (i_Position.Row == (BoardSize - 1) && (this.GameBoard[i_Position.Row, i_Position.Column] == Position.eSquareStatus.PlayerTwoChecker)))
            {
                UpgradeToKing(i_Position);
            }
        }

        internal void UpgradeToKing(Position i_Position)
        {
            if (this.GameBoard[i_Position.Row, i_Position.Column] == Position.eSquareStatus.PlayerOneChecker)
            {
                this.GameBoard[i_Position.Row, i_Position.Column] = Position.eSquareStatus.PlayerOneKing;
            }

            if (this.GameBoard[i_Position.Row, i_Position.Column] == Position.eSquareStatus.PlayerTwoChecker)
            {
                this.GameBoard[i_Position.Row, i_Position.Column] = Position.eSquareStatus.PlayerTwoKing;
            }
        }

        internal void UpdateCheckersListAfterMove(Position[] i_SourceAndDestPositions, Position.eSquareStatus i_SourceStatus)
        {
            Position source = i_SourceAndDestPositions[0];
            Position target = i_SourceAndDestPositions[1];
            Position positionToDelete = source;
            if (i_SourceStatus == Position.eSquareStatus.PlayerOneChecker || i_SourceStatus == Position.eSquareStatus.PlayerOneKing)
            {
                foreach (Position position in PlayerOneCheckerList)
                {
                    if (source.Row == position.Row && source.Column == position.Column)
                    {
                        positionToDelete = position;
                    }
                }

                PlayerOneCheckerList.Remove(positionToDelete);
                PlayerOneCheckerList.Add(target);
            }

            if (i_SourceStatus == Position.eSquareStatus.PlayerTwoChecker || i_SourceStatus == Position.eSquareStatus.PlayerTwoKing)
            {
                foreach (Position position in PlayerTwoCheckerList)
                {
                    if (source.Row == position.Row && source.Column == position.Column)
                    {
                        positionToDelete = position;
                    }
                }

                PlayerTwoCheckerList.Remove(positionToDelete);
                PlayerTwoCheckerList.Add(target);
            }
        }

        internal void RemoveEatenPositionFromList(Position i_MiddlePositionToRemove, Position.eSquareStatus i_SquereStatus)
        {
            Position positionToDelete = i_MiddlePositionToRemove;

            if (i_SquereStatus == Position.eSquareStatus.PlayerOneChecker || i_SquereStatus == Position.eSquareStatus.PlayerOneKing)
            {
                foreach (Position position in PlayerOneCheckerList)
                {
                    if (i_MiddlePositionToRemove.Row == position.Row && i_MiddlePositionToRemove.Column == position.Column)
                    {
                        positionToDelete = position;
                    }
                }

                PlayerOneCheckerList.Remove(positionToDelete);
            }

            if (i_SquereStatus == Position.eSquareStatus.PlayerTwoChecker || i_SquereStatus == Position.eSquareStatus.PlayerTwoKing)
            {
                foreach (Position position in PlayerTwoCheckerList)
                {
                    if (i_MiddlePositionToRemove.Row == position.Row && i_MiddlePositionToRemove.Column == position.Column)
                    {
                        positionToDelete = position;
                    }
                }

                PlayerTwoCheckerList.Remove(positionToDelete);
            }
        }

        internal void RemoveFromBoardAfterJump(Position[] i_SourceAndDestPositions)
        {
            int row = (i_SourceAndDestPositions[0].Row + i_SourceAndDestPositions[1].Row) / 2;
            int col = (i_SourceAndDestPositions[0].Column + i_SourceAndDestPositions[1].Column) / 2;
            GameBoard[row, col] = Position.eSquareStatus.EmptySquere;
        }

        internal int GetValueOfPlayerOneCheckers()
        {
            int totalScore = 0;
            foreach (Position position in PlayerOneCheckerList)
            {
                if (position.SquareStatus == Position.eSquareStatus.PlayerOneKing)
                {
                    totalScore += 4;
                }
                else
                {
                    totalScore++;
                }
            }

            return totalScore;
        }

        internal int GetValueOfPlayerTwoCheckers()
        {
            int totalScore = 0;
            foreach (Position position in PlayerTwoCheckerList)
            {
                if (position.SquareStatus == Position.eSquareStatus.PlayerTwoKing)
                {
                    totalScore += 4;
                }
                else
                {
                    totalScore++;
                }
            }

            return totalScore;
        }
    }
}
