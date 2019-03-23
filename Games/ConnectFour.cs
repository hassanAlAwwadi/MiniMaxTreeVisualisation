using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using HassanLib;

namespace MiniMaxTreeVisualisation.Games
{
    public enum Stone { Empty, Yellow, Red };
    public class ConnectFour : IZeroSumGame
    {
        private static int[][] evaluationTable = {
            new int[] {3, 4, 5 , 7 , 5 , 4, 3},
            new int[] {4, 6, 8 , 10, 8 , 6, 4},
            new int[] {5, 8, 11, 13, 11, 8, 5},
            new int[] {5, 8, 11, 13, 11, 8, 5},
            new int[] {4, 6, 8 , 10, 8 , 6, 4},
            new int[] {3, 4, 5 , 7 , 5 , 4, 3}};

        //used to check if a move is possible
        private Dictionary<int, int> columnCount = new Dictionary<int, int>()
        {
            {0, 0},
            {1, 0},
            {2, 0},
            {3, 0},
            {4, 0},
            {5, 0},
            {6, 0},
        };

        //  0   1  2  3  4  5  6 
        //  _____________________
        //0|__|__|__|__|__|__|__|
        //1|__|__|__|__|__|__|__|
        //2|__|__|__|__|__|__|__|
        //3|__|__|__|__|__|__|__|
        //4|__|__|__|__|__|__|__|
        //5|__|__|__|__|__|__|__|

        public Stone[,] board;
        public bool PlayerStarts;
        private MiniMaxTree MMT;
        private Canvas g_;
        private int check = 0;
        private int _depth;
        public ConnectFour(Canvas canvas = null, bool playerStarts = true, int depth = 6)
        {
            board = new Stone[6, 7];
            for (int i = 0; i < 6; i++)
                for (int j = 0; j < 7; j++)
                    board[i, j] = Stone.Empty;

            _depth = depth;
            MMT = new MiniMaxTree(this);

            PlayerStarts = playerStarts;
            if (!PlayerStarts)
                DoBestMove();

            this.g_ = canvas;

        }

        public void DoBestMove()
        {
            if (!RedTurn())
            {
                var b = MMT.Minimise(_depth);
                b.bestAction.Invoke();
            }
            else
            {
                var b = MMT.Maximise(_depth);
                b.bestAction.Invoke();
            }

        }

        private bool RedTurn()
        {
            return (columnCount[0] + columnCount[1] + columnCount[2] + columnCount[3] +
                    columnCount[4] + columnCount[5] + columnCount[6]) % 2 == 0;
        }

        private void DoMove(int c, Stone st)
        {

            if (c > 6)
                return;

            board[5 - columnCount[c], c] = st;
            columnCount[c] ++;


        }


        public IEnumerable<(Action Move, Action UndoMove)> Possible()
        {
            if (IsWinner(Stone.Red).win || IsWinner(Stone.Yellow).win)
                yield break;
            if (RedTurn())
            {
                for (int c = 0; c < 7; c++)
                {
                    var i = c;
                    if (columnCount[i] < 6)
                    {
                        yield return (
                            () =>
                            {
                                DoMove(i, Stone.Red);
                                check++;
                            }
                            ,
                            () => { UndoMove(i); }
                        );
                    }
                }
            }
            else
            {
                for (int c = 0; c < 7; c++)
                {
                    var i = c;
                    if (columnCount[i] < 6)
                    {
                        yield return (
                            () =>
                            {
                                DoMove(i, Stone.Yellow);
                                check++;
                            }
                            ,
                            () => { UndoMove(i); }
                        );
                    }
                }
            }
        }

        private void UndoMove(int c)
        {
            if (c > 6)
                return;
            var r = 6 - columnCount[c];
            board[r, c] = Stone.Empty;
            columnCount[c]--;
        }

        public int StaticEvaluation()
        {
            var redWinner = IsWinner(Stone.Red);
            if (redWinner.win)
                return 1000;

            var yellowWinner = IsWinner(Stone.Yellow);
            if (yellowWinner.win)
                return -1000;

            var eval = redWinner.score - yellowWinner.score;
            return eval;
        }

