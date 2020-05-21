using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;

namespace Server
{
    class OnlineUsers
    {
        public string User { get; private set; }
        public NetworkStream Stream { get; private set; }

        public OnlineUsers(string user, NetworkStream stream)
        {
            User = user;
            Stream = stream;
        }
    }
}
