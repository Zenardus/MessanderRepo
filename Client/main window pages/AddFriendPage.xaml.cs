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
using System.Net;
using System.Net.Sockets;
using ChatInstruction;

namespace Client
{
    /// <summary>
    /// Логика взаимодействия для AddFriendPage.xaml
    /// </summary>
    public partial class AddFriendPage : Page
    {
        NetworkStream stream;
        string from;

        public AddFriendPage(NetworkStream s, string from)
        {
            InitializeComponent();
            stream = s;
            this.from = from;
        }

        private void textBox_search_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (textBox_search.Text.Length > 0)
            {
                Instruction instr = new Instruction(Operation.SearchUser, from, null, textBox_search.Text);
                byte[] data = MyObjectConverter.ObjectToByteArray(instr);
                stream.WriteAsync(data, 0, data.Length);
            }
            else
            {
                listBox_users.Dispatcher.Invoke(new Action(() =>
                {
                    listBox_users.Items.Clear();
                }));
            }
        }
        public void SetList(List<UserData> users)
        {
            listBox_users.Dispatcher.Invoke(new Action(() =>
            {
                listBox_users.Items.Clear();
                foreach (var item in users)
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

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            UserData user = ((listBox_users.SelectedItem as Label).Content as UserData);
            Instruction instr = new Instruction(Operation.FriendRequest, from, user.Nickname, null);
            byte[] data = MyObjectConverter.ObjectToByteArray(instr);
            stream.WriteAsync(data, 0, data.Length);
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

        private void listBox_users_LostFocus(object sender, RoutedEventArgs e)
        {
            //this.listBox_users.UnselectAll();
        }
    }
}
