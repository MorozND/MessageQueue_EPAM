using Microsoft.Extensions.Configuration;

namespace SharedAssembly
{
    public class LocalConfigurationBuilder
    {
        public IConfiguration BuildJson(string fileName)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile(fileName);

            return configuration.Build();
        }
    }
}
