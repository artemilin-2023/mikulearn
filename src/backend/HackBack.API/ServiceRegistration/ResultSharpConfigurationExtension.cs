using ResultSharp.Configuration;
using ResultSharp.Logging.MicrosoftLogger;

namespace HackBack.API.ServiceRegistration;

public static class ResultSharpConfigurationExtension
{
    public static void UseResultSharpLogging(this WebApplication app)
    {
        new ResultConfigurationGlobal().Configure(options =>
        {
            options.EnableLogging = true;

            options.LoggingConfiguration.Configure(config =>
            {
                config.LoggingAdapter = new MicrosoftLoggingAdapter(app.Logger);
            });
        });
    }
}
