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
using System.Net.Sockets;
using ChatInstruction;

namespace Client
{
    /// <summary>
    /// Логика взаимодействия для RequestsPage.xaml
    /// </summary>
    public partial class RequestsPage : Page
    {
        NetworkStream stream;
        string from;

        public RequestsPage(NetworkStream stream, string from)
        {
            InitializeComponent();

            this.stream = stream;
            this.from = from;
        }

        public void SetList(List<UserData> requests)
        {
            listBox_users.Dispatcher.Invoke(new Action(() =>
            {
                listBox_users.Items.Clear();

                foreach (var item in requests)
                {
                    Label lbl = new Label();
                    lbl.Foreground = Brushes.WhiteSmoke;
                    lbl.HorizontalContentAlignment = HorizontalAlignment.Left;
                    lbl.Content = item;
                    lbl.BorderThickness = new Thickness(1);
                    lbl.BorderBrush = Brushes.Black;
                    listBox_users.Items.Add(lbl);
                }
            }));
        }

        private void CommandBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            try
            {
                if (listBox_users.SelectedIndex >= 0)
                    e.CanExecute = true;
                else
                    e.CanExecute = false;
            }
            catch
            {
                e.CanExecute = false;
            }
        }

        private void button_accept_Click(object sender, RoutedEventArgs e)
        {
            UserData user = ((listBox_users.SelectedItem as Label).Content as UserData);
            Instruction instr = new Instruction(Operation.RequestResult, from, user.Nickname, true);
            byte[] data = MyObjectConverter.ObjectToByteArray(instr);
            stream.WriteAsync(data, 0, data.Length);

            listBox_users.Items.Remove(listBox_users.SelectedItem);
        }

        private void button_decline_Click(object sender, RoutedEventArgs e)
        {
            UserData user = ((listBox_users.SelectedItem as Label).Content as UserData);
            Instruction instr = new Instruction(Operation.RequestResult, from, user.Nickname, false);
            byte[] data = MyObjectConverter.ObjectToByteArray(instr);
            stream.WriteAsync(data, 0, data.Length);

            listBox_users.Items.Remove(listBox_users.SelectedItem);
        }
    }
}
