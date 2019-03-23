using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using HassanLib;
using MiniMaxTreeVisualisation.Games;

namespace MiniMaxTreeVisualisation
{
    public enum Gamestate { TTT, CF, NONE}
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        TicTacToe TTT;
        private DispatcherTimer TTTTimer = new DispatcherTimer();
        ConnectFour CF;
        private DispatcherTimer CFTimer = new DispatcherTimer();
        private int Depth = 6;
        private bool Paused = false;
        
        public MainWindow()
        {
            InitializeComponent();
        }
        private void StartCF()
        {
            CF = new ConnectFour(g, true, Depth);
            CF.Draw();
        }

        private void StartTTT()
        {
            TTT = new TicTacToe(g,  true, Depth);
            TTT.Draw();
        }

        private void MainWindow_OnKeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Escape)
                this.Close();
            if (e.Key == Key.P)
                Paused = !Paused;
        }

        private void TTTBUTTON_OnClick(object sender, RoutedEventArgs e)
        {
            CF = null;
            TTTTimer.Stop();
            CFTimer.Stop();
            StartTTT();
        }

        private void TTTBUTTONAI_OnClick(object sender, RoutedEventArgs e)
        {
            CF = null;
            TTTTimer.Stop();
            CFTimer.Stop();
            StartTTT();
            TTTTimer = new DispatcherTimer();
            TTTTimer.Tick += (_, __) =>
            {
                if(Paused)
                    return;
                try
                {
                    TTT.DoBestMove();
                    TTT.Draw();
                }
                catch
                {
                    //
                }
            };
            TTTTimer.Interval = new TimeSpan(0, 0, 1);
            TTTTimer.Start();
        }

        private void CFBUTTON_OnClick(object sender, RoutedEventArgs e)
        {
            TTT = null;
            TTTTimer.Stop();
            CFTimer.Stop();
            StartCF();
        }

        private void CFBUTTONAI_OnClick(object sender, RoutedEventArgs e)
        {
            TTT = null;
            TTTTimer.Stop();
            CFTimer.Stop();
            StartCF();
            
            CFTimer = new DispatcherTimer();
            CFTimer.Tick += (_, __) =>
            {
                if(Paused)
                    return;
                try
                {
                    CF.DoBestMove();
                    CF.Draw();
                }
                catch
                {
                 //
                }
                
            };
            CFTimer.Interval = new TimeSpan(0, 0, 1);
            CFTimer.Start();
        }

        private void changeDepth(object sender, RoutedEventArgs e)
        {
            if (sender is TextBox senderBox)
            {
                if (int.TryParse(senderBox.Text, out int num))
                {
                    Depth = num;
                }
                else
                {
                    senderBox.Text = Depth.ToString();
                }
            }
        }

    }
}
