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
    /// Interaction logic for Register.xaml
    /// </summary>
    public partial class Register : Window
    {
        private int hoursToAdd;

        public Register()
        {
            InitializeComponent();
            userNameTextBox.Focus();
        }

        private void RegisterCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void RegisterCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            User user = DatabaseManager.AddUser(userNameTextBox.Text, passwordTextBox.Password);
            if (user!=null)
            {
                foreach (Game game in GamesApi.Games)
                {
                    game.Dt = DateTime.Parse(game.Dt).AddHours(hoursToAdd).ToString();
                }

                TodayMatches main = new TodayMatches(user);
                main.Show();
                this.Close();
            }
            else
            {
                MessageBox.Show("Could not register you right now. Try again.", "Registration error", MessageBoxButton.OK, MessageBoxImage.Error);
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
    }
}