        private (bool win, int score) IsWinner(Stone st)
        {
            int score = 0;
            //checks horizontal 4 in a row.
            for (int r = 0; r < 6; r++)
            {
                var inARow = 0;
                for (int c = 0; c < 7; c++)
                {
                    if (board[r, c] == st)
                    {
                        inARow++;
                        score += evaluationTable[r][c];
                    }
                    else
                        inARow = 0;

                    if (inARow == 4)
                        return (true, score);


                }
            }

            //checks vertical 4 in a row.
            for (int c = 0; c < 7; c++)
            {
                var inARow = 0;
                for (int r = 0; r < 6; r++)
                {
                    if (board[r, c] == st)
                    {
                        inARow++;
                    }
                    else
                        inARow = 0;

                    if (inARow == 4)
                        return (true, score);


                }
            }

            //checks diagonal 4 in a row.
            //check \\ these
            for (int r = 0; r < 3; r++)
            {
                var inARow = 0;
                var rp = r;
                for (int c = 0; rp < 6 && c < 7; c++)
                {
                    if (board[rp, c] == st)
                    {
                        inARow++;
                    }
                    else
                        inARow = 0;

                    if (inARow == 4)
                        return (true, score);
                    rp++;
                }
            }
            for (int c = 0; c <= 3; c++)
            {
                var inARow = 0;
                var cp = c;
                for (int r = 0; cp < 7 && r < 6; r++)
                {
                    if (board[r, cp] == st)
                    {
                        inARow++;
                    }
                    else
                        inARow = 0;

                    if (inARow == 4)
                        return (true, score);

                    cp++;
                }
            }

            //check // these
            for (int r = 5; r > 3; r--)
            {
                var rp = r;
                var inARow = 0;
                for (int c = 0; rp >= 0 && c < 7; c++)
                {
                    if (board[rp, c] == st)
                    {
                        inARow++;
                    }
                    else
                        inARow = 0;

                    if (inARow == 4)
                        return (true, score);
                    rp--;
                }
            }
            for (int c = 6; c >= 3; c--)
            {
                var inARow = 0;
                var cp = c;
                for (int r = 0; cp > -0 && r < 6; r++)
                {
                    if (board[r, cp] == st)
                    {
                        inARow++;
                    }
                    else
                        inARow = 0;

                    if (inARow == 4)
                        return (true, score);
                    cp--;
                }
            }
            return (false, score);

        }

        public void Draw()
        {
            g_.Children.Clear();

            var w = g_.ActualWidth;
            var h = g_.ActualHeight;
            var ws = w / 7;
            var hs = h / 7;


            //creates the background board
            SolidColorBrush mySolidColorBrush = new SolidColorBrush(Colors.Blue);
            var boardB = new Rectangle
            {
                Fill = mySolidColorBrush,
                StrokeThickness = 2,
                Stroke = Brushes.Black,
                Width = w,
                Height = h - hs
            };
            g_.Children.Add(boardB);
            Canvas.SetTop(boardB, hs);

            //creates the current stones
            for (int r = 0; r < 6; r++)
            {
                for (int c = 0; c < 7; c++)
                {
                    SolidColorBrush color;
                    Ellipse child;

                    if (board[r, c] == Stone.Red)
                    {
                        color = new SolidColorBrush(Colors.Red);
                        child = new Ellipse
                        {
                            Fill = color,
                            StrokeThickness = 2,
                            Stroke = Brushes.Black,
                            Width = ws,
                            Height = hs
                        };
                    }
                    else if (board[r, c] == Stone.Yellow)
                    {
                        color = new SolidColorBrush(Colors.Yellow);
                        child = new Ellipse
                        {
                            Fill = color,
                            StrokeThickness = 2,
                            Stroke = Brushes.Black,
                            Width = ws,
                            Height = hs
                        };

                    }
                    else
                    {
                        color = new SolidColorBrush(Colors.White);
                        child = new Ellipse
                        {
                            Fill = color,
                            StrokeThickness = 2,
                            Stroke = Brushes.Black,
                            Width = ws,
                            Height = hs
                        };
                    }
                    g_.Children.Add(child);
                    Canvas.SetLeft(child, c * ws);
                    Canvas.SetTop(child, r * hs + hs);

                }
            }

            //creates the possible moves
            if (IsWinner(Stone.Red).win || IsWinner(Stone.Yellow).win) return;
            for (int c = 0; c < 7; c++)
            {
                if (columnCount[c] < 6)
                {
                    var child = new Button();
                    child = new Button
                    {
                        Content = "Place Stone",
                        Width = ws * 0.7,
                        Height = hs * 0.7,
                    };
                    int i = c;
                    if (PlayerStarts)
                    {
                        (child).Click += delegate
                        {

                            DoMove(i, Stone.Red);
                            DoBestMove();
                            Draw();

                        };
                    }
                    else
                    {
                        (child).Click += delegate
                        {

                            DoMove(i, Stone.Yellow);
                            DoBestMove();
                            Draw();
                        };
                    };
                    g_.Children.Add(child);
                    Canvas.SetTop(child, hs * 0.2);
                    Canvas.SetLeft(child, c * ws + ws * 0.2);
                }
            }
        }

    }
}
