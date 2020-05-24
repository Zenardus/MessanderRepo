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
using Client.MyControls;
using System.IO;
using System.Windows.Forms;

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
        bool attachment = false;
        byte[] data;
        string fileName = "";

        public MessagesPage(NetworkStream stream, string from)
        {
            InitializeComponent();

            this.stream = stream;
            this.from = from;

        }
        public void AddMessage(MessageData message, System.Windows.HorizontalAlignment aligment)
        {
            
            listBox_messages.Dispatcher.Invoke(new Action(() =>
            {

                if (message.Type == MessageType.TextMessage)
                {
                    MessageControl msg;
                    if (isPersonalMessage)
                        msg = new MessageControl((string)message.Message, message.Time.ToShortTimeString());
                    else
                    {
                        string text = ((string)message.Message).Replace($"{from}:\n", "");
                        msg = new MessageControl(text, message.Time.ToShortTimeString());
                    }
                    msg.HorizontalAlignment = aligment;
                    msg.Margin = new Thickness(1);
                    listBox_messages.Items.Add(msg);
                    listBox_messages.ScrollIntoView(msg);
                }
                else
                {
                    if (((string)message.Message).EndsWith(".jpeg") || ((string)message.Message).EndsWith(".jpg") || ((string)message.Message).EndsWith(".png"))
                    {
                        PhotoControl photo = new PhotoControl((string)message.Message, message.Time.ToShortTimeString(), stream);
                        photo.HorizontalAlignment = aligment;
                        photo.Margin = new Thickness(1);
                        listBox_messages.Items.Add(photo);
                        listBox_messages.ScrollIntoView(photo);
                    }
                    else
                    {
                        FileControl file = new FileControl((string)message.Message, "", message.Time.ToShortTimeString(), stream);
                        file.HorizontalAlignment = aligment;
                        file.Margin = new Thickness(1);
                        listBox_messages.Items.Add(file);
                        listBox_messages.ScrollIntoView(file);
                    }
                }

                
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
            MessageData message;
            if (attachment)
            {
                message = new MessageData(null, fileName, data, DateTime.Now, MessageType.File);
            }
            else
            {
                message = new MessageData(null, null, textBox_message.Text, DateTime.Now, MessageType.TextMessage);
            }
            if (isPersonalMessage)
            {
                Instruction instr = new Instruction(Operation.PersonalMessage, from, label_user.Content.ToString(), message);
                textBox_message.Clear();
                byte[] data = MyObjectConverter.ObjectToByteArray(instr);
                stream.WriteAsync(data, 0, data.Length);
                if (attachment)
                {
                    MessageData disp = new MessageData(null, null, fileName, DateTime.Now, MessageType.File);
                    AddMessage(disp, System.Windows.HorizontalAlignment.Right);
                }
                else
                {
                    AddMessage(message, System.Windows.HorizontalAlignment.Right);
                }

            }
            else
            {
                Instruction instr = new Instruction(Operation.GroupMessage, from, groupID.ToString(), textBox_message.Text);
                MessageData msg = new MessageData("none", "none", textBox_message.Text, DateTime.Now);
                AddMessage(msg, System.Windows.HorizontalAlignment.Right);
                textBox_message.Clear();
                byte[] data = MyObjectConverter.ObjectToByteArray(instr);
                stream.Write(data, 0, data.Length);
            }
        }

        private void CommandBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            try
            {
                if (textBox_message.Text.Trim(' ', '\r', '\n').Length > 0)
                    e.CanExecute = true;
                else
                    e.CanExecute = false;
            }
            catch
            {
                e.CanExecute = false;
            }
        }

        private void button_attachment_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            //ofd.Filter = "txt file (*.txt)|*.txt|"
            //           + "Image files (*.jpg, *.jpeg, *.png) | *.jpg; *.jpeg; *.png";
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                data = File.ReadAllBytes(ofd.FileName);
                if(!(ofd.SafeFileName.EndsWith(".png") || ofd.SafeFileName.EndsWith(".jpeg") || ofd.SafeFileName.EndsWith(".jpg")))
                if (data.Length > 1024 * 1024 * 100)
                {
                    System.Windows.MessageBox.Show("Файл повинен займати менше 100мб");
                    data = null;
                    return;
                }
                attachment = true;
                fileName = ofd.SafeFileName;
                this.textBox_message.Text = $"[{ofd.SafeFileName}]";
            }

        }

        private void textBox_message_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (attachment && e.Key == Key.Back)
            {
                textBox_message.Clear();
                attachment = false;
                data = null;
                fileName = "";
                e.Handled = true;
            }
            else if (attachment)
            {
                e.Handled = true;
            }
        }

        private void CommandBinding_CanExecute_1(object sender, CanExecuteRoutedEventArgs e)
        {
            if (isPersonalMessage)
                e.CanExecute = true;
            else
                e.CanExecute = false;
        }

        private void listBox_messages_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            listBox_messages.UnselectAll();
            e.Handled = true;
        }
        public PhotoControl GetPhoto(string name)
        {
            foreach (var item in listBox_messages.Items)
            {
                try
                {
                    PhotoControl photo = item as PhotoControl;
                    if (photo.Name == name)
                        return photo;
                }
                catch
                {

                }
            }
            return null;

        }
    }
}
