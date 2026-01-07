using Fcg.Games.Service.Application.Interfaces;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Fcg.Games.Service.Application.AppServices;
public class UsuarioAutenticadoAppService : IUsuarioAutenticadoAppService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UsuarioAutenticadoAppService(
        IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public Guid ObterIdUsuario()
    {
        var user = _httpContextAccessor.HttpContext?.User;
        if (user == null || !Guid.TryParse(user.FindFirst(ClaimTypes.NameIdentifier)?.Value, out Guid userId))
            throw new ApplicationException();

            return userId;
    }
}
