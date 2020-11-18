using System;
using System.Collections.Generic;
using CallOfNat.Interfaces;

namespace CallOfNat.Game.TestGame
{
    public class TestGame : AbstractGamePortMapping
    {
        public TestGame()
        {
            TcpPorts = new List<int>();
            UdpPorts = new List<int>();

            TcpPorts.Add(12345);
            UdpPorts.Add(12345);
        }
    }
}
