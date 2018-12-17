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

        private Thread getBetHistory;
        private int hoursToAdd;

        public Login()
        {
            TeamsApi.UpdateTeamIcons = false;
            TeamsApi.GetTeams();

            GamesApi.GetGames();
            GamesApi.SetFullTeamNames(GamesApi.Games);

            Thread getLastTenHTeamsGames = new Thread(GetAllLastHTeamTenGames);
            getLastTenHTeamsGames.Start();

            Thread getLastTenVTeamsGames = new Thread(GetAllLastVTeamTenGames);
            getLastTenVTeamsGames.Start();

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
            User user = DatabaseManager.VerifyUser(userNameTextBox.Text, passwordTextBox.Password);

            if (user!=null)
            {
                getBetHistory.Start(user);
                user.BetMatchIds = DatabaseManager.GetUnscoredMatches(user.Id);

                foreach(Bet bet in user.BetMatchIds)
                {
                    bet.ActualScore = Convert.ToString(GamesApi.GetMatchScore(Int32.Parse(bet.MatchId), bet.MatchDate).Winner);
                    if (bet.ActualScore == bet.Prediction)
                    {
                        user.Score += 1;
                    }
                }

                DatabaseManager.UpdateUserScore(user);

                foreach(Game game in GamesApi.Games)
                {
                    game.Dt = DateTime.Parse(game.Dt).AddHours(hoursToAdd).ToString();
                }
                main = new TodayMatches(user);
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

        void GetAllLastHTeamTenGames()
        {
            foreach (Game game in GamesApi.Games)
            {
                game.HTeamLastTenMatches = GamesApi.SetLastTenResults(game.HTeam.TeamName, DateTime.Today);
            }
        }

        void GetAllLastVTeamTenGames()
        {
            foreach (Game game in GamesApi.Games)
            {
                game.VTeamLastTenMatches = GamesApi.SetLastTenResults(game.VTeam.TeamName, DateTime.Today);
            }
        }

        void GetBetHistory(object user)
        {
            List<Bet> betHistory = DatabaseManager.GetLastTwentyBets(((User)user).Id);
            ((User)user).BetHistory = betHistory;
        }
    }
}
