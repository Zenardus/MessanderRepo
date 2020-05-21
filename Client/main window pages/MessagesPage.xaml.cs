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
    /// Логика взаимодействия для MessagesPage.xaml
    /// </summary>
    public partial class MessagesPage : Page
    {
        string from;
        NetworkStream stream;
        bool isPersonalMessage;
        int groupID;

        public MessagesPage(NetworkStream stream, string from)
        {
            InitializeComponent();

            this.stream = stream;
            this.from = from;

        }
        public void AddMessage(string message, HorizontalAlignment aligment)
        {
            listBox_messages.Dispatcher.Invoke(new Action(() =>
            {
                //TODO: розбивання тексту
                TextBox b = new TextBox();
                b.Text = message;
                b.IsReadOnly = true;
                b.BorderThickness = new Thickness(0);
                b.HorizontalAlignment = aligment;
                b.Background = Brushes.DarkViolet;
                b.Foreground = Brushes.WhiteSmoke;
                b.Margin = new Thickness(1);
                b.Padding = new Thickness(5);

                listBox_messages.Items.Add(b);
                listBox_messages.ScrollIntoView(b);
            }));
        }
        public void ClearList()
        {
            listBox_messages.Dispatcher.Invoke(new Action(() =>
            {
                listBox_messages.Items.Clear();
            }));
        }
        public void SetUser(string name)
        {
            label_user.Dispatcher.Invoke(new Action(() =>
            {
                label_user.Content = name;
            }));
        }
        public void SetGroupID(int id)
        {
            groupID = id;
        }
        public void SetIsPersonalMessage(bool value)
        {
            isPersonalMessage = value;
        }

        private void button_send_Click(object sender, RoutedEventArgs e)
        {
            if (isPersonalMessage)
            {
                Instruction instr = new Instruction(Operation.PersonalMessage, from, label_user.Content.ToString(), textBox_message.Text);
                AddMessage(textBox_message.Text, HorizontalAlignment.Right);
                textBox_message.Clear();
                byte[] data = MyObjectConverter.ObjectToByteArray(instr);
                stream.WriteAsync(data, 0, data.Length);
            }
            else
            {
                Instruction instr = new Instruction(Operation.GroupMessage, from, groupID.ToString(), textBox_message.Text);
                AddMessage(textBox_message.Text, HorizontalAlignment.Right);
                textBox_message.Clear();
                byte[] data = MyObjectConverter.ObjectToByteArray(instr);
                stream.Write(data, 0, data.Length);
            }
        }

        private void CommandBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            try
            {
                if (textBox_message.Text.Trim(' ').Length > 0)
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
