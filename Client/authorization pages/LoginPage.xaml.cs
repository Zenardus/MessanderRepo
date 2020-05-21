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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Client
{
    /// <summary>
    /// Логика взаимодействия для LoginPage.xaml
    /// </summary>
    public partial class LoginPage : Page
    {
        public string Login { get; private set; }
        public string Password { get; private set; }

        public LoginPage()
        {
            InitializeComponent();
        }
       public bool SetPasswordLogin()
        {
            if (textBox_password.Text.Length >= 1 && textBox_login.Text.Length >= 1)
            {
                Password = textBox_password.Text;
                Login = textBox_login.Text;
                return true;
                
            } else
            {
                return false;
            }
        }

    }
}
