using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using HassanLib;

namespace MiniMaxTreeVisualisation.Games
{
    public class TicTacToe : IZeroSumGame
    {
        private int _checks = 0;
        public enum Marble { EMPTY, CROSS, ZERO };

        static Dictionary<int, (int x, int y)> keyToPos = new Dictionary<int, (int, int)>()
        {
            {1, (0,0)},
            {2, (1,0)},
            {3, (2,0)},
            {4, (0,1)},
            {5, (1,1)},
            {6, (2,1)},
            {7, (0,2)},
            {8, (1,2)},
            {9, (2,2)},
        };
        /**
         * 1 2 3
         * 4 5 6
         * 7 8 9
         * */

        Marble[] board;
        int _count;          //  number of marbles on the board
        private bool _playerStarts;
        private MiniMaxTree MMT;
        private Canvas g_;
        private int _depth;

        public TicTacToe(Canvas canvas = null, bool playerStarts = true, int depth = 9)
        {
            _playerStarts = playerStarts;
            board = new Marble[10];
            _count = 0;
            MMT = new MiniMaxTree(this);
            _depth = depth;
            if (!_playerStarts)
                DoBestMove();

            this.g_ = canvas;

        }

        public void DoMove(Marble marble, int position)
        {

            if (board[position].Equals(Marble.EMPTY))
            {
                board[position] = marble;
                _count++;
            }
        }
        private void undoMove(Marble marble, int position)
        {
            if (board[position].Equals(marble))
            {
                board[position] = Marble.EMPTY;
                _count--;
            }
        }

        public bool IsWinner(Marble m)
        {
            if (board[1] == m && board[2] == m && board[3] == m
            || board[4] == m && board[5] == m && board[6] == m
            || board[7] == m && board[8] == m && board[9] == m
            || board[1] == m && board[4] == m && board[7] == m
            || board[2] == m && board[5] == m && board[8] == m
            || board[3] == m && board[6] == m && board[9] == m
            || board[1] == m && board[5] == m && board[9] == m
            || board[3] == m && board[5] == m && board[7] == m)
                return true;
            return false;
        }

        public IEnumerable<(Action Move, Action UndoMove)> Possible()
        {
            if (!(IsWinner(Marble.CROSS) || IsWinner(Marble.ZERO)))
            {
                if (CrossTurn())
                {
                    for (int i = 1; i < 10; i++)
                    {

                        if (board[i] == Marble.EMPTY)
                        {
                            var j = i;
                            yield return (
                                    () => { DoMove(Marble.CROSS, j); _checks++; }
                            ,
                                    () => { undoMove(Marble.CROSS, j); }
                            );
                        }
                    }
                }
                else
                {
                    for (int i = 1; i < 10; i++)
                    {
                        if (board[i] == Marble.EMPTY)
                        {
                            var j = i;
                            yield return (
                                () => { DoMove(Marble.ZERO, j); _checks++; }
                            ,
                                () => { undoMove(Marble.ZERO, j); }
                            );
                        }
                    }
                }
            }
        }



        private bool CrossTurn()
        {
            return board.Count(m => m == Marble.CROSS) <= _count * 1f / 2;
        }

        public int StaticEvaluation()
        {
            if (IsWinner(Marble.CROSS))
                return 1;
            if (IsWinner(Marble.ZERO))
                return -1;
            return 0;
        }

        public override string ToString()
        {
            return $"{board[1]}|{board[2]}|{board[3]}\n" +
                   $"{board[4]}|{board[5]}|{board[6]}\n" +
                   $"{board[7]}|{board[8]}|{board[9]}";


        }

        public void Draw()
        {
            g_.Children.Clear();
            bool end = IsWinner(Marble.CROSS) || IsWinner(Marble.ZERO);

            var w = g_.ActualWidth;
            var h = g_.ActualHeight;
            var ws = w / 3;
            var hs = h / 3;
            g_.Children.Add(new Line
            {
                X1 = ws,
                Y1 = 0,
                X2 = ws,
                Y2 = h,
                StrokeThickness = 4,
                Stroke = System.Windows.Media.Brushes.Black,
            });
            g_.Children.Add(new Line
            {
                X1 = ws * 2,
                Y1 = 0,
                X2 = ws * 2,
                Y2 = h,
                StrokeThickness = 4,
                Stroke = System.Windows.Media.Brushes.Black,
            });
            g_.Children.Add(new Line
            {
                X1 = 0,
                Y1 = hs,
                X2 = w,
                Y2 = hs,
                StrokeThickness = 4,
                Stroke = System.Windows.Media.Brushes.Black,
            });
            g_.Children.Add(new Line
            {
                X1 = 0,
                Y1 = hs * 2,
                X2 = w,
                Y2 = hs * 2,
                StrokeThickness = 4,
                Stroke = System.Windows.Media.Brushes.Black,
            });

            for (int i = 1; i < 10; i++)
            {
                UIElement child;
                var (x, y) = keyToPos[i];
                if (board[i] == Marble.CROSS)
                {
                    // Create a SolidColorBrush with a red color to fill the 
                    // Ellipse with.
                    SolidColorBrush mySolidColorBrush = new SolidColorBrush
                    {
                        Color = Colors.Blue,
                    };

                    // Create a red Ellipse.
                    child = new Rectangle
                    {
                        Fill = mySolidColorBrush,
                        StrokeThickness = 2,
                        Stroke = Brushes.Black,
                        Width = w / 8,
                        Height = h / 8
                    };
                    g_.Children.Add(child);
                    Canvas.SetLeft(child, (x * ws) + (ws / 2) - w / 16);
                    Canvas.SetTop(child, (y * hs) + (hs / 2) - h / 16);
                }
                else if (board[i] == Marble.ZERO)
                {

                    // Create a SolidColorBrush with a red color to fill the 
                    // Ellipse with.
                    SolidColorBrush mySolidColorBrush = new SolidColorBrush
                    {
                        Color = Colors.Red,
                    };

                    // Create a red Ellipse.
                    child = new Ellipse
                    {
                        Fill = mySolidColorBrush,
                        StrokeThickness = 2,
                        Stroke = Brushes.Black,
                        Width = w / 8,
                        Height = h / 8
                    };
                    g_.Children.Add(child);
                    Canvas.SetLeft(child, (x * ws) + (ws / 2) - w / 16);
                    Canvas.SetTop(child, (y * hs) + (hs / 2) - h / 16);
                }
                else if (!end)
                {
                    child = new Button
                    {
                        Content = "Place Stone",
                        Width = w / 8,
                        Height = h / 8,
                    };
                    var j = i;
                    if (_playerStarts)
                    {
                        ((Button)child).Click += delegate
                        {

                            DoMove(Marble.CROSS, j);
                            var res = MMT.Minimise(6);
                            res.bestAction.Invoke();
                            Draw();

                        };
                    }
                    else
                    {
                        ((Button)child).Click += delegate
                        {

                            DoMove(Marble.ZERO, j);
                            var res = MMT.Maximise(6);
                            res.bestAction.Invoke();
                            Draw();
                        };
                    };
                    g_.Children.Add(child);
                    Canvas.SetLeft(child, (x * ws) + (ws / 2) - w / 16);
                    Canvas.SetTop(child, (y * hs) + (hs / 2) - h / 16);
                }

            }
        }

        public void DoBestMove()
        {
            if (!CrossTurn())
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
    }
}