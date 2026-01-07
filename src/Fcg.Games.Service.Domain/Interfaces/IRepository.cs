using Fcg.Games.Service.Domain.Entities;
using System.Linq.Expressions;

namespace Fcg.Games.Service.Domain.Interfaces;

public interface IRepository<T> where T : EntityBase
{
    Task<List<T>> ObterAsync(Expression<Func<T, bool>>? filtro = null);
    Task<T?> ObterPorIdAsync(Guid id);
    Task<T> AdicionarAsync(T entidade);
    Task Atualizar(T entidade);
    Task Remover(T entidade);
    Task<List<T>> ObterPorIdsAsync(List<Guid> ids);
}