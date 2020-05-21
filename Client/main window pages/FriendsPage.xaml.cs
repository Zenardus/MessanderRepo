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
using System.Net;
using System.Net.Sockets;

namespace Client
{
    /// <summary>
    /// Логика взаимодействия для FriendsPage.xaml
    /// </summary>
    public partial class FriendsPage : Page
    {
        NetworkStream stream;
        string from;

        public FriendsPage(NetworkStream stream, string from)
        {
            InitializeComponent();

            this.stream = stream;
            this.from = from;
        }

        public void SetList(List<UserData> friends)
        {
            listBox_friends.Dispatcher.Invoke(new Action(() =>
            {
                listBox_friends.Items.Clear();

                foreach (var item in friends)
                {
                    Label lbl = new Label();
                    lbl.Foreground = Brushes.WhiteSmoke;
                    lbl.HorizontalContentAlignment = HorizontalAlignment.Left;
                    lbl.Content = item;
                    lbl.BorderThickness = new Thickness(1);
                    lbl.BorderBrush = Brushes.Black;
                    listBox_friends.Items.Add(lbl);
                }
            }));
        }

        private void Button_DeleteClick(object sender, RoutedEventArgs e)
        {
            listBox_friends.Dispatcher.Invoke(new Action(() =>
            {
                UserData user = ((listBox_friends.SelectedItem as Label).Content as UserData);
                Instruction instr = new Instruction(Operation.DeleteFriend, from, user.Nickname, null);

                listBox_friends.Items.Remove(listBox_friends.SelectedItem);

                byte[] data = MyObjectConverter.ObjectToByteArray(instr);
                stream.WriteAsync(data, 0, data.Length);

            }));
        }

        private void button_write_Click(object sender, RoutedEventArgs e)
        {
            listBox_friends.Dispatcher.Invoke(new Action(() =>
            {
                UserData user = ((listBox_friends.SelectedItem as Label).Content as UserData);
                Instruction instr = new Instruction(Operation.GetMessagesFrom, from, user.Nickname, null);
                byte[] data = MyObjectConverter.ObjectToByteArray(instr);
                stream.WriteAsync(data, 0, data.Length);
            }));
        }

        private void CommandBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            try
            {
                if (listBox_friends.SelectedIndex >= 0)
                    e.CanExecute = true;
                else
                    e.CanExecute = false;
            }
            catch
            {
                e.CanExecute = false;
            }
        }
    }
}
