using Microsoft.Extensions.Configuration;

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
