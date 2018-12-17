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
    /// Interaction logic for BetsHistoryWindow.xaml
    /// </summary>
    public partial class BetsHistoryWindow : Window
    {
        public User User { get; set; }

        public BetsHistoryWindow(User user)
        {
            User = user;

            InitializeComponent();

            userInfoTextBox.Text = String.Format("User: {0}, Score: {1}", User.UserName, User.Score);

            betsListBox.ItemsSource = User.BetHistory;
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
    }
}
