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

        struct ThreadObject
        {
            public ThreadObject(int _index, bool _isHomeGames)
            {
                index = _index;
                isHomeGames = _isHomeGames;
            }
            public int index;
            public bool isHomeGames;
        };

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

            for (int index = 0; index < GamesApi.Games.Count; index++)
            {
                ThreadPool.QueueUserWorkItem(SetAllLastHTeamTenGames, new ThreadObject(index, true));
            }

            for (int index = 0; index < GamesApi.Games.Count; index++)
            {
                ThreadPool.QueueUserWorkItem(SetAllLastHTeamTenGames, new ThreadObject(index, false));
            }

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
            string errorMessage = "";
            string errorTitle = "";

            if (User != null && User.Id != 0)
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
            else if (User.Id == 0)
            {
                errorMessage = "There was a problem with the connection. Please try again later.";
                errorTitle = "Connection error";
            }
            {
                errorMessage = "Wrong login credentials.";
                errorTitle = "Unsuccessful login";
            }
            MessageBox.Show(errorMessage, errorTitle, MessageBoxButton.OK, MessageBoxImage.Error);
            userNameTextBox.Text = "";
            passwordTextBox.Password = "";
            userNameTextBox.Focus();
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

        void SetAllLastHTeamTenGames(object _threadObject)
        {
            ThreadObject threadObject = (ThreadObject)_threadObject;
            GamesApi.SetLastTenResults(GamesApi.Games[threadObject.index], date.AddDays(-1), threadObject.isHomeGames);
        }

        void GetBetHistory(object id)
        {
            List<Bet> betHistory = DatabaseManager.GetLastTwentyBets((int)id);
            User.BetHistory = betHistory;
        }
    }
}
