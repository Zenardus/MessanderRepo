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
using System.Net.Sockets;
using System.Net;
using System.Threading;
using ChatInstruction;


namespace Client
{
    /// <summary>
    /// Логика взаимодействия для AuthorizationWindow.xaml
    /// </summary>
    public partial class AuthorizationWindow : Window
    {
        //SERVER
        NetworkStream stream;
        //NAVIGATION (login-register)
        LoginPage loginPage = new LoginPage();
        RegisterPage registerPage = new RegisterPage();
        bool isLogin;
        //LOGIN
        public string Login { get; private set; }
        bool authorizationSuccess = false;



        public AuthorizationWindow(NetworkStream s)
        {
            InitializeComponent();

            stream = s;

            isLogin = true;
            frame.NavigationService.Navigate(loginPage);

            Thread receiveThread = new Thread(Receiver);
            receiveThread.IsBackground = true;
            receiveThread.Start();
        }


        //SERVER METHODS
        private void button_authorize_Click(object sender, RoutedEventArgs e)
        {
            if (isLogin) //login authorization button click
            {
                if (loginPage.SetPasswordLogin())
                {
                    Login = loginPage.Login;
                    Instruction instr = new Instruction(Operation.Login, loginPage.Login, null, loginPage.Password);
                    byte[] data = MyObjectConverter.ObjectToByteArray(instr);
                    stream.WriteAsync(data, 0, data.Length);
                }
                else
                    MessageBox.Show("Поля логін і пароль повинні бути заповнені");
            } else    //register authorization button click
            {
                if (registerPage.SetUserData())
                {
                    Login = registerPage.User.Login;
                    Instruction instr = new Instruction(Operation.Register, null, null, registerPage.User);
                    byte[] data = MyObjectConverter.ObjectToByteArray(instr);
                    stream.WriteAsync(data, 0, data.Length);
                }
            }
            
        }
        private void Receiver()
        {
            //прийом даних з серверу
            try
            {
                while (true)
                {
                    byte[] data = new byte[2048];
                    stream.Read(data, 0, data.Length);
                    Instruction instr = MyObjectConverter.ByteArrayToObject(data) as Instruction;
                    InstructionHandler(instr);

                }
            } catch (Exception)
            {
                if (authorizationSuccess == false)
                {
                    MessageBox.Show("Сервер перестав відповідати");
                    this.Dispatcher.Invoke(new Action(() =>
                    {
                        this.DialogResult = false;
                    }));
                }
                
            }
        }
        private void InstructionHandler(Instruction instr)
        {
            //оброюляє інструкції REGISTER i LOGIN з серверу
            switch (instr.Operation)
            {
                //LOGIN
                case Operation.Login:
                    if ((bool)instr.Data == true)
                    {
                        authorizationSuccess = true;
                        this.Dispatcher.Invoke(new Action(() =>
                        {
                            this.DialogResult = true;
                        }));
                    }
                    else
                        MessageBox.Show(instr.From);
                    break;
                //REGISTER
                case Operation.Register:
                    if ((bool)instr.Data == true)
                    {
                        authorizationSuccess = true;
                        this.Dispatcher.Invoke(new Action(() =>
                        {
                            this.DialogResult = true;
                        }));
                    }
                    else
                        MessageBox.Show(instr.From);
                    break;
            }
        }

        //NAVIGATE BETWEEN LOGIN AND REGISTER
        private void label_link_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                isLogin = !isLogin;
                if (isLogin)
                {
                    frame.NavigationService.Navigate(loginPage);
                    label_link.Content = "register";
                    button_authorize.Content = "Login";
                } else
                {
                    frame.NavigationService.Navigate(registerPage);
                    label_link.Content = "login";
                    button_authorize.Content = "Register";
                }
            }
        }
    }
}
