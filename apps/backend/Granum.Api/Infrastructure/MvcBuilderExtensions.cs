using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Granum.Api.Infrastructure;

public static class MvcBuilderExtensions
{
    public static JsonSerializerSettings GetJsonSerializerSettings()
    {
        return new JsonSerializerSettings
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver(),
            MissingMemberHandling = MissingMemberHandling.Error
        };
    }

    public static IMvcBuilder AddCustomJsonConfiguration(this IMvcBuilder builder)
    {
        var settings = GetJsonSerializerSettings();
        return builder.AddNewtonsoftJson(options =>
        {
            options.SerializerSettings.ContractResolver = settings.ContractResolver;
            options.SerializerSettings.MissingMemberHandling = settings.MissingMemberHandling;
        });
    }
}
