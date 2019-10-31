using System.Configuration;

namespace Demo
{
    public static class Config
    {
        public static string ConnectionString
        {
            get
            {
                return ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString;
            }
        }
    }
}