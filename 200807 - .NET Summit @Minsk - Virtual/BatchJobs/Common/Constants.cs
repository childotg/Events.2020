using System;

namespace Common
{
    public static class Constants
    {
        public static string ServiceBusConnection = "";
        public static string TodoQueueName = "todo";
        public static string ResultsQueueName = "result";
        public static string AuthorityUri = "https://login.microsoftonline.com/[tenant]";
        public static string BatchResourceUri = "https://batch.core.windows.net/";
        public static string BatchAccountUrl = "https://[account].northeurope.batch.azure.com";
        public static string ClientId = "";
        public static string ClientKey = "";
        public static string PoolName = "Pool01";
    }
}
