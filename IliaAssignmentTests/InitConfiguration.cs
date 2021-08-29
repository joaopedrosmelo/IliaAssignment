using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

interface IliaAssignmentTestsInterface
{
    public static IConfiguration InitConfiguration()
    {
        var config = new ConfigurationBuilder()
            .AddJsonFile("appsettings.Tests.json")
            .Build();
        return config;
    }
}
