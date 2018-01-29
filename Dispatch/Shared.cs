using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dispatch
{
    internal class Shared
    {
        public static Parser _Parser { get; set; }

        public static string UserName { get; set; }

        public static string _ApiUrl { get; set; }

        public static void InitApiSet()
        {
            Logger.Log.Info("Reading apiset");
            //var apiset= Logger._AppDir + "\\apiset";
            //if (!File.Exists(apiset))
            //    return null;
            if (!File.Exists("apiset.txt"))
            {
                Logger.Log.Error("apiset is missing");
                //MessageBox.Show("apiset file is missing", "Error", MessageBoxButton.OK, MessageBoxImage.Error);

                throw new Exception("apiset file is missing");
            }

            var data = File.ReadAllText("apiset.txt");
            if (string.IsNullOrEmpty(data))
            {
                Logger.Log.Error("apiset is corrupted");
                //File.Delete(apiset);
                //MessageBox.Show("apiset file is currpted", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                throw new Exception("apiset file is corrupted");
            }
            _ApiUrl = data;
            _Parser = new Parser(data);
        }

    }
}
