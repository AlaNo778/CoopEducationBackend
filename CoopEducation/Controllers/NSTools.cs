using System.Globalization;

namespace CoopEducation.Controllers
{
    public static class NSTools
    {
        public static class ConfigurationHelper
        {
            private static IConfiguration? Config;
            public static void Initialize(IConfiguration Configuration)
            {
                Config = Configuration;
            }
            public static IConfiguration? config => Config;
        }
        public static string GetAppConfig(string varname)
        {
            try
            {
                var config = ConfigurationHelper.config;
                if (config != null)
                    return config.GetSection(varname).Value ?? string.Empty;
                return string.Empty;
            }
            catch
            {
                return "";
            }
        }
        public static bool IsNumeric(Object objValue)
        {
            if (objValue != null)
            {
                return double.TryParse(objValue.ToString(), NumberStyles.Any, null, out _);
            }
            else { return false; }
        }
    }
}
