# AmericanCheckers
C# implemantation for the game 'American Checkers' for Console. 

The program will allow two human players to play against eachother by turns, or
for a human player to play against the computer which randomly chooses a move out of
the current set of legal moves.

The flow:
1. The user will be asked to enter his name (no spaces, max 10 chars).
2. The user will be asked to choose board size (6, 8 or 10)
3. The user will be asked to choose weather to play against a computer upponent 
   or another human upponent. In case of another human uponent, the user will
   be asked to enter the upponents name (no spaces, max 10 chars).ֿ
4. At each stage, the player will be asked to enter his 'move' by this format: 
   COLrow>COLrow (i.e. Af>Be)
6. If the move is 'illegal', the user will be prompted to enter a valid move.
7. After a valid move was entered by the user, the screen will be cleared and the
   board will be re-drawn with the new state.
8. The O's kings are marked with 'Q'. The X's kings are marked with 'Z'.
9. In case of a 'jump' over an opposing coin ("Eating" / "Capturing"), the
   opposing man is eliminated.
10. The user may decide to quit by entering 'Q' instead of a valid move.
11. In case of a win the winner will gain points according the number of coins
    difference between him and the upponent (A king is worth 4 points) and a
    win will be prompted indlucing the current aggregated score.
12. After a win/draw/quit the user will be asked to decide weather to play
    another round or to quit the program. Another round will be with the same
    configuration as the previous one. The players will continue to gain points.
    
