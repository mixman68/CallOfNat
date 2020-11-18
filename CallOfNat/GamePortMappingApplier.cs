using System;
using System.Linq;
using CallOfNat.Interfaces;
using Mono.Nat;

namespace CallOfNat
{
    public class GamePortMappingApplier
    {
        IGamePortMapping GamePortMapping { get; set; }

        public GamePortMappingApplier(IGamePortMapping gamePortMapping)
        {
            GamePortMapping = gamePortMapping;
        }

        public void AddRules(INatDevice device)
        {
            foreach(int port in GamePortMapping.TcpPorts)
            {
                AddPortMapping(device, new Mapping(Protocol.Tcp, port, port, 0, "CallofNat"));
            }
            foreach (int port in GamePortMapping.UdpPorts)
            {
                AddPortMapping(device, new Mapping(Protocol.Udp, port, port, 0, "CallofNat"));
            }
        }

        public void RemoveRules(INatDevice device)
        {
            foreach (int port in GamePortMapping.TcpPorts)
            {
                RemovePortMapping(device, new Mapping(Protocol.Tcp, port, port, 0, "CallofNat"));
            }
            foreach (int port in GamePortMapping.UdpPorts)
            {
                RemovePortMapping(device, new Mapping(Protocol.Udp, port, port, 0, "CallofNat"));
            }
        }

        private static void AddPortMapping(INatDevice device, Mapping m)
        {
            Console.WriteLine("Ajout du port " + m.ToString());
            if (GetMappingFromDevice(device, m) != null)
            {
                Console.WriteLine("Le port " + m.ToString() + "existe déjà");
            } else
            {
                device.CreatePortMap(m);
            }
        }

        private static void RemovePortMapping(INatDevice device, Mapping m)
        {
            Console.WriteLine("Suppression du port " + m.ToString());
            var routerMapping = GetMappingFromDevice(device, m);
            if (routerMapping == null)
            {
                Console.WriteLine("Le port " + m.ToString() + " n'existe pas");
            }
            else
            {
                device.DeletePortMap(routerMapping);
            }
        }

        private static Mapping GetMappingFromDevice(INatDevice device, Mapping mapping)
        {
            //Récupere la liste actuelle
            var routerTableMapping = device.GetAllMappings();

            //On check si le port y est
            return routerTableMapping.Where(m => m.Protocol == mapping.Protocol
                                                && m.PrivatePort == mapping.PrivatePort
                                                && m.PublicPort == mapping.PublicPort)
                .FirstOrDefault();
        }
    }
}
