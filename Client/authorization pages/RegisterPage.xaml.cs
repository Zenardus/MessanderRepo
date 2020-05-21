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
using ChatInstruction;

namespace Client
{
    /// <summary>
    /// Логика взаимодействия для RegisterPage.xaml
    /// </summary>
    public partial class RegisterPage : Page
    {
        public RegistrationData User { get; private set; }


        public RegisterPage()
        {
            InitializeComponent();
        }

        public bool SetUserData()
        {
            if (textBox_login.Text.Length < 5)
            {
                MessageBox.Show("Логін повинен містити як мінімум три символів");
                return false;
            }
            if (textBox_password.Text.Length < 5)
            {
                MessageBox.Show("Пароль повинен містити як мінімум 5 символів");
                return false;
            }
            if (textBox_name.Text.Length < 1)
            {
                MessageBox.Show("Заповніть поле [name]");
                return false;
            }
            if (textBox_surname.Text.Length < 1)
            {
                MessageBox.Show("Заповніть поле [surname]");
                return false;
            }
            User = new RegistrationData(textBox_login.Text, textBox_password.Text,
                textBox_name.Text, textBox_surname.Text);
            return true;
        }
    }
}
