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
    /// Interaction logic for UserRanking.xaml
    /// </summary>
    public partial class UserRanking : Window
    {
        private User User { get; set; }

        public UserRanking(User user)
        {
            User = user;

            InitializeComponent();

            userNameTextBox.Text = String.Format("User: {0}   ", User.UserName);
            userScoreTextBox.Text = String.Format("Score: {0}", User.Score);

            usersListBox.ItemsSource = DatabaseManager.GetTopTenUsers();
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
