using System;
using System.Collections.Generic;

namespace CallOfNat.Interfaces
{
    public interface IGamePortMapping
    {
        ICollection<int> TcpPorts { get; set; }
        ICollection<int> UdpPorts { get; set; }

        string Description();
    }
}
