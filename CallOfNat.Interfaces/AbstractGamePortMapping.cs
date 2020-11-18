using System;
using System.Collections.Generic;
using System.Linq;

namespace CallOfNat.Interfaces
{
    public class AbstractGamePortMapping : IGamePortMapping
    {
        public AbstractGamePortMapping()
        {
        }

        public ICollection<int> TcpPorts { get; set; }
        public ICollection<int> UdpPorts { get; set; }

        public string Description()
        {
            string res = "Description: " + System.Environment.NewLine;

            res = "Game Class Name : " + this.GetType().Name + System.Environment.NewLine;
            res += "Tcp ports : ";
            foreach(int port in TcpPorts)
            {
                res += port.ToString();
                if(port != TcpPorts.Last())
                {
                    res += ", ";
                }
            }
            res += System.Environment.NewLine + "Udp Ports : ";
            foreach (int port in UdpPorts)
            {
                res += port.ToString();
                if (port != UdpPorts.Last())
                {
                    res += ", ";
                }
            }

            return res;
        }
    }
}
