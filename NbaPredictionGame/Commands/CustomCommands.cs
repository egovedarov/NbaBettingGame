using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace NbaPredictionGame.Commands
{
    public static class CustomCommands
    {
        public static readonly RoutedUICommand Login = new RoutedUICommand
            (
                "Login",
                "Login",
                typeof(CustomCommands),
                new InputGestureCollection()
                {
                    new KeyGesture(Key.Enter)
                }
            );

        //Define more commands here, just like the one above
    }
}
