﻿using StackExchange.Redis;
using Microsoft.Extensions.Configuration;
using System.IO;
namespace SensitiveWordsAPI.DAL.Utility
{
    static class ConfigurationManager
    {
        public static IConfiguration AppSetting
        {
            get;
        }
        static ConfigurationManager()
        {
            AppSetting = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json").Build();
        }
    }
}
