﻿using System;
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
using System.Threading;
using System.Net.Sockets;
using System.Net;
using ChatInstruction;
using System.Collections.ObjectModel;
//using System.Windows.Forms;


namespace Client
{
    enum CurrentPage { None, MessagesList, Messages, Groups }
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //--------------НАВІГАЦІЙНІ СТОРІНКИ
        MessagesPage messages;
        FriendsPage friends;
        AddFriendPage addFriend;
        GroupsPage groupsList;
        MessagesList messagesList;
        RequestsPage requests;

        CreateGroupWindow createGroupWindow;

        //---------------МЕРЕЖЕВІ ЗМІННІ
        IPAddress serverIP = IPAddress.Parse("127.0.0.1");
        int serverPort = 1555;
        TcpClient client;
        NetworkStream stream;

        //---------------ДАНІ ДОДАТКУ
        string login;
        CurrentPage currentPage = CurrentPage.None;
        string currentReceiver = "";
        

        public MainWindow()
        {
            InitializeComponent();

            if (ConnectToServer()) //якщо законектилось до серверу
            {
                //Authorization
                AuthorizationWindow authorize = new AuthorizationWindow(stream);
                if (authorize.ShowDialog() == true) //success authorization
                {
                    login = authorize.Login;
                    this.label_nickname.Content = login;
                    //System.Windows.MessageBox.Show("success");


                    messages = new MessagesPage(stream, login);
                    friends = new FriendsPage(stream, login);
                    addFriend = new AddFriendPage(stream, login);
                    groupsList = new GroupsPage(stream, login);
                    messagesList = new MessagesList(stream, login);
                    requests = new RequestsPage();

                    createGroupWindow = new CreateGroupWindow(stream, login);

                    Instruction instr = new Instruction(Operation.AddOnline, login, null, null);
                    byte[] data = MyObjectConverter.ObjectToByteArray(instr);
                    stream.Write(data, 0, data.Length);

                    Thread receiverThread = new Thread(Receiver);
                    receiverThread.IsBackground = true;
                    receiverThread.Start();

                } else //fail authorization
                {
                    this.Close();
                }
                

            } else
            {
                System.Windows.MessageBox.Show("Сервер не відповідає\nСпробуйте пізніше");
                this.Close();
            }
            
        }

        //NAVIGATION BUTTONS
        private void button_messages_Click(object sender, RoutedEventArgs e)
        {
            currentPage = CurrentPage.MessagesList;

            frame.NavigationService.Navigate(messagesList);

            Instruction instr = new Instruction(Operation.GetMessages, login, null, null);
            byte[] data = MyObjectConverter.ObjectToByteArray(instr);
            stream.WriteAsync(data, 0, data.Length);
        }
        private void button_groups_Click(object sender, RoutedEventArgs e)
        {
            frame.NavigationService.Navigate(groupsList);

            Instruction instr = new Instruction(Operation.Groups, login, null, null);
            byte[] data = MyObjectConverter.ObjectToByteArray(instr);
            stream.WriteAsync(data, 0, data.Length);

            currentPage = CurrentPage.Groups;
        }
        private void button_friends_Click(object sender, RoutedEventArgs e)
        {
            frame.NavigationService.Navigate(friends);

            Instruction instruction = new Instruction(Operation.Friends, login, null, null);
            byte[] data = MyObjectConverter.ObjectToByteArray(instruction);
            stream.WriteAsync(data, 0, data.Length);

            currentPage = CurrentPage.None;
        }
        //REQUESTS TODO
        private void button_requests_Click(object sender, RoutedEventArgs e)
        {
            frame.NavigationService.Navigate(requests);
            currentPage = CurrentPage.None;
        }
        private void button_addFriends_Click(object sender, RoutedEventArgs e)
        {
            frame.NavigationService.Navigate(addFriend);

            currentPage = CurrentPage.None;
        }

