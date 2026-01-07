using Fcg.Games.Service.Domain.Entities;

namespace Fcg.Games.Service.Infra.Elastic.Clients.Jogo.Interfaces;
public interface IJogoElastic
{
    /// <summary>
    /// Busca de forma assíncrona sugestões de jogos no Elasticsearch com base na biblioteca de um usuário.
    /// </summary>
    /// <remarks>
    /// Este método utiliza uma consulta "More Like This" (MLT) do Elasticsearch para encontrar jogos com conteúdo textual semelhante
    /// aos que o usuário já possui. A lógica de busca considera os seguintes pontos:
    /// <list type="bullet">
    /// <item>
    /// <description>A similaridade é calculada com base nos campos de <c>Nome</c> e <c>Descricao</c> dos jogos.</description>
    /// </item>
    /// <item>
    /// <description>Utiliza o primeiro jogo da lista <paramref name="jogosPossuidosIds"/> como o principal documento de "exemplo" para encontrar outros similares.</description>
    /// </item>
    /// <item>
    /// <description>Todos os jogos cujos IDs estão na lista <paramref name="jogosPossuidosIds"/> são explicitamente excluídos dos resultados para não sugerir algo que o usuário já tem.</description>
    /// </item>
    /// <item>
    /// <description>A consulta retorna no máximo 5 sugestões.</description>
    /// </item>
    /// </list>
    /// </remarks>
    /// <param name="jogosPossuidosIds">Uma lista de <c>Guid</c> contendo os identificadores dos jogos que o usuário já possui. Estes jogos servirão de base para a recomendação e serão excluídos do resultado final.</param>
    /// <returns>
    /// Uma tarefa que representa a operação assíncrona. O resultado da tarefa é uma coleção somente leitura de <c>JogoEntity</c>
    /// com até 5 sugestões de jogos. Retorna uma coleção vazia se nenhuma sugestão for encontrada ou se a lista de entrada for nula/vazia.
    /// </returns>
    Task<IReadOnlyCollection<JogoEntity>> BuscarSugestaodeJogosAsync(List<Guid> jogosPossuidosIds);
    Task<IReadOnlyCollection<JogoEntity>> BuscarJogosAsync(string termoDeBusca);
    Task IndexarJogoAsync(JogoEntity jogoEntity);
}
