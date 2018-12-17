using NbaPredictionGame.Backend;
using NbaPredictionGame.Backend.Database;
using NbaPredictionGame.Backend.GameObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace NbaPredictionGame.Views
{
    /// <summary>
    /// Interaction logic for MatchWindow.xaml
    /// </summary>
    public partial class MatchWindow : Window
    {
        public Game Game { get; set; }
        public User User { get; set; }

        public List<Result> HTeamMatches { get; set; }

        public List<Result> VTeamMatches { get; set; }

        public MatchWindow(User user, Game game)
        {
            // Initialize properties
            Game = game;
            User = user;

            HTeamMatches = Game.HTeamLastTenMatches;
            VTeamMatches = Game.VTeamLastTenMatches;

            InitializeComponent();

            ImageSource imageSource = new BitmapImage(new Uri(Game.HTeam.Image));
            hTeamImage.Source = imageSource;
            hTeamName.Text = Game.HTeam.TeamName;

            gameTime.Text = Game.Dt;

            vTeamName.Text = Game.VTeam.TeamName;
            imageSource = new BitmapImage(new Uri(Game.VTeam.Image));
            vTeamImage.Source = imageSource;

            hTeamWinButton.Content = Game.HTeam.TeamName + " Win";
            vTeamWinButton.Content = Game.VTeam.TeamName + " Win";

            hTeamMatchesListBox.ItemsSource = HTeamMatches;
            vTeamMatchesListBox.ItemsSource = VTeamMatches;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Teams teamsWindow = new Teams();
            teamsWindow.Show();
            this.Close();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            TodayMatches main = new TodayMatches(this.User);
            main.Show();
            this.Close();
        }

        private void hTeamWinButton_Click(object sender, RoutedEventArgs e)
        {
            TodayMatches main;
            if (DatabaseManager.DoesGameBetExist(User, Game))
            {
                main = new TodayMatches(User);
                MessageBox.Show("You have already made your bet for this match.");
                main.Show();
                this.Close();
                return;
            }

            string winner = "1";

            bool isDraw = drawCheckBox.IsChecked ?? false;

            if (isDraw)
            {
                winner += "x";
            }

            DatabaseManager.PlaceBet(User, Game, winner);

            main = new TodayMatches(User);
            main.Show();
            this.Close();
        }

        private void vTeamWinButton_Click(object sender, RoutedEventArgs e)
        {
            TodayMatches main;
            if (DatabaseManager.DoesGameBetExist(User, Game))
            {
                main = new TodayMatches(User);
                MessageBox.Show("You have already made your bet for this match.");
                main.Show();
                this.Close();
                return;
            }

            string winner = "2";

            bool isDraw = drawCheckBox.IsChecked ?? false;

            if (isDraw)
            {
                winner += "x";
            }

            DatabaseManager.PlaceBet(User, Game, winner);

            main = new TodayMatches(User);
            main.Show();
            this.Close();
        }
    }
}
