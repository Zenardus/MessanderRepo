using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

namespace ChatInstruction
{
    public enum MessageType { File, Photo, TextMessage }
    //[Serializable]
    public enum Operation
    {
        PersonalMessage, //+
        GroupMessage,    //+
        GetMessagesFromGroup, //+
        ReceiveGroupMessage, //+
        GetMessages,     //+
        GetMessagesFrom, //+
        Friends,       //<<<<<
        AddFriend,     //<<<<<
        DeleteFriend,  //<<<<<
        FriendRequest, //<<<<<
        Groups,        //+
        CreateGroup,   //+
        GroupsMembers, //+
        Login,         //+
        Register,      //+
        AddOnline,     //+
        SearchUser,     //+
        GetFriendRequests,
        RequestResult,
        GetFile
    }
    //OPERATIONS:
    //        SERVER RECEIVE                               CLIENT RECEIVE
    //--------------------------------Login
    //from - username : string                          from - errorText : string  OR null  [if data is false]
    //to   - null                                       to   - null
    //data - password : string                          data - loginResult : bool

    //--------------------------------Register
    //from - null                                       from - errorText : string  OR null  [if data is false]
    //to   - null                                       to   - null
    //data - user : RegistrationData                    data - registerResult : bool

    //--------------------------------AddOnline
    //from - username : string
    //to   - null                                           null
    //data - null

    //--------------------------------SearchUser
    //from - login : string                             from - null
    //to   - null                                       to   - null
    //data - user : string                              data - userList : List<UserData>

    //--------------------------------AddFriend
    //from - user : string                              from - message : string
    //to   - user : string                              to   - null
    //data - null                                       data - result : bool

    //--------------------------------Friends
    //from - user : string                              from - null
    //to   - null                                       to   - null
    //data - null                                       data - friendList : List<UserData>

    //--------------------------------DeleteFriend
    //from - user : string
    //to   - user : string                                  null
    //data - null

    //--------------------------------AddOnline
    //from - user : string
    //to   - null                                           null
    //data - null

    //--------------------------------GetMessages
    //from - user : string                               from - null
    //to   - null                                        to   - null
    //data - null                                        data - messagesList : List<MessageData>

    //--------------------------------GetMessagesFrom
    //from - userSender : string                         from - userReceiver
    //to   - userReceiver : string                       to   - null
    //data - null                                        data - messagesList : List<MessageData>

    //--------------------------------PersonalMessage
    //from - user : string                               from - null
    //to   - user : string                               to   - user : string
    //data - message : string                            data - message : MessageData

    //--------------------------------Groups
    //from - user : string                               from - null
    //to   - null                                        to   - null
    //data - null                                        data - groups : List<GroupData>

    //--------------------------------GroupsMembers
    //from - user : string                               from - null
    //to   - null                                        to - null
    //data - null                                        data - memberList : List<UserData>

    //--------------------------------CreateGroup
    //from - user : string
    //to   - groupName : string                             null
    //data - userList : List<UserData>

    //--------------------------------GetMessagesFromGroup
    //from - null                                        from - groupID : int
    //to   - null                                        to   - groupName : string
    //data - groupID : int                               data - messages : List<MessageData>

    //--------------------------------GroupMessage
    //from - username : string
    //to   - grouID : string
    //data - message : string 

    [Serializable]
    public class RegistrationData
    {
        public string Login { get; private set; }
        public string Password { get; private set; }
        public string Name { get; private set; }
        public string Surname { get; private set; }

        public RegistrationData(string login, string password, string name, string surname)
        {
            Login = login;
            Password = password;
            Name = name;
            Surname = surname;
        }
    }
    [Serializable]
    public class UserData
    {
        public string Nickname { get; private set; }
        public string Name { get; private set; }
        public string Surname { get; private set; }

        public UserData(string nickname, string name, string surname)
        {
            Nickname = nickname;
            Name = name;
            Surname = surname;
        }
        public override string ToString()
        {
            return $"{Name} {Surname}\n{Nickname}";
        }

    }
    [Serializable]
    public class MessageData
    {
        public string Nickname { get; private set; }
        public string Name { get; private set; }
        public object Message { get; private set; }
        public DateTime Time { get; private set; }
        public MessageType Type { get; private set; }


        public MessageData(string nickname, string name, object message, DateTime time)
        {
            Nickname = nickname;
            Name = name;
            Message = message;
            Time = time;
            Type = MessageType.TextMessage;
        }
        public MessageData(string nickname, string name, object message, DateTime time, MessageType type)
        {
            Nickname = nickname;
            Name = name;
            Message = message;
            Time = time;
            Type = type;
        }


        public override string ToString()
        {
            return $"{Name} [{Nickname}]\n{Message}\n{Time}";
        }
    }
    [Serializable]
    public class GroupMemberData
    {
        public int ID { get; private set; }
        public string Name { get; private set; }
        public string Surname { get; private set; }
        public string Nickname { get; private set; }

        public GroupMemberData(int id, string name, string surname, string nickname)
        {
            ID = id;
            Name = name;
            Surname = surname;
            Nickname = nickname;
        }
        public override string ToString()
        {
            return $"{Surname} {Name} [{Nickname}]";
        }
    }
    [Serializable]
    public class GroupData
    {
        public int ID { get; private set; }
        public string Name { get; private set; }
        public string LastMessage { get; private set; }
        public DateTime Time { get; private set; }


        public GroupData(int id, string name, string lastMessage, DateTime time)
        {
            ID = id;
            Name = name;
            LastMessage = lastMessage;
            Time = time;
        }

        public override string ToString()
        {
            return $"{Name}\n{LastMessage}\n{Time}";
        }
    }


    [Serializable]
    public class Instruction
    {
        public Operation Operation { get; private set; }
        public string From { get; private set; }
        public string To { get; private set; }
        public object Data { get; private set; }

        public Instruction(Operation operation, string from, string to, object data)
        {
            Operation = operation;
            From = from;
            To = to;
            Data = data;
        }
        public override string ToString()
        {
            return $"{Operation}  {From}  {To}  {Data.ToString()}";
        }
    }



    //--------------------BYTE CONVERTING-------------
    public static class MyObjectConverter
    {
        public static byte[] ObjectToByteArray(object obj)
        {
            if (obj == null)
                return null;
            BinaryFormatter bf = new BinaryFormatter();
            using (MemoryStream ms = new MemoryStream())
            {
                bf.Serialize(ms, obj);
                return ms.ToArray();
            }
        }
        public static object ByteArrayToObject(byte[] arrBytes)
        {

            MemoryStream memStream = new MemoryStream();
            BinaryFormatter binForm = new BinaryFormatter();
            memStream.Write(arrBytes, 0, arrBytes.Length);
            memStream.Seek(0, SeekOrigin.Begin);
            object obj = binForm.Deserialize(memStream);
            return obj;

        }
    }
   
}
