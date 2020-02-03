using Eco.Shared.Localization;
using Eco.Shared.Utils;

namespace Eco.Plugins.EcoApiExportMod
{
    public class Logger
    {
        public static void Debug(string message, string file_location = null)
        {
            Log.Write(new LocString("EcoTestMod DEBUG:" + message + "\n"));
            //WriteToFile(message, file_location);
        }

        public static void DebugVerbose(string message, string file_location = null)
        {
            Log.Write(new LocString("EcoTestMod DEBUG:" + message + "\n"));
            //WriteToFile(message, file_location);
        }

        public static void Info(string message, string file_location = null)
        {

            Log.Write(new LocString("EcoTestMod: " + message + "\n"));
            //WriteToFile(message, file_location);
        }

        public static void Error(string message, string file_location = null)
        {
            Log.Write(new LocString("EcoTestMod ERROR: " + message + "\n"));
            //WriteToFile(message, file_location);
        }
    }
}