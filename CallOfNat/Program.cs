using System;
using System.Linq;
using System.Threading;
using CallOfNat.Interfaces;
using CommandLine;
using Mono.Nat;
using CallOfNat.Game.TestGame;
using CallOfNat.Utils;
using System.Security.Policy;
using System.Reflection;

namespace CallOfNat
{
    internal class Options
    {
        [Option('r', "remove", Required = false, HelpText = "Set to remove ports")]
        public bool RemoveMode { get; set; }

        [Option('L', "listports", Required = false, HelpText = "Lists all actives ports")]
        public bool ListPorts { get; set; }

        [Option('l', "listgames", Required = false, HelpText = "Lists all games")]
        public bool ListGames { get; set; }

        [Option('i', "ip", Required = false, Default = "")]
        public string IpAddress { get; set; }

        [Value(0, Default = "")]
        public string Game { get; set; }

    }

    enum Mode
    {
        REMOVE,
        ADD,
        LISTGAME,
        LISTPORT
    }

    class MainClass
    {
        public static bool RemoveMode = false;
        public static Mode Mode = Mode.ADD;
        public static string GameName = "TestGame";
        public static void Main(string[] args)
        {
            Console.WriteLine("CallOfNat");
            Parser.Default.ParseArguments<Options>(args)
            .WithParsed<Options>(o =>
            { 
                if (!o.Game.Equals(""))
                {
                    GameName = o.Game;
                }

                if (o.RemoveMode)
                {
                    Mode = Mode.REMOVE;
                }
                if (o.ListPorts)
                {
                    Mode = Mode.LISTPORT;
                }
                if (o.ListGames)
                {
                    Mode = Mode.LISTGAME;

                    //Liste des jeux possibles
                    string path = System.Reflection.Assembly.GetExecutingAssembly().Location;
                    var directory = System.IO.Path.GetDirectoryName(path);
                    var gamesDirectory = System.IO.Path.Combine(directory, "Games");
                    string[] pluginFiles = System.IO.Directory.GetFiles(gamesDirectory, "*.dll");
                    foreach (string file in pluginFiles)
                    {
                        var asm = System.Reflection.Assembly.LoadFile(file);
                        try
                        {
                            if (!AssemblyLoader.ValidateAssembly(asm))
                                continue;
                            if (asm.GetName().Name.StartsWith("CallOfNat.Game."))
                            {
                                Console.WriteLine(asm.GetName().Name.Replace("CallOfNat.Game.", ""));
                            }
                        } catch (InvalidOperationException)
                        {

                        }
                        
                    }

                    Environment.Exit(0);
                }

                // Raised whenever a device is discovered.
                NatUtility.DeviceFound += DeviceFound;

                // If you know the gateway address, you can directly search for a device at that IP
                if (!o.IpAddress.Equals(""))
                {
                    NatUtility.Search(System.Net.IPAddress.Parse(o.IpAddress), NatProtocol.Pmp);
                    NatUtility.Search(System.Net.IPAddress.Parse(o.IpAddress), NatProtocol.Upnp);
                }
                //On démarre
                NatUtility.StartDiscovery();

                Console.WriteLine("Discovery started");

                while (true)
                {
                    //Toutes les 500 secondes on refait tourner le script au cas où les règles ne sont plus là
                    Thread.Sleep(500000);
                    NatUtility.StopDiscovery();
                    NatUtility.StartDiscovery();
                }
            });
            
        }


        private static void DeviceFound(object sender, DeviceEventArgs args)
        {
            INatDevice device = args.Device;
            IGamePortMapping gamePortMapping;

            switch (Mode)
            {
                case Mode.ADD:
                case Mode.REMOVE:
                    if (GameName == "TestGame")
                    {
                        //Tweak pour load l'assembly
                        gamePortMapping = new TestGame();
                    }
                    else
                    {
                        gamePortMapping = (IGamePortMapping)AssemblyLoader.GetInstanceOfGameMapping(GameName);
                    }

                    if (gamePortMapping == null)
                    {
                        Console.WriteLine("Le jeu " + GameName + " n'a pas été trouvé, veuillez réessayer.");
                        System.Environment.Exit(1);
                    }

                    Console.WriteLine(gamePortMapping.Description());

                    if (Mode == Mode.REMOVE)
                    {
                        Console.WriteLine("Le système va tenter de supprimer ces ports de la table NAT");
                    }
                    else
                    {
                        Console.WriteLine("Le système va tenter d'ajouter ces ports de la table NAT");
                    }

                    Console.WriteLine("Appliquer ? [O]/N");
                    String res = Console.ReadLine();

                    if (res.ToLower().Equals("n"))
                    {
                        System.Environment.Exit(0);
                    }

                    GamePortMappingApplier applier = new GamePortMappingApplier(gamePortMapping);

                    if (Mode == Mode.ADD)
                    {
                        applier.AddRules(device);
                        Console.WriteLine("Les ports sont bien ajoutés, le système reste ouvert et va se relancer toutes les 500 secondes");
                        Console.WriteLine("CTRL+C pour quitter");
                    }
                    else
                    {
                        applier.RemoveRules(device);
                        Environment.Exit(0);
                    }
                    break;
                case Mode.LISTPORT:
                    var routerTable = device.GetAllMappings();
                    foreach (Mapping m in routerTable){
                        Console.WriteLine(m.ToString());
                    }
                    Environment.Exit(0);
                    break;


                default:
                    break;
            }

           
        }
    }
}
