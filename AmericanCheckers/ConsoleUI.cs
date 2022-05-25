using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmericanCheackers
{
    internal class ConsoleUI
    {
        private Game.eBoardSize m_BoardSize;
        private Game m_Game;

        public void InitializeGame()
        {
            Game.eGameType gameType = GetGameType();
            Game.eBoardSize boardSize = GetBoardSize();
            this.m_BoardSize = boardSize;
            string firstPlayerName = string.Empty;
            string secondPlayerName = string.Empty;
            bool continueGame = true;

            GetPlayersNameByGameType(gameType, ref firstPlayerName, ref secondPlayerName);

            this.m_Game = new Game(boardSize, gameType, firstPlayerName, secondPlayerName);

            while (continueGame)
            {
                startGame();

                Console.WriteLine(string.Format(
@"Would you like to play again?
Press Y for: Yes
Press N for: No"));
                continueGame = CheckContinueGameInput(Console.ReadLine());

                if (continueGame)
                {
                    m_Game.ResetGame();
                }

                Ex02.ConsoleUtils.Screen.Clear();
            }

            Console.WriteLine(Environment.NewLine + "GoodBye mate!");
        }

        private void startGame()
        {
            bool isGameOver = false;
            bool isJump = false;
            Game.ePlayerTurn playerTurn = Game.ePlayerTurn.PlayerOneTurn;
            bool isFirstTurnOfGame = true;
            bool isMustEatWithSameChecker = false;
            bool isPlayerQuite = false;

            string previousTurnMove = string.Empty;
            DrawBoard();
            Console.WriteLine(string.Format("{0}'s Turn (O):\n", m_Game.FirstPlayer.Name));

            while (!isGameOver)
            {
                if (!isFirstTurnOfGame)
                {
                    UpdateTurnIfNeeded(ref playerTurn, isJump, m_Game.GameType);
                    DrawBoard();
                    Console.WriteLine(
                    playerTurn == Game.ePlayerTurn.PlayerOneTurn
                    ? string.Format(
@"{0}'s move was (X): {1}
{2}'s Turn (O): ",
m_Game.GetNameOfCurrentPlayer(Game.ePlayerTurn.PlayerTwoTurn),
previousTurnMove,
m_Game.GetNameOfCurrentPlayer(Game.ePlayerTurn.PlayerOneTurn))
                    : string.Format(
@"{0}'s move was (O): {1}
{2}'s Turn (X): ",
m_Game.GetNameOfCurrentPlayer(Game.ePlayerTurn.PlayerOneTurn),
previousTurnMove,
m_Game.GetNameOfCurrentPlayer(Game.ePlayerTurn.PlayerTwoTurn)));
                }

                isFirstTurnOfGame = false;

                MakeMoveByTypeOfPlayer(playerTurn, ref isJump, ref isMustEatWithSameChecker, ref previousTurnMove, ref isPlayerQuite);
                if (isPlayerQuite)
                {
                    break;
                }

                isGameOver = CheckIfGameOver(playerTurn);

                while (isMustEatWithSameChecker && !isGameOver)
                {
                    DrawBoard();
                    Console.WriteLine(string.Format(
@"{0}'s move was {1}
{0}'s Turn ({2}): ", m_Game.GetNameOfCurrentPlayer(playerTurn), previousTurnMove, GetSymbolByPlayerTurn(playerTurn)));
                    MakeMoveByTypeOfPlayer(playerTurn, ref isJump, ref isMustEatWithSameChecker, ref previousTurnMove, ref isPlayerQuite);
                    isGameOver = CheckIfGameOver(playerTurn);
                }
            }
        }

        private char GetSymbolByPlayerTurn(Game.ePlayerTurn i_PlayerTurn)
        {
            char symbol;
            if (i_PlayerTurn == Game.ePlayerTurn.PlayerTwoTurn)
            {
                symbol = 'X';
            }
            else
            {
                symbol = 'O';
            }

            return symbol;
        }

        internal void MakeMoveByTypeOfPlayer(Game.ePlayerTurn i_PlayerTurn, ref bool io_IsJump, ref bool o_IsMustEatWithSameChecker, ref string o_PreviousTurnMove, ref bool o_IsPlayerQuite)
        {
            // if computer turn
            if (i_PlayerTurn == Game.ePlayerTurn.PlayerTwoTurn && m_Game.GameType == Game.eGameType.SinglePlayer)
            {
                o_PreviousTurnMove = MakeComputerMove(ref io_IsJump, ref o_IsMustEatWithSameChecker, o_PreviousTurnMove);
            }
            else
            {
                Position[] sourceAndDestPositions = new Position[2];
                string sourceAndDestStr = GetNextMoveInputFromPlayer(i_PlayerTurn, ref io_IsJump, ref o_IsMustEatWithSameChecker, o_PreviousTurnMove);
                if (sourceAndDestStr.Equals("Q"))
                {
                    GameOverMessage(Game.eGameOverStatus.Surrended, i_PlayerTurn);
                    o_IsPlayerQuite = true;
                }
                else
                {
                    o_PreviousTurnMove = sourceAndDestStr;
                    sourceAndDestPositions = ConvertStrToSourceAndDestPositions(o_PreviousTurnMove);
                    m_Game.MakePlayerMove(sourceAndDestPositions, i_PlayerTurn, ref o_IsMustEatWithSameChecker, io_IsJump);
                }
            }
        }

        internal void GameOverMessage(Game.eGameOverStatus i_GameOverStatus, Game.ePlayerTurn i_playerTurn)
        {
            DrawBoard();
            m_Game.GameOverUpdate(i_GameOverStatus, i_playerTurn);

            switch (i_GameOverStatus)
            {
                case Game.eGameOverStatus.Surrended:
                    {
                        Console.WriteLine(string.Format(

@"{0}'s Surrender!
New score status is:
{1}: {2}
{3}: {4}", m_Game.GetNameOfCurrentPlayer(i_playerTurn), m_Game.FirstPlayer.Name, m_Game.FirstPlayer.Score, m_Game.SecondPlayer.Name, m_Game.SecondPlayer.Score));
                        break;
                    }

                case Game.eGameOverStatus.Win:
                    {
                        Console.WriteLine(string.Format(

@"We have a WINNER! 
Congratulations {0}! You Won the game!
New score status is:
{1}: {2}
{3}: {4}", m_Game.GetNameOfCurrentPlayer(i_playerTurn), m_Game.FirstPlayer.Name, m_Game.FirstPlayer.Score, m_Game.SecondPlayer.Name, m_Game.SecondPlayer.Score));
                        break;
                    }

                case Game.eGameOverStatus.BothStuck:
                    {
                        Console.WriteLine(string.Format(

@"It's a TIE ! 
I'm sorry but it seems like you got both stuck... !
New score status is:
{0}: {1}
{2}: {3}", m_Game.FirstPlayer.Name, m_Game.FirstPlayer.Score, m_Game.SecondPlayer.Name, m_Game.SecondPlayer.Score));
                        break;
                    }
            }
        }

        internal bool CheckIfGameOver(Game.ePlayerTurn i_PlayerTurn)
        {
            bool isGameOver = false;
            bool isBothPlayersStuck = m_Game.CheckIfBothPlayersStuck(i_PlayerTurn);
            bool isWin = m_Game.CheckIfWin();

            if (isBothPlayersStuck)
            {
                GameOverMessage(Game.eGameOverStatus.BothStuck, i_PlayerTurn);
                isGameOver = true;
            }

            if (isWin)
            {
                GameOverMessage(Game.eGameOverStatus.Win, i_PlayerTurn);
                isGameOver = true;
            }

            return isGameOver;
        }

        internal void UpdateTurnIfNeeded(ref Game.ePlayerTurn io_PlayerTurn, bool i_IsJump, Game.eGameType i_GameType)
        {
            bool playerIsStuck = m_Game.CheckIfPlayerIsStuckByTurn(io_PlayerTurn);

            if (!playerIsStuck)
            {
                Game.ChangePlyaerTurn(ref io_PlayerTurn);
            }
        }

        public string GetNextMoveInputFromPlayer(Game.ePlayerTurn i_PlayerTurn, ref bool o_IsJump, ref bool io_IsMustEatWithSameChecker, string i_PreviousTurnMove)
        {
            string inputMoveStr = Console.ReadLine();
            if (!inputMoveStr.Equals("Q"))
            {
                while (!CheckMoveInputSyntaxValidation(inputMoveStr))
                {
                    Console.WriteLine(Environment.NewLine + "The syntax of your input move is not valid. Please try again");
                    inputMoveStr = Console.ReadLine();
                }

                Position[] sourceAndDestPositions = ConvertStrToSourceAndDestPositions(inputMoveStr);

                while (!m_Game.CheckInputMoveAvailability(sourceAndDestPositions, i_PlayerTurn, ref o_IsJump))
                {
                    Console.WriteLine(Environment.NewLine + "Ooops... seems like your move is not available. Please try again");
                    inputMoveStr = Console.ReadLine();
                    sourceAndDestPositions = ConvertStrToSourceAndDestPositions(inputMoveStr);
                }

                if (!o_IsJump)
                {
                    while (!o_IsJump && m_Game.CheckIfPlayerHaveToEatByTurn(i_PlayerTurn))
                    {
                        Console.WriteLine(Environment.NewLine + "You have eating option and you entered simple move! Please try again");
                        inputMoveStr = Console.ReadLine();
                        sourceAndDestPositions = ConvertStrToSourceAndDestPositions(inputMoveStr);
                        m_Game.CheckInputMoveAvailability(sourceAndDestPositions, i_PlayerTurn, ref o_IsJump);
                    }
                }

                if (io_IsMustEatWithSameChecker)
                {
                    Position[] prevMove = ConvertStrToSourceAndDestPositions(i_PreviousTurnMove);
                    while (m_Game.CheckIfMoveIsEatWithPrevChecker(prevMove[1], sourceAndDestPositions))
                    {
                        Console.WriteLine(Environment.NewLine + "You must eat with same checker!");
                        inputMoveStr = Console.ReadLine();
                        sourceAndDestPositions = ConvertStrToSourceAndDestPositions(inputMoveStr);
                    }
                }
            }

            return inputMoveStr;
        }

        private bool CheckMoveInputSyntaxValidation(string i_inputNextMove)
        {
            bool isMoveValid = false;
            char[] strInput = i_inputNextMove.ToCharArray();

            int lastLetterRow = (int)m_BoardSize + 'a';
            int lastLetterCol = (int)m_BoardSize + 'A';

            if (strInput.Length == 5 && strInput[2] == '>')
            {
                for (int i = 0; i < 2; i += 3)
                {
                    if (strInput[i] >= 'A' && strInput[i] <= lastLetterCol && strInput[i + 1] >= 'a' && strInput[i + 1] <= lastLetterRow)
                    {
                        isMoveValid = true;
                    }
                }
            }

            return isMoveValid;
        }

        public Position[] ConvertStrToSourceAndDestPositions(string i_InputMoveStr)
        {
            string[] stringInputSplittedSquere;

            stringInputSplittedSquere = i_InputMoveStr.Split('>');
            Position sourcePosition = ConvertSquareStrToPosition(stringInputSplittedSquere[0]);
            Position destPosition = ConvertSquareStrToPosition(stringInputSplittedSquere[1]);

            return new Position[] { sourcePosition, destPosition };
        }

        private Position ConvertSquareStrToPosition(string i_ColAndRow)
        {
            int col = i_ColAndRow[0] - 'A';
            int row = i_ColAndRow[1] - 'a';

            return new Position(row, col);
        }

        public string MakeComputerMove(ref bool i_IsJump, ref bool o_IsMustEatWithSameChecker, string i_prevTrun)
        {
            Position prevCkecker = ConvertStrToSourceAndDestPositions(i_prevTrun)[1];
            Position[] sourceAndDestPositions = m_Game.GenerateAndUpdateComputerMove(ref i_IsJump, ref o_IsMustEatWithSameChecker, prevCkecker);
            string computerMovestr = ConvertPositionsArrayToString(sourceAndDestPositions);
            return computerMovestr;
        }

        public string ConvertPositionsArrayToString(Position[] i_sourceAndDestPositions)
        {
            return ConvertPositionToString(i_sourceAndDestPositions[0]) + '>' + ConvertPositionToString(i_sourceAndDestPositions[1]);
        }

        public string ConvertPositionToString(Position i_Position)
        {
            char row = Convert.ToChar(i_Position.Row + 'a');
            char col = Convert.ToChar(i_Position.Column + 'A');
            char[] chars = { col, row };
            string positionStr = new string(chars);

            return positionStr;
        }

        public bool CheckContinueGameInput(string i_ContinueGameInputStr)
        {
            while (!i_ContinueGameInputStr.Equals("N") && !i_ContinueGameInputStr.Equals("Y"))
            {
                Console.WriteLine(Environment.NewLine + "Yout input is not valid.Would you like to play again?\n Y=Yes\nN=No");
                i_ContinueGameInputStr = Console.ReadLine();
            }

            bool isContinue = i_ContinueGameInputStr == "Y";
            return isContinue;
        }

        public void GetPlayersNameByGameType(Game.eGameType i_GameType, ref string o_FirstPlayerName, ref string o_SecondPlayerName)
        {
            if (i_GameType == Game.eGameType.SinglePlayer)
            {
                Console.WriteLine(Environment.NewLine + "Please enter your name: (No spaces and 10 characters max)");
                o_FirstPlayerName = GetPlayerName();
                o_SecondPlayerName = "Computer";
            }
            else
            {
                Console.WriteLine(Environment.NewLine + "Please enter your names: (No spaces and 10 characters max)");
                Console.WriteLine("First player name:");
                o_FirstPlayerName = GetPlayerName();
                Console.WriteLine(Environment.NewLine + "Second player name:");
                o_SecondPlayerName = GetPlayerName();
            }
        }

        public string GetPlayerName()
        {
            string playerName = Console.ReadLine();

            while (!Player.CheckNameValidation(playerName))
            {
                Console.WriteLine(Environment.NewLine + "Your input is NOT valid.\nPlease write your name again:");
                playerName = Console.ReadLine();
            }

            return playerName;
        }

        public static Game.eBoardSize GetBoardSize()
        {
            Console.WriteLine(Environment.NewLine + string.Format(
@"Please enter your desired board size:
For small (6X6) Enter : 6
For medium (8X8) Enter : 8
For large (10X10) Enter : 10"));
            string boardSize = Console.ReadLine();

            while (!boardSize.Equals("6") && !boardSize.Equals("8") && !boardSize.Equals("10"))
            {
                Console.WriteLine(Environment.NewLine + "Your input is NOT valid.\nPlease write your desired board size again:");
                boardSize = Console.ReadLine();
            }

            Game.eBoardSize enumBoardSize = (Game.eBoardSize)System.Enum.Parse(typeof(Game.eBoardSize), boardSize);
            return enumBoardSize;
        }

        public static Game.eGameType GetGameType()
        {
            Console.WriteLine(Environment.NewLine + string.Format(
@"*********************************************************************
******************* Welcome to American Checkers! *******************
*********************************************************************

Please enter your desired type of game:
For two players press: 0
For Singleplayer press: 1"));
            string gameTypeStr = Console.ReadLine();

            while (!gameTypeStr.Equals("0") && !gameTypeStr.Equals("1"))
            {
                Console.WriteLine(Environment.NewLine + string.Format(
@"The game type you entered is invalid.
Please enter your desired type of game again.
For two players press 0
For single player press 1"));
                gameTypeStr = Console.ReadLine();
            }

            Game.eGameType enumGameType = (Game.eGameType)int.Parse(gameTypeStr);
            return enumGameType;
        }

        private void DrawBoard()
        {
            StringBuilder boardGameStr = new StringBuilder();

            buildBoardGame(ref boardGameStr);
            Ex02.ConsoleUtils.Screen.Clear();
            Console.WriteLine(boardGameStr);
        }

        // build board game for print
        private void buildBoardGame(ref StringBuilder io_BoardGameStr)
        {
            addUpperLettersToBoard(ref io_BoardGameStr);
            addBorderToBoard(ref io_BoardGameStr);

            for (int rowIndex = 0; rowIndex < (int)m_BoardSize; rowIndex++)
            {
                addLineToBoard(rowIndex, ref io_BoardGameStr);
                addBorderToBoard(ref io_BoardGameStr);
            }
        }

        // append one line of board to stringBuilder
        private void addLineToBoard(int i_RowIndex, ref StringBuilder io_BoardGameStr)
        {
            io_BoardGameStr.Append((char)('a' + i_RowIndex) + "|");

            for (int columnIndex = 0; columnIndex < (int)m_BoardSize; columnIndex++)
            {
                io_BoardGameStr.Append(" ");
                m_Game.AppendSquereStatusToStringBuilder(i_RowIndex, columnIndex, ref io_BoardGameStr);
                io_BoardGameStr.Append(" |");
            }

            io_BoardGameStr.Append("\n");
        }

        // print the line of index columns
        private void addUpperLettersToBoard(ref StringBuilder io_BoardGameStr)
        {
            io_BoardGameStr.Append("   ");

            for (int i = 0; i < (int)m_BoardSize; i++)
            {
                io_BoardGameStr.Append((char)('A' + i) + "   ");
            }

            io_BoardGameStr.Append("\n");
        }

        // print the border line
        private void addBorderToBoard(ref StringBuilder io_BoardGameStr)
        {
            io_BoardGameStr.Append(" =");

            for (int i = 0; i < (int)m_BoardSize; i++)
            {
                io_BoardGameStr.Append("====");
            }

            io_BoardGameStr.Append("\n");
        }
    }
}
