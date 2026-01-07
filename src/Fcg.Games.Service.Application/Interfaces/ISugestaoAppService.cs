using Fcg.Games.Service.Application.Dtos.Jogo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fcg.Games.Service.Application.Interfaces;
public interface ISugestaoAppService
{
    Task<List<JogoDto>> ObterSugestoesParaUsuarioAsync(Guid usuarioId);
}
