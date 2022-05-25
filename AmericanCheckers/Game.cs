using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmericanCheackers
{
    internal class Game
    {

        private readonly eGameType r_eGameType;
        private readonly eBoardSize r_eBoardSize;
        private readonly Player r_FirstPlayer;
        private readonly Player r_SecondPlayer;
        private Board m_board;

        public eGameType GameType
        {
            get { return r_eGameType; }
        }

        public eBoardSize BoardSize
        {
            get { return r_eBoardSize; }
        }

        public Player FirstPlayer
        {
            get { return r_FirstPlayer; }
        }

        public Player SecondPlayer
        {
            get { return r_SecondPlayer; }
        }

        public Board BoardOfGame
        {
            get { return m_board; }
            set { m_board = value; }
        }

        internal enum eBoardSize
        {
            Small = 6,
            Medium = 8,
            Large = 10,
        }

        internal enum eGameType
        {
            TwoPlayes,
            SinglePlayer,
        }

        public enum ePlayerTurn
        {
            PlayerOneTurn = 1,
            PlayerTwoTurn = 2,
        }

        public enum eGameOverStatus
        {
            Surrended = 0,
            Win = 1,
            BothStuck = 2
        }

        public Game(eBoardSize i_eBoardSize, eGameType i_eGameType, string i_FirstPlayerName, string i_SecondPlayerName)
        {
            int numOfStartingSoldiers = ((int)i_eBoardSize / 2) * (((int)i_eBoardSize / 2) - 1);
            this.r_eBoardSize = i_eBoardSize;
            this.r_eGameType = i_eGameType;
            this.m_board = new Board(i_eBoardSize);
            this.r_FirstPlayer = new Player(i_FirstPlayerName, numOfStartingSoldiers);
            this.r_SecondPlayer = new Player(i_SecondPlayerName, numOfStartingSoldiers);
        }

        internal string GetNameOfCurrentPlayer(ePlayerTurn i_PlayerTurn)
        {
            string currentPlayerName = string.Empty;

            if (i_PlayerTurn == ePlayerTurn.PlayerOneTurn)
            {
                currentPlayerName = FirstPlayer.Name;
            }
            else
            {
                currentPlayerName = SecondPlayer.Name;
            }

            return currentPlayerName;
        }

        public void ResetGame()
        {
            int numOfStartingSoldiers = ((int)r_eBoardSize / 2) * (((int)r_eBoardSize / 2) - 1);
            BoardOfGame.InitializeBoard();
            r_FirstPlayer.NumberOfActiveCheckes = numOfStartingSoldiers;
            r_SecondPlayer.NumberOfActiveCheckes = numOfStartingSoldiers;
        }

        internal void GameOverUpdate(eGameOverStatus i_GameOverStatus, ePlayerTurn i_PlayerTurn)
        {
            int totalScoreSub = Math.Abs(BoardOfGame.GetValueOfPlayerOneCheckers() - BoardOfGame.GetValueOfPlayerTwoCheckers());

            if (i_GameOverStatus == eGameOverStatus.Surrended)
            {
                if (i_PlayerTurn == ePlayerTurn.PlayerOneTurn)
                {
                    SecondPlayer.Score = SecondPlayer.Score + totalScoreSub;
                }
                else
                {
                    FirstPlayer.Score = FirstPlayer.Score + totalScoreSub;
                }
            }
            else if (i_GameOverStatus == eGameOverStatus.Win)
            {
                if (i_PlayerTurn == ePlayerTurn.PlayerOneTurn)
                {
                    FirstPlayer.Score = FirstPlayer.Score + totalScoreSub;
                }
                else
                {
                    SecondPlayer.Score = SecondPlayer.Score + totalScoreSub;
                }
            }
        }

        private List<Position> getListOfCheckersByPlayer(Player i_Player)
        {
            List<Position> listOfCheckersByPlayers;
            if (i_Player == FirstPlayer)
            {
                listOfCheckersByPlayers = BoardOfGame.PlayerOneCheckerList;
            }
            else
            {
                listOfCheckersByPlayers = BoardOfGame.PlayerTwoCheckerList;
            }

            return listOfCheckersByPlayers;
        }

        private List<Position> getListOfCheckersByPlayerTurn(ePlayerTurn i_PlayerTurn)
        {
            List<Position> listOfCheckersByPlayerTurn;
            if (i_PlayerTurn == ePlayerTurn.PlayerOneTurn)
            {
                listOfCheckersByPlayerTurn = getListOfCheckersByPlayer(FirstPlayer);
            }
            else
            {
                listOfCheckersByPlayerTurn = getListOfCheckersByPlayer(SecondPlayer);
            }

            return listOfCheckersByPlayerTurn;
        }

        internal bool IsCheckerCanMove(Position i_SourcePosition, ePlayerTurn i_PlayerTurn, Board.eLeftRightDirection i_LeftOrRight, Board.eUpDownDirection i_UpOrDown)
        {
            bool isMoveAvailable = false;
            Position destinationPosition = new Position(i_SourcePosition.Row + (int)i_UpOrDown, i_SourcePosition.Column + (int)i_LeftOrRight);

            if (BoardOfGame.IsPositionExist(destinationPosition))
            {
                Position.eSquareStatus destinationStatus = BoardOfGame.GameBoard[destinationPosition.Row, destinationPosition.Column];
                isMoveAvailable = destinationStatus == Position.eSquareStatus.EmptySquere;
            }

            return isMoveAvailable;
        }

        // this function gets player turn and return if he must eat or not
        internal bool CheckIfPlayerHaveToEatByTurn(ePlayerTurn i_PlayerTurn)
        {
            bool isPlayerHaveToEat = false;

            if (i_PlayerTurn == ePlayerTurn.PlayerOneTurn)
            {
                isPlayerHaveToEat = CheckIfPlayerCanEat(FirstPlayer);
            }
            else
            {
                isPlayerHaveToEat = CheckIfPlayerCanEat(SecondPlayer);
            }

            return isPlayerHaveToEat;
        }

        // this function gets a player object and return if he can eat
        internal bool CheckIfPlayerCanEat(Player i_PlayerToCheck)
        {
            bool isPlayerCanEat = false;
            List<Position> positionListToIter = getListOfCheckersByPlayer(i_PlayerToCheck);

            foreach (Position position in positionListToIter)
            {
                if (CheckIfCheckerCanEat(position))
                {
                    isPlayerCanEat = true;
                }
            }

            return isPlayerCanEat;
        }

        internal bool CheckIfCheckerCanEat(Position i_Position)
        {
            bool isCheckerCanEat = false;

            if (BoardOfGame.GameBoard[i_Position.Row, i_Position.Column] == Position.eSquareStatus.PlayerOneKing ||
                BoardOfGame.GameBoard[i_Position.Row, i_Position.Column] == Position.eSquareStatus.PlayerTwoKing)
            {
                isCheckerCanEat = CheckIfKingCanEat(i_Position);
            }
            else
            {
                isCheckerCanEat = CheckIfSimpleCheckerCanEat(i_Position);
            }

            return isCheckerCanEat;
        }

        internal bool CheckIfSimpleCheckerCanEat(Position i_Position)
        {
            bool isCheckerCanEat = false;
            bool boolVarToSend = false;
            Position.eSquareStatus status = BoardOfGame.GameBoard[i_Position.Row, i_Position.Column];

            if (status == Position.eSquareStatus.PlayerOneChecker)
            {
                Position firstEatOption = new Position(i_Position.Row - 2, i_Position.Column - 2);
                Position secondEatOption = new Position(i_Position.Row - 2, i_Position.Column + 2);
                if (BoardOfGame.IsPositionExist(firstEatOption))
                {
                    if (GetTypeOfMove(i_Position, firstEatOption, ref boolVarToSend) == Position.eTypeOfMove.EatingMove)
                    {
                        isCheckerCanEat = true;
                    }
                }

                if (BoardOfGame.IsPositionExist(secondEatOption))
                {
                    if (GetTypeOfMove(i_Position, secondEatOption, ref boolVarToSend) == Position.eTypeOfMove.EatingMove)
                    {
                        isCheckerCanEat = true;
                    }
                }
            }

            if (status == Position.eSquareStatus.PlayerTwoChecker)
            {
                Position firstEatOption = new Position(i_Position.Row + 2, i_Position.Column + 2);
                Position secondEatOption = new Position(i_Position.Row + 2, i_Position.Column - 2);
                if (BoardOfGame.IsPositionExist(firstEatOption))
                {
                    if (GetTypeOfMove(i_Position, firstEatOption, ref boolVarToSend) == Position.eTypeOfMove.EatingMove)
                    {
                        isCheckerCanEat = true;
                    }
                }

                if (BoardOfGame.IsPositionExist(secondEatOption))
                {
                    if (GetTypeOfMove(i_Position, secondEatOption, ref boolVarToSend) == Position.eTypeOfMove.EatingMove)
                    {
                        isCheckerCanEat = true;
                    }
                }

            }

            return isCheckerCanEat;
        }

        internal bool CheckIfKingCanEat(Position i_Position)
        {
            bool isKingCanEat = false;
            bool boolVarToSend = false;
            Position firstEatOption = new Position(i_Position.Row + 2, i_Position.Column + 2);
            Position secondEatOption = new Position(i_Position.Row + 2, i_Position.Column - 2);
            Position thirdEatOption = new Position(i_Position.Row - 2, i_Position.Column - 2);
            Position fourthEatOption = new Position(i_Position.Row - 2, i_Position.Column + 2);

            if (BoardOfGame.IsPositionExist(firstEatOption) || BoardOfGame.IsPositionExist(secondEatOption)
                || BoardOfGame.IsPositionExist(thirdEatOption) || BoardOfGame.IsPositionExist(fourthEatOption))
            {
                if (BoardOfGame.IsPositionExist(firstEatOption) && GetTypeOfMove(i_Position, firstEatOption, ref boolVarToSend) == Position.eTypeOfMove.EatingMove)
                {
                    isKingCanEat = true;
                }

                if (BoardOfGame.IsPositionExist(secondEatOption) && GetTypeOfMove(i_Position, secondEatOption, ref boolVarToSend) == Position.eTypeOfMove.EatingMove)
                {
                    isKingCanEat = true;
                }

                if (BoardOfGame.IsPositionExist(thirdEatOption) && GetTypeOfMove(i_Position, thirdEatOption, ref boolVarToSend) == Position.eTypeOfMove.EatingMove)
                {
                    isKingCanEat = true;
                }

                if (BoardOfGame.IsPositionExist(fourthEatOption) && GetTypeOfMove(i_Position, fourthEatOption, ref boolVarToSend) == Position.eTypeOfMove.EatingMove)
                {
                    isKingCanEat = true;
                }
            }

            return isKingCanEat;
        }


        internal bool CheckIfPlayerIsStuckByTurn(ePlayerTurn i_PlayerTurn)
        {
            bool isPlayerNotStuckCheckNumberOne = false;
            bool isPlayerNotStuckCheckNumberTwo = false;
            bool isPlayerNotStuckCheckNumberThree = false;
            bool isPlayerStuck = true;
            bool isPositionIsKing = false;
            List<Position> positionListByPlayer = getListOfCheckersByPlayerTurn(i_PlayerTurn);

            foreach (Position position in positionListByPlayer)
            {
                isPositionIsKing = BoardOfGame.GameBoard[position.Row, position.Column] == Position.eSquareStatus.PlayerOneKing ||
                                          BoardOfGame.GameBoard[position.Row, position.Column] == Position.eSquareStatus.PlayerOneKing;

                if (i_PlayerTurn == ePlayerTurn.PlayerOneTurn || isPositionIsKing)
                {
                    isPlayerNotStuckCheckNumberOne = IsCheckerCanMove(position, i_PlayerTurn, Board.eLeftRightDirection.Left, Board.eUpDownDirection.Up);
                    isPlayerNotStuckCheckNumberTwo = IsCheckerCanMove(position, i_PlayerTurn, Board.eLeftRightDirection.Right, Board.eUpDownDirection.Up);
                    isPlayerNotStuckCheckNumberThree = CheckIfCheckerCanEat(position);
                    if (isPlayerNotStuckCheckNumberOne || isPlayerNotStuckCheckNumberTwo || isPlayerNotStuckCheckNumberThree)
                    {
                        isPlayerStuck = false;
                        break;
                    }
                }
                else if (i_PlayerTurn == ePlayerTurn.PlayerTwoTurn || isPositionIsKing)
                {
                    isPlayerNotStuckCheckNumberOne = IsCheckerCanMove(position, i_PlayerTurn, Board.eLeftRightDirection.Left, Board.eUpDownDirection.Down);
                    isPlayerNotStuckCheckNumberTwo = IsCheckerCanMove(position, i_PlayerTurn, Board.eLeftRightDirection.Right, Board.eUpDownDirection.Down);
                    isPlayerNotStuckCheckNumberThree = CheckIfCheckerCanEat(position);
                    if (isPlayerNotStuckCheckNumberOne || isPlayerNotStuckCheckNumberTwo || isPlayerNotStuckCheckNumberThree)
                    {
                        isPlayerStuck = false;
                        break;
                    }
                }
            }

            return isPlayerStuck;
        }

        internal bool CheckIfBothPlayersStuck(ePlayerTurn i_PlayerTurn)
        {
            bool isPlayerOneStuck = false;
            bool isPlayerTwoStuck = false;

            isPlayerOneStuck = CheckIfPlayerIsStuckByTurn(ePlayerTurn.PlayerTwoTurn);
            isPlayerTwoStuck = CheckIfPlayerIsStuckByTurn(ePlayerTurn.PlayerTwoTurn);

            return isPlayerOneStuck && isPlayerTwoStuck;
        }

        internal bool CheckIfSameCheckerMustEatAgain(Player i_Player, Position i_Position, ref bool o_IsMustEatWithSameChecker)
        {
            return CheckIfCheckerCanEat(i_Position);
        }

        internal static void ChangePlyaerTurn(ref Game.ePlayerTurn i_PlayerTurn)
        {
            if (i_PlayerTurn == Game.ePlayerTurn.PlayerOneTurn)
            {
                i_PlayerTurn = Game.ePlayerTurn.PlayerTwoTurn;
            }
            else
            {
                i_PlayerTurn = Game.ePlayerTurn.PlayerOneTurn;
            }
        }

        internal bool CheckInputMoveAvailability(Position[] i_SourceAndDestPositions, ePlayerTurn i_PlaterTurn, ref bool o_IsJump)
        {
            bool isValidMove = false;
            bool isSourceValid = CheckeIfSourceValid(i_SourceAndDestPositions[0], i_PlaterTurn);
            bool isDestValid = CheckeIfDestValid(i_SourceAndDestPositions[1], i_PlaterTurn);
            if (isSourceValid && isDestValid)
            {
                isValidMove = CheckIfMoveIsValid(i_SourceAndDestPositions, ref o_IsJump);
            }

            return isValidMove;
        }

        internal bool CheckeIfSourceValid(Position i_SourcePositions, ePlayerTurn i_PlaterTurn)
        {
            bool isSourceBelongToPlayer = false;
            bool isSourceInBoundries = BoardOfGame.IsPositionExist(i_SourcePositions);

            if (i_PlaterTurn == ePlayerTurn.PlayerOneTurn)
            {
                Position.eSquareStatus sourceStatus = BoardOfGame.GameBoard[i_SourcePositions.Row, i_SourcePositions.Column];
                if (sourceStatus == Position.eSquareStatus.PlayerOneChecker || sourceStatus == Position.eSquareStatus.PlayerOneKing)
                {
                    isSourceBelongToPlayer = true;
                }
            }

            if (i_PlaterTurn == ePlayerTurn.PlayerTwoTurn)
            {
                Position.eSquareStatus sourceStatus = BoardOfGame.GameBoard[i_SourcePositions.Row, i_SourcePositions.Column];
                if (sourceStatus == Position.eSquareStatus.PlayerTwoChecker || sourceStatus == Position.eSquareStatus.PlayerTwoKing)
                {
                    isSourceBelongToPlayer = true;
                }
            }

            return isSourceBelongToPlayer && isSourceInBoundries;
        }

        internal bool CheckeIfDestValid(Position i_DestPositions, ePlayerTurn i_PlaterTurn)
        {
            bool isDestSquareAvailable = BoardOfGame.GameBoard[i_DestPositions.Row, i_DestPositions.Column] == Position.eSquareStatus.EmptySquere;
            bool isSourceInBoundries = BoardOfGame.IsPositionExist(i_DestPositions);

            return isDestSquareAvailable && isSourceInBoundries;
        }

        internal bool CheckIfMoveIsValid(Position[] i_SourceAndDestPositions, ref bool o_IsJump)
        {
            bool isValidMoveForPlayer = true;
            Position source = i_SourceAndDestPositions[0];
            Position target = i_SourceAndDestPositions[1];
            Position.eTypeOfMove typeOfMove = GetTypeOfMove(source, target, ref o_IsJump);
            if (typeOfMove == Position.eTypeOfMove.IllegalMove)
            {
                isValidMoveForPlayer = false;
            }

            return isValidMoveForPlayer;
        }

        internal Position.eTypeOfMove GetTypeOfMove(Position i_Source, Position i_Target, ref bool o_IsJump)
        {
            Position.eTypeOfMove typeOfMove = Position.eTypeOfMove.SimpleMove;

            Position.eSquareStatus sourceStatus = BoardOfGame.GameBoard[i_Source.Row, i_Source.Column];

            switch (sourceStatus)
            {
                case Position.eSquareStatus.EmptySquere:
                    {
                        typeOfMove = Position.eTypeOfMove.IllegalMove;
                        break;
                    }

                case Position.eSquareStatus.PlayerOneChecker:
                    {
                        typeOfMove = GetTypeOfMoveByPlayer(i_Source, i_Target, Board.eUpDownDirection.Up, ref o_IsJump);
                        break;
                    }

                case Position.eSquareStatus.PlayerTwoChecker:
                    {
                        typeOfMove = GetTypeOfMoveByPlayer(i_Source, i_Target, Board.eUpDownDirection.Down, ref o_IsJump);
                        break;
                    }

                case Position.eSquareStatus.PlayerOneKing:
                    {
                        typeOfMove = GetTypeOfMoveByPlayerForKing(i_Source, i_Target, ref o_IsJump);
                        break;
                    }

                case Position.eSquareStatus.PlayerTwoKing:
                    {
                        typeOfMove = GetTypeOfMoveByPlayerForKing(i_Source, i_Target, ref o_IsJump);
                        break;
                    }
            }

            return typeOfMove;
        }

        internal Position.eTypeOfMove GetTypeOfMoveByPlayer(Position i_Source, Position i_Target, Board.eUpDownDirection i_UpOrDown, ref bool o_IsJump)
        {
            Position.eTypeOfMove typeOfMove = Position.eTypeOfMove.IllegalMove;
            Position.eSquareStatus sourceStatus = BoardOfGame.GameBoard[i_Source.Row, i_Source.Column];
            Position[] sourceAndDestPositions = new Position[] { i_Source, i_Target };
            bool isTargetTaken = BoardOfGame.GameBoard[i_Target.Row, i_Target.Column] != Position.eSquareStatus.EmptySquere;

            // check for simple move
            bool isValidRow = i_Target.Row == i_Source.Row + (int)i_UpOrDown;
            bool isValidColumn = (i_Target.Column == i_Source.Column + 1) || (i_Target.Column == i_Source.Column - 1);
            if (isValidRow && isValidColumn)
            {
                o_IsJump = false;
                typeOfMove = Position.eTypeOfMove.SimpleMove;
            }

            // check for jump move
            isValidRow = i_Target.Row == i_Source.Row + ((int)i_UpOrDown * 2);
            isValidColumn = (i_Target.Column == i_Source.Column + 2) || (i_Target.Column == i_Source.Column - 2);

            if (isValidRow && isValidColumn && BoardOfGame.CheckIfMiddlePositionIsOppose(sourceAndDestPositions) && !isTargetTaken)
            {
                typeOfMove = Position.eTypeOfMove.EatingMove;
                o_IsJump = true;
            }

            return typeOfMove;
        }

        internal Position.eTypeOfMove GetTypeOfMoveByPlayerForKing(Position i_Source, Position i_Target, ref bool o_IsJump)
        {
            Position.eTypeOfMove typeOfMove = Position.eTypeOfMove.IllegalMove;

            if (i_Source.Row < i_Target.Row)
            {
                Position.eTypeOfMove downTypeOfMove = GetTypeOfMoveByPlayer(i_Source, i_Target, Board.eUpDownDirection.Down, ref o_IsJump);
            }
            else
            {
                Position.eTypeOfMove upTypeOfMove = GetTypeOfMoveByPlayer(i_Source, i_Target, Board.eUpDownDirection.Up, ref o_IsJump);

            }

            return typeOfMove;
        }

        internal bool CheckIfWin()
        {
            return ((FirstPlayer.NumberOfActiveCheckes == 0) || (SecondPlayer.NumberOfActiveCheckes == 0));
        }

        internal void AppendSquereStatusToStringBuilder(int i_RowIndex, int i_columnIndex, ref StringBuilder io_BoardGameStr)
        {
            if (BoardOfGame.IsPositionExist(new Position(i_RowIndex, i_columnIndex)))
            {
                io_BoardGameStr.Append(Convert.ToChar(BoardOfGame.GameBoard[i_RowIndex, i_columnIndex]));
            }
        }

        internal bool CheckIfMoveIsEatWithPrevChecker(Position i_prevChecker, Position[] i_sourceAndDestPositions)
        {
            bool isSameCheckerEat = false;
            bool boolVarToSend = false;
            Position newSourceChecker = i_sourceAndDestPositions[0];
            Position.eSquareStatus checkerStatus = BoardOfGame.GameBoard[i_prevChecker.Row, i_prevChecker.Column];
            Board.eUpDownDirection upOrDown = Board.eUpDownDirection.Down;

            if (checkerStatus == Position.eSquareStatus.PlayerOneChecker || checkerStatus == Position.eSquareStatus.PlayerOneKing)
            {
                upOrDown = Board.eUpDownDirection.Up;
            }

            if (i_prevChecker.Row == newSourceChecker.Row && i_prevChecker.Column == newSourceChecker.Column)
            {
                if (GetTypeOfMoveByPlayer(i_sourceAndDestPositions[0], i_sourceAndDestPositions[1], upOrDown, ref boolVarToSend) == Position.eTypeOfMove.EatingMove)
                {
                    isSameCheckerEat = true;
                }
            }

            return isSameCheckerEat;
        }

        // this function gets valid move and change the game board with this move. and update mustEatAgain bool
        internal void MakePlayerMove(Position[] sourceAndDestPositions, ePlayerTurn i_PlayerTurn, ref bool o_IsMustEatWithSameChecker, bool i_IsJump)
        {
            this.BoardOfGame.UpdateBoardFromOneMove(sourceAndDestPositions, i_IsJump);
            if (i_IsJump)
            {
                if (i_PlayerTurn == ePlayerTurn.PlayerOneTurn)
                {
                    SecondPlayer.NumberOfActiveCheckes--;
                }
                else
                {
                    FirstPlayer.NumberOfActiveCheckes--;
                }
            }
        }

        // ======================Computer Turn Methods===============================

        // this function generate random computer move update the board and return it
        public Position[] GenerateAndUpdateComputerMove(ref bool io_IsJump, ref bool o_IsMustEatWithSameChecker, Position i_PrevChecker)
        {
            List<Position[]> randomCheckerToMoveList = GetListOfAvailableMovesForComputer(this.r_SecondPlayer, ref io_IsJump, ref o_IsMustEatWithSameChecker, i_PrevChecker); // function return list of arrays of two positions
            Random rnd = new Random();
            int random = rnd.Next(randomCheckerToMoveList.Count);
            Position[] sourceAndDestPositions = randomCheckerToMoveList[random];
            BoardOfGame.UpdateBoardFromOneMove(sourceAndDestPositions, io_IsJump); // update the game board
            if (io_IsJump)
            {
                FirstPlayer.NumberOfActiveCheckes--;
            }

            o_IsMustEatWithSameChecker = CheckIfSameCheckerMustEatAgain(FirstPlayer, sourceAndDestPositions[1], ref o_IsMustEatWithSameChecker);
            return sourceAndDestPositions;
        }

        // This Function Get list of available moves for computer
        public List<Position[]> GetListOfAvailableMovesForComputer(Player i_PlayerToCheck, ref bool o_IsJump, ref bool o_IsMustEatWithSameChecker, Position i_PrevChecker)
        {
            List<Position[]> availableMoveList = new List<Position[]>(); // list of sources and destinations positions
            if (o_IsMustEatWithSameChecker)
            {
                AppendEatMoveToList(i_PrevChecker, ref availableMoveList);
            }
            else if (CheckIfPlayerCanEat(i_PlayerToCheck))
            {
                availableMoveList = GetListOfAvailableEatMoves(SecondPlayer);
                o_IsJump = true;
            }
            else
            {
                o_IsJump = false;
                foreach (Position position in m_board.PlayerTwoCheckerList)
                {
                    AppendAvailableMovesForCheckerOfComputer(ref availableMoveList, position);
                }
            }

            return availableMoveList;
        }

        public void AppendAvailableMovesForCheckerOfComputer(ref List<Position[]> o_availableMoveList, Position i_Position)
        {
            AppendAvailableMovesByDirection(i_Position, Board.eUpDownDirection.Down, ref o_availableMoveList);

            if (i_Position.SquareStatus == Position.eSquareStatus.PlayerTwoKing)
            {
                AppendAvailableMovesByDirection(i_Position, Board.eUpDownDirection.Up, ref o_availableMoveList);
            }
        }

        public void AppendAvailableMovesByDirection(Position i_Position, Board.eUpDownDirection i_UpOrDown, ref List<Position[]> o_availableMoveList)
        {
            bool isPossibleMoveToLeft = IsCheckerCanMove(i_Position, ePlayerTurn.PlayerTwoTurn, Board.eLeftRightDirection.Left, i_UpOrDown);
            bool isPossibleMoveToRight = IsCheckerCanMove(i_Position, ePlayerTurn.PlayerTwoTurn, Board.eLeftRightDirection.Right, i_UpOrDown);

            if (isPossibleMoveToLeft)
            {
                Position[] sourceAndDestinationPosition = new Position[2];
                sourceAndDestinationPosition[0] = i_Position;
                sourceAndDestinationPosition[1] = new Position(i_Position.Row + (int)i_UpOrDown, i_Position.Column - 1);
                o_availableMoveList.Add(sourceAndDestinationPosition);
            }

            if (isPossibleMoveToRight)
            {
                Position[] sourceAndDestinationPosition = new Position[2];
                sourceAndDestinationPosition[0] = i_Position;
                sourceAndDestinationPosition[1] = new Position(i_Position.Row + (int)i_UpOrDown, i_Position.Column + 1);
                o_availableMoveList.Add(sourceAndDestinationPosition);
            }
        }

        // this function gets available eat move for computer
        public List<Position[]> GetListOfAvailableEatMoves(Player i_PlayerToCheck)
        {
            List<Position[]> listOfEatMoves = new List<Position[]>();

            foreach (Position position in BoardOfGame.PlayerTwoCheckerList)
            {
                if (CheckIfCheckerCanEat(position))
                {
                    AppendEatMoveToList(position, ref listOfEatMoves);
                }
            }

            return listOfEatMoves;
        }

        // this function append to list available move eat by source position
        internal void AppendEatMoveToList(Position i_Position, ref List<Position[]> o_ListOfEatMoves)
        {
            bool boolVarToSend = false;
            Position[] sourceAndDest = new Position[2];
            for (int i = -2; i < 4; i += 4)
            {
                Position firstDestPosition = new Position(i_Position.Row + i, i_Position.Column + 2);
                if (GetTypeOfMove(i_Position, firstDestPosition, ref boolVarToSend) == Position.eTypeOfMove.EatingMove)
                {
                    o_ListOfEatMoves.Add(sourceAndDest);
                }

                Position secondDestPosition = new Position(i_Position.Row - i, i_Position.Column - 2);
                if (GetTypeOfMove(i_Position, secondDestPosition, ref boolVarToSend) == Position.eTypeOfMove.EatingMove)
                {
                    o_ListOfEatMoves.Add(sourceAndDest);
                }
            }
        }
    }
}
