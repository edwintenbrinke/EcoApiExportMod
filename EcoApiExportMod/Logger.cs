using Eco.Shared.Localization;
using Eco.Shared.Utils;

namespace Eco.Plugins.EcoApiExportMod
{
    public class Logger
    {
        public static bool debug = false;

        public static void Debug(string message, string file_location = null)
        {
            if (!debug) return;
            Log.Write(new LocString("EcoApiExportMod DEBUG:" + message + "\n"));
        }

        public static void DebugVerbose(string message, string file_location = null)
        {
            Log.Write(new LocString("EcoApiExportMod DEBUG:" + message + "\n"));
        }

        public static void Info(string message, string file_location = null)
        {

            Log.Write(new LocString("EcoApiExportMod: " + message + "\n"));
        }

        public static void Error(string message, string file_location = null)
        {
            Log.Write(new LocString("EcoApiExportMod ERROR: " + message + "\n"));
        }
    }
}