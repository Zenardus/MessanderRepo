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
using Client.MyControls;

namespace Client
{
    /// <summary>
    /// Логика взаимодействия для MessagesList.xaml
    /// </summary>
    public partial class MessagesList : Page
    {
        string from;
        NetworkStream stream;

        public MessagesList(NetworkStream stream, string from)
        {
            InitializeComponent();
            this.stream = stream;
            this.from = from;
        }
        public void SetList(List<MessageData> messages)
        {
            listBox_messages.Dispatcher.Invoke(new Action(() =>
            {
                listBox_messages.Items.Clear();

                foreach (var item in messages)
                {
                    Label lbl = new Label();
                    lbl.Foreground = Brushes.WhiteSmoke;
                    lbl.HorizontalContentAlignment = HorizontalAlignment.Left;
                    lbl.Content = item;
                    lbl.BorderThickness = new Thickness(1);
                    lbl.BorderBrush = Brushes.Black;
                    listBox_messages.Items.Add(lbl);
                }
            }));
        }
        private void listBox_messages_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (listBox_messages.SelectedIndex >= 0)
            {
                MessageData msg = ((listBox_messages.SelectedItem as Label).Content as MessageData);
                Instruction instr = new Instruction(Operation.GetMessagesFrom, from, msg.Nickname, null);
                byte[] data = MyObjectConverter.ObjectToByteArray(instr);
                stream.WriteAsync(data, 0, data.Length);
            }
        }
        public void NewMessage(MessageData message)
        {
            listBox_messages.Dispatcher.Invoke(new Action(() =>
            {
                for (int i = 0; i < listBox_messages.Items.Count; i++)
                {
                    if (((listBox_messages.Items[i] as Label).Content as MessageData).Nickname == message.Nickname)
                    {
                        listBox_messages.Items.RemoveAt(i);
                        
                        break;
                    }

                }
                Label lbl = new Label();
                lbl.HorizontalContentAlignment = HorizontalAlignment.Left;
                lbl.Foreground = Brushes.WhiteSmoke;
                lbl.Content = message;
                lbl.BorderThickness = new Thickness(1);
                lbl.BorderBrush = Brushes.Black;
                listBox_messages.Items.Insert(0, lbl);
            }));
        }
    }
}
