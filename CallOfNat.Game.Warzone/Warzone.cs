using System;
using System.Collections.Generic;
using CallOfNat.Interfaces;

namespace CallOfNat.Game.Warzone
{
    public class Warzone : AbstractGamePortMapping
    {
        public Warzone()
        {
            TcpPorts = new List<int>();
            UdpPorts = new List<int>();

            TcpPorts.Add(3074);
            for (int i = 27014; i < 27050; i++)
            {
                TcpPorts.Add(i);
            }


            //Warzone UDP
            UdpPorts.Add(3074);
            UdpPorts.Add(3478);
            UdpPorts.Add(4380);
            UdpPorts.Add(4379);
            UdpPorts.Add(27036);
            for (int i = 27000; i < 27031; i++)
            {
                UdpPorts.Add(i);
            }
        }
    }
}
