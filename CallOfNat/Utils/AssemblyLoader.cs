using System;
using System.Linq;
using System.Reflection;
using System.Security.Permissions;
using System.Security.Policy;

namespace CallOfNat.Utils
{
    public class AssemblyLoader
    {
        public AssemblyLoader()
        {
        }

        /*public static StrongName GetStrongName(Assembly assembly)
        {
            if (assembly == null)
                throw new ArgumentNullException("assembly");
            AssemblyName assemblyName = assembly.GetName();

            // get the public key blob
            byte[] publicKey = assemblyName.GetPublicKey();
            if (publicKey == null || publicKey.Length == 0)
                throw new InvalidOperationException(String.Format("{0} is not strongly named", assembly));

            StrongNamePublicKeyBlob keyBlob = new StrongNamePublicKeyBlob(publicKey);

            // create the StrongName
            return new StrongName(keyBlob, assemblyName.Name, assemblyName.Version);
        }*/
        public static bool ValidateAssembly(Assembly assembly)
        {
            if (assembly == null)
                throw new ArgumentNullException("assembly");
            AssemblyName assemblyName = assembly.GetName();

            // get the public key blob
            byte[] publicKey = assemblyName.GetPublicKey();
            if (publicKey == null || publicKey.Length == 0)
                throw new InvalidOperationException(String.Format("{0} is not strongly named", assembly));

            //Get current assembly key
            byte[] myPublicKey = Assembly.GetExecutingAssembly().GetName().GetPublicKey();

            if (myPublicKey.SequenceEqual(publicKey))
            {
                return true;
            }

            return false;
        }


        public static object GetInstanceOfGameMapping(string gameName)
        {
            string strFullyQualifiedName = "CallOfNat.Game." + gameName + "." + gameName;
            Type type = Type.GetType(strFullyQualifiedName);
            if (type != null)
                return Activator.CreateInstance(type);
            foreach (var asm in AppDomain.CurrentDomain.GetAssemblies())
            {
                type = asm.GetType(strFullyQualifiedName);
                if (type != null)
                    return Activator.CreateInstance(type);
            }

            //Try to load by Plugin
            String path = System.Reflection.Assembly.GetExecutingAssembly().Location;
            var directory = System.IO.Path.GetDirectoryName(path);
            var gamesDirectory = System.IO.Path.Combine(directory, "Games");
            string[] pluginFiles = System.IO.Directory.GetFiles(gamesDirectory, "*.dll");
            foreach (string file in pluginFiles)
            {
                var asm = System.Reflection.Assembly.LoadFile(file);
                try
                {
                    if (!ValidateAssembly(asm))
                        return null;
                }
                catch (InvalidOperationException)
                {

                }

                type = asm.GetType(strFullyQualifiedName);
                if (type != null)
                    return Activator.CreateInstance(type);
            }
            return null;
        }
    }
}