        //SERVER METHODS
        private bool ConnectToServer()
        {
            client = new TcpClient();
            try
            {
                client.Connect(serverIP, serverPort);
                stream = client.GetStream();
            }
            catch(Exception)
            {
                return false;
            }
            return true;

        }
        private void StartReceiveThread()
        {
            Thread receiveThread = new Thread(Receiver);
            receiveThread.IsBackground = true;
            receiveThread.Start();
        }
        private void Receiver()
        {
            try
            {
                while (true)
                {
                    byte[] data = new byte[10240];
                    stream.Read(data, 0, data.Length);
                    Instruction instr = MyObjectConverter.ByteArrayToObject(data) as Instruction;
                    InstructionHandler(instr);
                }
            }
            catch(Exception)
            {
                System.Windows.MessageBox.Show("Сервер перестав відповідати");
                //this.Close();
                this.Dispatcher.Invoke(new Action(() =>
                {
                    this.Close();
                }));
            }
        }
        private void InstructionHandler(Instruction instruction)
        {
            switch (instruction.Operation)
            {
                //SearchUser
                case Operation.SearchUser:
                    SearchedUser(instruction);
                    break;
                //AddFriend
                case Operation.AddFriend:
                    AddFriend(instruction);
                    break;
                //Friends
                case Operation.Friends:
                    Friends(instruction);
                    break;
                //GetMessages
                case Operation.GetMessages:
                    GetMessages(instruction);
                    break;
                //GetMessagesFrom
                case Operation.GetMessagesFrom:
                    GetMessagesFrom(instruction);
                    break;
                //PersonalMessage
                case Operation.PersonalMessage:
                    PersonalMessage(instruction);
                    break;
                //Groups
                case Operation.Groups:
                    Groups(instruction);
                    break;
                //GroupsMembers
                case Operation.GroupsMembers:
                    GroupsMembers(instruction);
                    break;
                //GetMessagesFromGroup
                case Operation.GetMessagesFromGroup:
                    GetMessagesFromGroup(instruction);
                    break;
                //GroupMessage
                case Operation.ReceiveGroupMessage:
                    GroupMessage(instruction);
                    break;
            }
        }
        private void SearchedUser(Instruction instr)
        {
            addFriend.SetList(instr.Data as List<UserData>);
        }
        //<<
        private void AddFriend(Instruction instr)
        {
            //if ((bool)instr.Data)
            //    MessageBox.Show(instr.From);
            //else
            //{
            //    MessageBox.Show(instr.From);
            //}
        }
        //<<
        private void Friends(Instruction instr)
        {
            //friends.SetList(instr.Data as List<UserData>);
        }
        //<<
        private void GetMessages(Instruction instr)
        {
            //messagesList.SetList(instr.Data as List<MessageData>);
        }
        //<<
        private void GetMessagesFrom(Instruction instr)
        {
            //currentReceiver = instr.From;
            //List<MessageData> messages = instr.Data as List<MessageData>;
            //this.messages.ClearList();
            //this.messages.SetIsPersonalMessage(true);
            //this.messages.SetUser(instr.From);
            //foreach(var item in messages)
            //{
            //    if (item.Nickname == instr.From)
            //        this.messages.AddMessage(item.Message, HorizontalAlignment.Left);
            //    else
            //        this.messages.AddMessage(item.Message, HorizontalAlignment.Right);
            //}
            //currentPage = CurrentPage.Messages;
            //frame.Dispatcher.Invoke(new Action(() =>
            //{
            //    frame.NavigationService.Navigate(this.messages);
            //}));
        }
        //<<
        private void PersonalMessage(Instruction instr)
        {
            //if (currentPage == CurrentPage.MessagesList)
            //{
            //    messagesList.NewMessage(instr.Data as MessageData);
            //}
            //else if (currentPage == CurrentPage.Messages && currentReceiver == (instr.Data as MessageData).Nickname)
            //{
            //    messages.AddMessage((instr.Data as MessageData).Message, HorizontalAlignment.Left);
            //}
            //else
            //    textBox_notification.Dispatcher.Invoke(new Action(() =>
            //    {
            //        textBox_notification.Text = "";
            //        textBox_notification.Text = $"you have new message from {(instr.Data as MessageData).Nickname}";
            //    }));
        }
        private void Groups(Instruction instr)
        {
            groupsList.SetList(instr.Data as List<GroupData>);
        }
        private void GroupsMembers(Instruction instr)
        {
            createGroupWindow.SetList(instr.Data as List<UserData>);
            createGroupWindow.Dispatcher.Invoke(new Action(() =>
            {
                createGroupWindow.ShowDialog();
                createGroupWindow = new CreateGroupWindow(stream, login);
            }));
        }
        //<<
        private void GetMessagesFromGroup(Instruction instr)
        {
            //this.messages.ClearList();
            //this.messages.SetIsPersonalMessage(false);
            //this.messages.SetUser(instr.To);
            //currentReceiver = instr.To;
            //this.messages.SetGroupID(int.Parse(instr.From));
            //List<MessageData> messages = instr.Data as List<MessageData>;
            //foreach (var item in messages)
            //{
            //    if (item.Nickname != login)
            //        this.messages.AddMessage($"{item.Nickname}: " + item.Message, HorizontalAlignment.Left);
            //    else
            //        this.messages.AddMessage(item.Message, HorizontalAlignment.Right);
            //}
            //currentPage = CurrentPage.Messages;
            //frame.Dispatcher.Invoke(new Action(() =>
            //{
            //    frame.NavigationService.Navigate(this.messages);
            //}));

        }
        //<<
        private void GroupMessage(Instruction instr)
        {
            //if (currentPage == CurrentPage.Groups)
            //{
            //    groupsList.NewMessage(instr.Data as GroupData);
            //}
            //else if (currentPage == CurrentPage.Messages && currentReceiver == (instr.Data as GroupData).Name)
            //{
            //    messages.AddMessage(instr.From + ": " + (instr.Data as GroupData).LastMessage, HorizontalAlignment.Left);
            //}
            //else
            //{
            //    textBox_notification.Dispatcher.Invoke(new Action(() =>
            //    {
            //        textBox_notification.Text = "";
            //        textBox_notification.Text = $"you have new message from group: {(instr.Data as GroupData).Name}";
            //    }));
            //}

        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            //при закритті програми клієнт і стрім не закриваю тому, що якщо їх закрити,
            //то на сервері при отриманні даних від цього клієнта не вилітає ексепшн. 
            //Через це, потік по прийому даних від цього користувача зациклюється назавжди

            //if (stream != null)
            //    stream.Close();
            //if (client != null)
            //    client.Close();

            try
            {
                createGroupWindow.Close();
            }
            catch
            {

            }
        }

        
    }
}