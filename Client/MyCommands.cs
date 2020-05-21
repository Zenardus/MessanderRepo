using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Client
{
    public static class MyCommands
    {
        public static RoutedUICommand ButtonEnabled { get; }
        

        static MyCommands()
        {
            ButtonEnabled = new RoutedUICommand("ButtonEnabled", "ButtonEnabled", typeof(MyCommands));
        }
    }
}
