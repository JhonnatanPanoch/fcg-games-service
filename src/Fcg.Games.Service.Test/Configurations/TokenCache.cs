using Fcg.Games.Service.Application.Interfaces;
using Fcg.Games.Service.Domain.Enums;
using Microsoft.Extensions.DependencyInjection;

namespace Fcg.Games.Service.Test.Configurations;

public static class TokenCache
{
    private static string _adminToken;
    private static string _userToken;

    public static string GetAdminToken(IServiceProvider services)
    {
        if (!string.IsNullOrEmpty(_adminToken))
            return _adminToken;

        var jwtService = services.GetRequiredService<IJwtAppService>();

        var token = jwtService.GerarToken("admin@email.com", RoleEnum.Admin.ToString());
        _adminToken = token;
       
        return token;
    }

    public static string GetUserToken(IServiceProvider services)
    {
        if (!string.IsNullOrEmpty(_userToken))
            return _userToken;

        var jwtService = services.GetRequiredService<IJwtAppService>();

        var token = jwtService.GerarToken( "user@email.com", RoleEnum.Usuario.ToString());
        _userToken = token;
        
        return token;
    }
}
