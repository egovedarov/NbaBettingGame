using NbaPredictionGame.Backend;
using NbaPredictionGame.Backend.APIs;
using NbaPredictionGame.Backend.GameObjects;
using NbaPredictionGame.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace NbaPredictionGame
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class TodayMatches : Window
    {

        public static List<Game> Games { get; set; }

        private User _user;
        private User User { get => _user; set => _user = value; }

        static TodayMatches()
        {
            Games = GamesApi.Games;
        }

        internal TodayMatches(User userrr)
        {
            this.User = userrr;

            InitializeComponent();
            userInfoTextBox.Text = String.Format("User: {0}, Score: {1}", User.UserName, User.Score);
            gamesListBox.ItemsSource = Games;
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

        private void gamesListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Game game = (Game)((ListBox)sender).SelectedItem;
            MatchWindow matchWindow = new MatchWindow(this.User, game);
            matchWindow.Show();
            this.Close();
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            BetsHistoryWindow betHistoryWindow = new BetsHistoryWindow(User);

            betHistoryWindow.Show();
            this.Close();
        }
    }
}
