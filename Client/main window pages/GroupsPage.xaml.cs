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
using System.Net.Sockets;

namespace Client
{
    /// <summary>
    /// Логика взаимодействия для Groups.xaml
    /// </summary>
    public partial class GroupsPage : Page
    {
        NetworkStream stream;
        string from;

        public GroupsPage(NetworkStream stream, string from)
        {
            InitializeComponent();

            this.stream = stream;
            this.from = from;
        }
        public void SetList(List<GroupData> groups)
        {
            listBox_groups.Dispatcher.Invoke(new Action(() =>
            {
                listBox_groups.Items.Clear();

                foreach (var item in groups)
                {
                    Label lbl = new Label();
                    lbl.Foreground = Brushes.WhiteSmoke;
                    lbl.HorizontalContentAlignment = HorizontalAlignment.Left;
                    lbl.Content = item;
                    lbl.BorderThickness = new Thickness(1);
                    lbl.BorderBrush = Brushes.Black;
                    listBox_groups.Items.Add(lbl);
                }
            }));
        }

        private void button_createGroup_Click(object sender, RoutedEventArgs e)
        {
            Instruction instr = new Instruction(Operation.GroupsMembers, from, null, null);
            byte[] data = MyObjectConverter.ObjectToByteArray(instr);
            stream.WriteAsync(data, 0, data.Length);
        }

        private void listBox_groups_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (listBox_groups.SelectedIndex >= 0)
            {
                GroupData g = ((listBox_groups.SelectedItem as Label).Content as GroupData);
                Instruction instr = new Instruction(Operation.GetMessagesFromGroup, null, null, g.ID);
                byte[] data = MyObjectConverter.ObjectToByteArray(instr);
                stream.WriteAsync(data, 0, data.Length);
            }
        }

        public void NewMessage(GroupData message)
        {
            listBox_groups.Dispatcher.Invoke(new Action(() =>
            {
                for (int i = 0; i < listBox_groups.Items.Count; i++)
                {
                    if (((listBox_groups.Items[i] as Label).Content as GroupData).ID == message.ID)
                    {
                        listBox_groups.Items.RemoveAt(i);
                        break;
                    }

                }
                Label lbl = new Label();
                lbl.HorizontalContentAlignment = HorizontalAlignment.Left;
                lbl.Foreground = Brushes.WhiteSmoke;
                lbl.Content = message;
                lbl.BorderThickness = new Thickness(1);
                lbl.BorderBrush = Brushes.Black;
                listBox_groups.Items.Insert(0, lbl);
            }));
        }
    }
}
