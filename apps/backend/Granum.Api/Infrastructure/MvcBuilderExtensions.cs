using Newtonsoft.Json;

namespace Granum.Api.Infrastructure;

public static class MvcBuilderExtensions
{
    public static IMvcBuilder AddCustomJsonConfiguration(this IMvcBuilder builder)
    {
        return builder.AddNewtonsoftJson(options =>
        {
            // Reject unknown properties
            options.SerializerSettings.MissingMemberHandling =
                MissingMemberHandling.Error;
        });
    }

}