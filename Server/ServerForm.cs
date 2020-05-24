using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using ChatInstruction;
using System.Runtime.InteropServices;
using System.Collections.ObjectModel;
using System.IO;
//using System.Windows.Media.Imaging;

namespace Server
{
    public partial class ServerForm : Form
    {
        //---------------МЕРЕЖЕВІ ЗМІННІ
        IPAddress serverIP = IPAddress.Parse("127.0.0.1");
        int serverPort = 1555;
        TcpListener server;
        //---------------БАЗА ДАНИХ
        ChatDBEntities db = new ChatDBEntities();
        //---------------ONLINE USERS
        List<OnlineUsers> online = new List<OnlineUsers>();
        //---------------counter for files name
        int counter = 0;
        //---------------path to save files
        string savePath = @"D:\MessangerDataBaseFiles\";



        public ServerForm()
        {
            InitializeComponent();

            server = new TcpListener(serverIP, serverPort);
            server.Start();

            Thread acceptThread = new Thread(AcceptClient);
            acceptThread.IsBackground = true;
            acceptThread.Start();


        }
        //SERVER METHODS
        private void AcceptClient()
        {
            try
            {
                while (true)
                {
                    TcpClient client = server.AcceptTcpClient();
                    listBox_log.Invoke(new Action(() =>
                    {
                        listBox_log.Items.Add(client.Client.RemoteEndPoint + " connected");
                    }));
                    Thread receiveThread = new Thread(ReceiveData);
                    receiveThread.IsBackground = true;
                    receiveThread.Start(client);

                }
            }
            catch(Exception e)
            {
                listBox_log.Invoke(new Action(() =>
                {
                    listBox_log.Items.Add(e.Message);
                }));
            }
        }
        private void ReceiveData(object obj)
        {
            TcpClient client = obj as TcpClient;
            NetworkStream stream = client.GetStream();
            try
            {
                while (true)
                {
                    //try
                    //{
                        byte[] data = new byte[5000000];
                        stream.Read(data, 0, data.Length);
                        Instruction instr = MyObjectConverter.ByteArrayToObject(data) as Instruction;
                        //listBox_log.Invoke(new Action(() =>
                        //{
                        //    listBox_log.Items.Add($"{instr.Operation} {instr.From} {instr.Data}");
                        //}));
                        InstructionHandler(instr, stream);
                    //}
                    //catch (NotSupportedException e)
                    //{

                    //}
                }
            }
            catch(Exception)
            {
                listBox_log.Invoke(new Action(() =>
                {
                    listBox_log.Items.Add(client.Client.RemoteEndPoint + " disconnected");
                }));
                RemoveOnline(stream);
            }
        }

        //INSTRRUCTIONS HANDLER
        private void InstructionHandler(Instruction instruction, NetworkStream stream)
        {
            switch (instruction.Operation)
            {
                //REGISTER
                case Operation.Register:
                    Register(instruction.Data as RegistrationData, stream);
                    break;
                //LOGIN
                case Operation.Login:
                    Login(instruction.From, (string)instruction.Data, stream);
                    break;
                //SearchUser
                case Operation.SearchUser:
                    SearchUser(instruction.From, (string)instruction.Data, stream);
                    break;
                //AddFriend
                case Operation.AddFriend:
                    AddFriend(instruction.From, instruction.To, stream);
                    break;
                //Friends
                case Operation.Friends:
                    Friends(instruction.From, stream);
                    break;
                //DeleteFriend
                case Operation.DeleteFriend:
                    DeleteFriend(instruction.From, instruction.To);
                    break;
                //AddOnline
                case Operation.AddOnline:
                    AddOnline(instruction.From, stream);
                    break;
                //GetMessages
                case Operation.GetMessages:
                    GetMessages(instruction.From, stream);
                    break;
                //GetMessagesFrom
                case Operation.GetMessagesFrom:
                    GetMessagesFrom(instruction.From, instruction.To, stream);
                    break;
                //PersonalMessage
                case Operation.PersonalMessage:
                    PersonalMessage(instruction, stream);
                    break;
                //Groups
                case Operation.Groups:
                    Groups(instruction.From, stream);
                    break;
                //GroupsMembers
                case Operation.GroupsMembers:
                    GroupsMembers(instruction.From, stream);
                    break;
                //CreateGroup
                case Operation.CreateGroup:
                    CreateGroup(instruction, stream);
                    break;
                //GetMessagesFromGroup
                case Operation.GetMessagesFromGroup:
                    GetMessageFromGroup((int)instruction.Data, stream);
                    break;
                //GroupMessage
                case Operation.GroupMessage:
                    GroupMessage(instruction);
                    break;
                case Operation.FriendRequest:
                    FriendRequest(instruction.From, instruction.To, stream);
                    break;
                case Operation.GetFriendRequests:
                    GetFriendsRequests(instruction.From, stream);
                    break;
                case Operation.RequestResult:
                    RequestResult(instruction, stream);
                    break;
                case Operation.GetFile:
                    GetFile(instruction, stream);
                    break;
            }
            
        }

