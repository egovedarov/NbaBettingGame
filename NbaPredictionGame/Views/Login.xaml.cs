using NbaPredictionGame.Backend;
using NbaPredictionGame.Backend.APIs;
using NbaPredictionGame.Backend.Database;
using NbaPredictionGame.Backend.GameObjects;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using System.Windows.Shapes;

namespace NbaPredictionGame.Views
{
    /// <summary>
    /// Interaction logic for Login.xaml
    /// </summary>
    public partial class Login : Window
    {
        public static TodayMatches main = null;
        public static DateTime date;

        private Thread getBetHistory;
        private int hoursToAdd;
        private User User { get; set; }

        public Login()
        {
            if (DateTime.Now.Hour < 6)
            {
                date = DateTime.Now.AddHours(-6);
            }
            else
            {
                date = DateTime.Now;
            }

            TeamsApi.UpdateTeamIcons = false;
            TeamsApi.GetTeams();

            GamesApi.GetGames();
            GamesApi.SetFullTeamNames(GamesApi.Games);

            Thread getLastTenHTeamsGames = new Thread(SetAllLastHTeamTenGames);
            getLastTenHTeamsGames.Start(true);

            Thread getLastTenVTeamsGames = new Thread(SetAllLastHTeamTenGames);
            getLastTenVTeamsGames.Start(false);

            Thread.Sleep(3000);

            getBetHistory = new Thread(GetBetHistory);

            InitializeComponent();
            userNameTextBox.Focus();
        }

        private void registerButton_Click(object sender, RoutedEventArgs e)
        {
            Register reg = new Register();
            reg.Show();
            this.Close();
        }


        private void LoginCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void LoginCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            User = DatabaseManager.VerifyUser(userNameTextBox.Text, passwordTextBox.Password);

            if (User != null)
            {
                User.BetMatchIds = DatabaseManager.GetUnscoredMatches(User.Id);

                foreach (Bet bet in User.BetMatchIds)
                {
                    bet.ActualScore = Convert.ToString(GamesApi.GetMatchScore(Int32.Parse(bet.MatchId), bet.MatchDate).Winner);
                    if (bet.ActualScore == bet.Prediction)
                    {
                        User.Score += 1;
                    }
                }
                DatabaseManager.UpdateUserScore(User);

                getBetHistory.Start(User.Id);

                foreach (Game game in GamesApi.Games)
                {
                    game.Dt = DateTime.Parse(game.Dt).AddHours(hoursToAdd).ToString();
                }
                main = new TodayMatches(User);
                main.Show();
                this.Close();
            }
            else
            {
                MessageBox.Show("Wrong login credentials.", "Unsuccessful login", MessageBoxButton.OK, MessageBoxImage.Error);
                userNameTextBox.Text = "";
                passwordTextBox.Password = "";
                userNameTextBox.Focus();
            }
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int selectedIndex = ((ComboBox)sender).SelectedIndex;
            switch (selectedIndex)
            {
                case 1:
                    hoursToAdd = 6;
                    break;
                case 2:
                    hoursToAdd = 7;
                    break;
                default:
                    hoursToAdd = 0;
                    break;
            }
        }

        void SetAllLastHTeamTenGames(object isLastTenHomeGames)
        {
            GamesApi.SetLastTenResults(GamesApi.Games, (bool)isLastTenHomeGames, date.AddDays(-1));
        }

        void GetBetHistory(object id)
        {
            List<Bet> betHistory = DatabaseManager.GetLastTwentyBets((int)id);
            User.BetHistory = betHistory;
        }
    }
}
