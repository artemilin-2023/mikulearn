using HackBack.Infrastructure.Data.Configurations.Configurator;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace HackBack.Infrastructure.Data.Contexts
{
    internal class DataContextConfigurator(IConfiguration configuration, ILoggerFactory loggerFactory) :
        BaseDbContextConfigurator<DataContext>(configuration, loggerFactory)
    {
        protected override string ConnectionStringName => "Postgres";
    }
}
