using NbaPredictionGame.Backend;
using NbaPredictionGame.Backend.APIs;
using NbaPredictionGame.Backend.GameObjects;
using System;
using System.Collections.Generic;
using System.Globalization;
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
    /// Interaction logic for BetsHistoryWindow.xaml
    /// </summary>
    public partial class BetsHistoryWindow : Window
    {
        public User User { get; set; }

        public BetsHistoryWindow(User user)
        {
            User = user;

            InitializeComponent();

            userNameTextBox.Text = String.Format("User: {0}   ", user.UserName);
            userScoreTextBox.Text = String.Format("Score: {0}", user.Score);

            if (User.BetHistory.Any())
            {
                foreach (Bet bet in User.BetHistory)
                {
                    Result result = GamesApi.GetMatchScore(Int32.Parse(bet.MatchId), bet.MatchDate);
                    bet.HTeamName = result.HTeamName;
                    bet.VTeamName = result.VTeamName;
                    bet.Score = result.ResultString;
                    bet.DateToVisialize = DateTime.ParseExact(bet.MatchDate, "yyyyMMdd", CultureInfo.InvariantCulture).ToString("dd/MM/yyyy");

                    if (bet.Prediction == "1")
                    {
                        bet.PredictionTeamName = bet.HTeamName;
                    }
                    else
                    {
                        bet.PredictionTeamName = bet.VTeamName;
                    }

                    if (bet.Prediction == bet.ActualScore)
                    {
                        bet.IsPredictionCorrect = true;
                    }

                    int breakIndex = 0;
                    foreach (Team team in TeamsApi.Teams)
                    {
                        if (team.TeamName == bet.VTeamName)
                        {
                            breakIndex++;
                            bet.VTeamImage = team.Image;
                        }
                        else if (team.TeamName == bet.HTeamName)
                        {
                            breakIndex++;
                            bet.HTeamImage = team.Image;
                        }

                        if (breakIndex == 2)
                        {
                            break;
                        }
                    }
                }
                betsListBox.ItemsSource = User.BetHistory;
            }
            else
            {
                noBetsTextBlock.Visibility = Visibility.Visible;
                betsListBox.Visibility = Visibility.Hidden;
            }
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

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            BetsHistoryWindow betHistoryWindow = new BetsHistoryWindow(User);

            betHistoryWindow.Show();
            this.Close();
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            UserRanking userRanking = new UserRanking(User);
            userRanking.Show();
            this.Close();
        }
    }
}
