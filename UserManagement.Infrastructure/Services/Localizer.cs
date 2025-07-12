using Microsoft.Extensions.Localization;
using UserManagement.Domain.Interfaces;

namespace UserManagement.Infrastructure.Services;
public class Localizer : ILocalizer
{
    private readonly IStringLocalizerFactory _factory;
    public Localizer(IStringLocalizerFactory factory)
    {
        _factory = factory;
    }
    public string this[string key] => GetString(key);

    public string GetString(string key)
    {
        // var assemplyName = new AssemblyName(GetType().Assembly.FullName!);
        var localizer = _factory.Create("Resource", "UserManagement.API"!);
        return localizer[key];
    }
}