        //INSTRUCTION METHODS
        private bool CanRegister(string login)
        {
            if (db.User.Where(usr => usr.nickname == login).ToList().Count == 0)
                return true;
            else
                return false;
        } 
        private void Register(RegistrationData reg, NetworkStream stream)
        {
            Instruction instr;
            if (CanRegister(reg.Login))
            {
                User user = new User()
                {
                    name = reg.Name,
                    surname = reg.Surname,
                    nickname = reg.Login,
                    password = reg.Password
                };
                db.User.Add(user);
                db.SaveChangesAsync();
                instr = new Instruction(Operation.Register, null, null, true);
            }
            else
            {
                instr = new Instruction(Operation.Register, "User already exist", null, false);
            }
            byte[] data = MyObjectConverter.ObjectToByteArray(instr);
            stream.WriteAsync(data, 0, data.Length);
        }
        private void Login(string login, string password, NetworkStream stream)
        {
            Instruction instr;
            try
            {
                User user = db.User.Where(usr => usr.nickname == login).First();
                if (user.password == password)
                    instr = new Instruction(Operation.Login, null, null, true);
                else
                    instr = new Instruction(Operation.Login, "Логін або пароль невірний", null, false);
            }
            catch (Exception)
            {
                instr = new Instruction(Operation.Login, "Такого користувача немає", null, false);
            }
            byte[] data = MyObjectConverter.ObjectToByteArray(instr);
            stream.WriteAsync(data, 0, data.Length);
        }
        private void SearchUser(string from, string name, NetworkStream stream)
        {
            List<User> list = db.User.Where(usr => 
                    (usr.name.StartsWith(name) ||
                    usr.surname.StartsWith(name) ||
                    usr.nickname.StartsWith(name)) &&
                    usr.nickname != from).ToList();
            List<UserData> users = new List<UserData>();
            foreach (var user in list)
            {
                users.Add(new UserData(user.nickname, user.name, user.surname));
                //listBox_log.Invoke(new Action(() =>
                //{
                //    listBox_log.Items.Add(user.ToString());
                //}));
            }
            Instruction instr = new Instruction(Operation.SearchUser, null, null, users);
            byte[] data = MyObjectConverter.ObjectToByteArray(instr);
            stream.Write(data, 0, data.Length); //не єбу, що не так, але без цього надсилає через раз
            stream.Write(data, 0, data.Length); //чому з першого разу не відправляє, я не шарю
        }
        //<< USELESS !!!!!!!! (DELETE)
        private void AddFriend(string from, string to, NetworkStream stream)
        {
            //int fromID = GetUserId(from);
            //int toID = GetUserId(to);

            //Instruction instr;

            //if (fromID == -1 || toID == -1)
            //{
            //        instr = new Instruction(Operation.AddFriend, "Користувача не знайдено\nОновіть сторінку юзерів", null, false);
            //}
            //else
            //{
            //    var tmp = db.Friends.Where(item => item.userID == fromID && item.friendID == toID).ToList();
            //    if (tmp.Count > 0)
            //    {
            //        instr = new Instruction(Operation.AddFriend, "Цей користувач вже є в списку друзів", null, false);
            //    }
            //    else
            //    {
            //        try
            //        {
            //            Friends friend = new Friends()
            //            {
            //                userID = GetUserId(from),
            //                friendID = GetUserId(to)
            //            };
            //            db.Friends.Add(friend);
            //            db.SaveChangesAsync();

            //            instr = new Instruction(Operation.AddFriend, "Користувача додано в список друзів", null, true);
            //        }
            //        catch (Exception e)
            //        {
            //            instr = new Instruction(Operation.AddFriend, e.Message, null, false);
            //        }
            //    }
            //}
            //byte[] data = MyObjectConverter.ObjectToByteArray(instr);
            //stream.WriteAsync(data, 0, data.Length);
            //stream.WriteAsync(data, 0, data.Length);
        }
        private void Friends(string from, NetworkStream stream)
        {
            int userID = GetUserId(from);

            List<Friends> tmp = db.Friends.Where(usr => usr.userID == userID || usr.friendID == userID).ToList();
            List<UserData> friends = new List<UserData>();
            foreach (var item in tmp)
            {
                if (item.userID == userID)
                    friends.Add(new UserData(item.User.nickname, item.User.name, item.User.surname));
                else
                    friends.Add(new UserData(item.User1.nickname, item.User1.name, item.User1.surname));

            }
            Instruction instr = new Instruction(Operation.Friends, null, null, friends);
            byte[] data = MyObjectConverter.ObjectToByteArray(instr);
            stream.WriteAsync(data, 0, data.Length);
            stream.WriteAsync(data, 0, data.Length);
        }
        private void DeleteFriend(string from, string to)
        {
            int userID = GetUserId(from);
            int friendID = GetUserId(to);

            try
            {
                List<Friends> friends = db.Friends.Where(usr => (usr.userID == userID && usr.friendID == friendID) || (usr.userID == friendID && usr.friendID == userID)).ToList();
                db.Friends.RemoveRange(friends);
                db.SaveChangesAsync();
            }
            catch (Exception)
            {

            }
        }
        private void AddOnline(string from, NetworkStream stream)
        {
            online.Add(new OnlineUsers(from, stream));
            label_online.Invoke(new Action(() =>
            {
                label_online.Text = online.Count.ToString();
            }));
            listBox_log.Invoke(new Action(() =>
            {
                listBox_log.Items.Add($"{from} is online");
            }));
        }
        private void RemoveOnline(NetworkStream stream)
        {
            for(int i = 0; i < online.Count; i++)
                if (online[i].Stream == stream)
                {
                    listBox_log.Invoke(new Action(() =>
                    {
                        listBox_log.Items.Add($"{online[i].User} is offline");
                    }));
                    online.RemoveAt(i);
                    break;
                }
            label_online.Invoke(new Action(() =>
            {
                label_online.Text = online.Count.ToString();
            }));
        }
        private void GetMessages(string from, NetworkStream stream)
        {
            //Впевнений, що це не самий оптимальний метод отримати останнє
            //повідомлення від кожного користувача, але я писав цей код в 5:14 ранку
            int fromID = GetUserId(from);

            User user = db.User.Where(usr => usr.id == fromID).First();
            var tmp1 = user.Messages.GroupBy(msg => msg.toID).ToList(); //sended msg
            var tmp2 = user.Messages1.GroupBy(msg => msg.fromID).ToList(); //received msg

            List<int> uniqueUser = new List<int>();
            foreach (var item in tmp1)
                uniqueUser.Add(item.Key);
            foreach (var item in tmp2)
                if (!uniqueUser.Contains(item.Key))
                    uniqueUser.Add(item.Key);

            List<MessageData> messages = new List<MessageData>();

            foreach (var usrID in uniqueUser)
            {
                Messages message = db.Messages
                    .Where(msg => (msg.fromID == usrID && msg.toID == fromID) || (msg.fromID == fromID && msg.toID == usrID))
                    .OrderByDescending(msg => msg.date)
                    .First();
                User usr = GetUserById(usrID);
                messages.Add(new MessageData(usr.nickname, $"{usr.surname} {usr.name}", message.message, message.date));
            }

            messages = messages.OrderBy(msg => msg.Time).ToList();
            messages.Reverse();

            Instruction instr = new Instruction(Operation.GetMessages, null, null, messages);
            byte[] data = MyObjectConverter.ObjectToByteArray(instr);
            stream.WriteAsync(data, 0, data.Length);
            stream.WriteAsync(data, 0, data.Length);
        }
        private void GetMessagesFrom(string from, string to, NetworkStream stream)
        {
            int fromID = GetUserId(from);
            int toID = GetUserId(to);

            List<Messages> tmp = db.Messages.Where(msg => (msg.fromID == fromID && msg.toID == toID)
                    || (msg.fromID == toID && msg.toID == fromID))
                    .ToList();
            List<MessageData> messages = new List<MessageData>();
            foreach (var msg in tmp)
            {
                User usr = GetUserById(msg.fromID);
                if (msg.isFile)
                {
                    messages.Add(new MessageData(usr.nickname, $"{usr.surname} {usr.name}", msg.message.Remove(0, msg.message.LastIndexOf("\\") + 1), msg.date, MessageType.File));
                }
                else
                    messages.Add(new MessageData(usr.nickname, $"{usr.surname} {usr.name}", msg.message, msg.date));
            }
            messages = messages.OrderBy(msg => msg.Time).ToList();
            Instruction instr = new Instruction(Operation.GetMessagesFrom, to, null, messages);
            byte[] data = MyObjectConverter.ObjectToByteArray(instr);
            stream.WriteAsync(data, 0, data.Length);
            stream.WriteAsync(data, 0, data.Length);
        }
        private void PersonalMessage(Instruction instruction, NetworkStream strem)
        {
            int _fromID = GetUserId(instruction.From);
            int _toID = GetUserId(instruction.To);

            User user = GetUserById(_fromID);
            MessageData messageFromClient = instruction.Data as MessageData;

            Messages msg;
            if (messageFromClient.Type == MessageType.TextMessage)
            {
                msg = new Messages()
                {
                    fromID = _fromID,
                    toID = _toID,
                    isFile = false,
                    message = (string)messageFromClient.Message,
                    date = DateTime.Now
                };
            }
            else
            {
                // save file
                string path = savePath + messageFromClient.Name;
                while (true)
                {
                    if (File.Exists(path))
                    {
                        path = path.Insert(path.IndexOf('.') - 1, counter.ToString());
                        counter++;
                        continue;
                    }
                    else
                    {
                        FileStream stream = new FileStream(path, FileMode.Create);
                        stream.Write((byte[])messageFromClient.Message, 0, ((byte[])messageFromClient.Message).Length);
                        stream.Close();
                        break;
                    }
                }
                
                msg = new Messages()
                {
                    fromID = _fromID,
                    toID = _toID,
                    isFile = true,
                    message = path,
                    date = DateTime.Now
                };
            }
            
            db.Messages.Add(msg);
            db.SaveChangesAsync();
            if (IsOnline(instruction.To))
            {
                MessageData m;
                if (messageFromClient.Type == MessageType.TextMessage)
                    m = new MessageData(user.nickname, $"{user.surname} {user.name}", msg.message, DateTime.Now);
                else
                    m = new MessageData(user.nickname, $"{user.surname} {user.name}", messageFromClient.Name, DateTime.Now, MessageType.File);
                Instruction instr = new Instruction(Operation.PersonalMessage, null, instruction.To, m);
                byte[] data = MyObjectConverter.ObjectToByteArray(instr);
                NetworkStream s = GetStreamByName(instruction.To);
                s.WriteAsync(data, 0, data.Length);
                s.WriteAsync(data, 0, data.Length);
            }

        }
        private bool IsOnline(string name)
        {
            foreach (var item in online)
                if (item.User == name)
                    return true;
            return false;
        }
        private NetworkStream GetStreamByName(string name)
        {
            for (int i = 0; i < online.Count; i++)
            {
                if (online[i].User == name)
                    return online[i].Stream;
            }
            return null;
        }
        private void Groups(string from, NetworkStream stream)
        {
            int userID = GetUserId(from);
            User user = GetUserById(userID);

            List<GroupMembers> groups = user.GroupMembers.ToList();
            List<GroupData> data = new List<GroupData>();
            foreach (var item in groups)
            {
                Groups group = item.Groups;
                GroupMessages lastMessage;
                try
                {
                    lastMessage = db.GroupMessages.Where(msg => msg.groupID == group.id).OrderByDescending(msg => msg.time).First();
                    data.Add(new GroupData(group.id, group.name, lastMessage.message, lastMessage.time));
                }
                catch
                {
                    data.Add(new GroupData(group.id, group.name, "", new DateTime()));
                }
            }
            data = data.OrderByDescending(x => x.Time).ToList();
            //data.Reverse();
            Instruction instr = new Instruction(Operation.Groups, null, null, data);
            byte[] d = MyObjectConverter.ObjectToByteArray(instr);
            stream.WriteAsync(d, 0, d.Length);
            stream.WriteAsync(d, 0, d.Length);
        }
        private void GroupsMembers(string from, NetworkStream stream)
        {
            int userID = GetUserId(from);

            List<Friends> tmp = db.Friends.Where(usr => usr.userID == userID || usr.friendID == userID).ToList();
            List<UserData> friends = new List<UserData>();
            foreach (var item in tmp)
            {
                if (item.userID == userID)
                    friends.Add(new UserData(item.User.nickname, item.User.name, item.User.surname));
                else
                    friends.Add(new UserData(item.User1.nickname, item.User1.name, item.User1.surname));

            }
            Instruction instr = new Instruction(Operation.GroupsMembers, null, null, friends);
            byte[] data = MyObjectConverter.ObjectToByteArray(instr);
            stream.WriteAsync(data, 0, data.Length);
            stream.WriteAsync(data, 0, data.Length);
        }
        private void CreateGroup(Instruction instr, NetworkStream stream)
        {
            Groups g = new Groups();
            g.name = instr.To;

            db.Groups.Add(g);
            db.SaveChanges();

            int id = g.id;
            foreach(var item in instr.Data as List<UserData>)
            {
                GroupMembers member = new GroupMembers();
                member.userID = GetUserId(item.Nickname);
                member.groupID = id;
                db.GroupMembers.Add(member);
            }
            GroupMembers creator = new GroupMembers();
            creator.userID = GetUserId(instr.From);
            creator.groupID = g.id;
            db.GroupMembers.Add(creator);

            db.SaveChangesAsync();
        }
        private void GetMessageFromGroup(int groupID, NetworkStream stream)
        {
            List<GroupMessages> tmp = db.GroupMessages.Where(msg => msg.groupID == groupID).ToList();
            List<MessageData> messages = new List<MessageData>();

            foreach (var item in tmp)
            {
                messages.Add(new MessageData(item.User.nickname, item.User.name, $"{item.User.nickname}:\n{item.message}", item.time));
            }
            Groups g = db.Groups.Where(gr => gr.id == groupID).First();
            Instruction instr = new Instruction(Operation.GetMessagesFromGroup, groupID.ToString(), g.name, messages);
            byte[] data = MyObjectConverter.ObjectToByteArray(instr);
            stream.WriteAsync(data, 0, data.Length);
            stream.WriteAsync(data, 0, data.Length);
        }
        private async Task GroupMessage(Instruction instr)
        {
            GroupMessages msg = new GroupMessages();
            msg.groupID = int.Parse(instr.To);
            msg.fromID = GetUserId(instr.From);
            msg.message = (string)instr.Data;
            msg.time = DateTime.Now;

            db.GroupMessages.Add(msg);
            await db.SaveChangesAsync();

            int id = int.Parse(instr.To);
            int fromId = GetUserId(instr.From);

            Groups group = db.Groups.Where(g => g.id == id).First();
            List<GroupMembers> members = group.GroupMembers.Where(m => m.userID != fromId).ToList();

            //foreach (var item in members)
            for (int i = 0; i < members.Count; i++)
            {
                GroupMembers tmp = members[i];
                if (IsOnline(tmp.User.nickname))
                {
                    GroupData gd = new GroupData(group.id, group.name, (string)instr.Data, DateTime.Now);
                    Instruction sendInstr = new Instruction(Operation.ReceiveGroupMessage, instr.From, null, gd);
                    byte[] data = MyObjectConverter.ObjectToByteArray(sendInstr);

                    NetworkStream s = GetStreamByName(tmp.User.nickname);
                    s.WriteAsync(data, 0, data.Length);
                    s.WriteAsync(data, 0, data.Length);
                }
            }
        }
        private void FriendRequest(string from, string to, NetworkStream stream)
        {
            int fromID = GetUserId(from);
            int toID = GetUserId(to);

            Instruction instr;

            if (fromID == -1 || toID == -1)
            {
                instr = new Instruction(Operation.FriendRequest, "Користувача не знайдено\nОновіть сторінку", null, false);
            }
            else
            {
                var tmp = db.FriendRequest.Where(item => (item.fromID == fromID && item.toID == toID) || (item.fromID == toID && item.toID == fromID)).ToList();
                var tmp2 = db.Friends.Where(item => (item.friendID == fromID && item.userID == toID) || (item.friendID == toID && item.userID == fromID)).ToList();
                if(tmp2.Count > 0)
                {
                    instr = new Instruction(Operation.FriendRequest, "Цей користувач уже є в списку ваших друзів", null, false);
                }
                else if (tmp.Count > 0)
                {
                    instr = new Instruction(Operation.FriendRequest, "Ви або цей користувач вже подали заявку в друзі", null, false);
                }
                else
                {
                    try
                    {
                        FriendRequest request = new FriendRequest()
                        {
                            fromID = GetUserId(from),
                            toID = GetUserId(to)
                        };
                        db.FriendRequest.Add(request);
                        db.SaveChangesAsync();

                        instr = new Instruction(Operation.FriendRequest, "Запит в друзі надіслано", null, true);
                    }
                    catch (Exception e)
                    {
                        instr = new Instruction(Operation.FriendRequest, e.Message, null, false);
                    }
                }
            }
            byte[] data = MyObjectConverter.ObjectToByteArray(instr);
            stream.WriteAsync(data, 0, data.Length);
            stream.WriteAsync(data, 0, data.Length);
        }
        private void GetFriendsRequests(string from, NetworkStream stream)
        {
            int userID = GetUserId(from);

            List<FriendRequest> tmp = db.FriendRequest.Where(usr =>  usr.toID == userID).ToList();
            List<UserData> friends = new List<UserData>();
            foreach (var item in tmp)
                friends.Add(new UserData(item.User.nickname, item.User.name, item.User.surname));
            Instruction instr = new Instruction(Operation.GetFriendRequests, null, null, friends);
            byte[] data = MyObjectConverter.ObjectToByteArray(instr);
            stream.WriteAsync(data, 0, data.Length);
            stream.WriteAsync(data, 0, data.Length);
        }
        private void RequestResult(Instruction instr, NetworkStream stream)
        {
            int fromID = GetUserId(instr.From);
            int toID = GetUserId(instr.To);

            if ((bool)instr.Data)
            {
                db.FriendRequest.RemoveRange
                    (
                    db.FriendRequest.Where(req => (req.fromID == fromID && req.toID == toID) || (req.fromID == toID && req.toID == fromID))
                    );
                Friends friend = new Server.Friends
                {
                    userID = fromID,
                    friendID = toID
                };
                db.Friends.Add(friend);
                db.SaveChangesAsync();
            }
            else
            {
                db.FriendRequest.RemoveRange
                    (
                    db.FriendRequest.Where(req => (req.fromID == fromID && req.toID == toID) || (req.fromID == toID && req.toID == fromID))
                    );
                db.SaveChangesAsync();
            }
        }
        private void GetFile(Instruction instr, NetworkStream stream)
        {
            try
            {
                Messages message = db.Messages.Where(msg => (msg.isFile == true) && (msg.message.EndsWith(instr.From))).First();
                //string p = @"C:\Users\Public\Pictures\Sample Pictures\Hydrangeas.jpg";
                byte[] data = File.ReadAllBytes(message.message);
                //MessageData tmp = new MessageData("none", "none", data, DateTime.MinValue);

                Instruction sendInstr = new Instruction(Operation.GetFile, instr.From, instr.To, data);

                byte[] instrData = MyObjectConverter.ObjectToByteArray(sendInstr);
                stream.Write(instrData, 0, instrData.Length);
                stream.Write(instrData, 0, instrData.Length);

            }
            catch
            {

            }

        }

        //DB GET METHODS
        private int GetUserId(string nickname)
        {
            try
            {
                User user = db.User.Where(usr => usr.nickname == nickname).First();
                return user.id;
            }
            catch (Exception)
            {
                return -1;
            }
        }
        private User GetUserById(int id)
        {
            try
            {
                User user = db.User.Where(usr => usr.id == id).First();
                return user;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
