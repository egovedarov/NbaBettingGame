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
    /// Interaction logic for Teams.xaml
    /// </summary>
    public partial class Teams : Window
    {
        public Teams()
        {
            InitializeComponent();
        }

        private void Matches_Click(object sender, RoutedEventArgs e)
        {
            TodayMatches main = new TodayMatches(new User(0,"","",0));
            main.Show();
            this.Close();
        }

        private void Teams_Click(object sender, RoutedEventArgs e)
        {
            Teams teams = new Teams();
            teams.Show();
            this.Close();
        }
    }
}
