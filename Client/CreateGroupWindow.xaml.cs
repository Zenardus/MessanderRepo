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
using ChatInstruction;

namespace Client
{
    /// <summary>
    /// Логика взаимодействия для CreateGroupWindow.xaml
    /// </summary>
    public partial class CreateGroupWindow : Window
    {
        NetworkStream stream;
        string from;

        public CreateGroupWindow(NetworkStream stream, string from)
        {
            InitializeComponent();

            this.stream = stream;
            this.from = from;
        }

        private void button_cancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }
        private void button_create_Click(object sender, RoutedEventArgs e)
        {
            if (listBox_members.Items.Count <= 1)
            {
                MessageBox.Show("Повинно бути мінімум 2 учасники");
                return;
            }
            if (textBox_groupName.Text.Trim(' ').Length < 3)
            {
                MessageBox.Show("Назва групи повинна складатися як мінімум з трьох символів");
                return;
            }
            List<UserData> users = new List<UserData>();
            foreach(UserData item in listBox_members.Items)
            {
                users.Add(new UserData(item.Nickname, item.Name, item.Surname));
            }
            Instruction instr = new Instruction(Operation.CreateGroup, from, textBox_groupName.Text, users);
            byte[] data = MyObjectConverter.ObjectToByteArray(instr);
            stream.Write(data, 0, data.Length);

            this.DialogResult = true;
        }
        private void button_add_Click(object sender, RoutedEventArgs e)
        {
            if (listBox_friends.SelectedIndex >= 0)
            {
                listBox_members.Items.Add(listBox_friends.SelectedItem);
                listBox_friends.Items.Remove(listBox_friends.SelectedItem);
            }
        }
        private void button_remove_Click(object sender, RoutedEventArgs e)
        {
            if (listBox_members.SelectedIndex >= 0)
            {
                listBox_friends.Items.Add(listBox_members.SelectedItem);
                listBox_members.Items.Remove(listBox_members.SelectedItem);
            }
        }

        public void SetList(List<UserData> users)
        {
            listBox_friends.Dispatcher.Invoke(new Action(() =>
            {
                foreach (var user in users)
                    listBox_friends.Items.Add(user);
            }));
        }
    }
}
