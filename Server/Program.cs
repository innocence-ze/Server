﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    class Program
    {
        static void Main(string[] args)
        {
            NetManager.StartLoop(8888);
        }
        ~Program()
        {
            NetManager.udpReceiveThread?.Abort();
        }
    }
}
