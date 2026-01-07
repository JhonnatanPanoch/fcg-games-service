using Fcg.Games.Service.Domain.Entities;
using Fcg.Games.Service.Infra.Elastic.Clients.Jogo.Interfaces;
using Microsoft.Extensions.Logging;
using OpenSearch.Client;

namespace Fcg.Games.Service.Infra.Elastic.Clients.Jogo;
public class JogoElastic: IJogoElastic
{
    private readonly IOpenSearchClient _elasticClient;
    private readonly ILogger<JogoElastic> _logger;

    public JogoElastic(
        IOpenSearchClient elasticClient,
        ILogger<JogoElastic> logger)
    {
        _elasticClient = elasticClient;
        _logger = logger;
    }

    public async Task IndexarJogoAsync(JogoEntity jogoEntity)
    {
        var response = await _elasticClient.IndexDocumentAsync(jogoEntity);

        if (!response.IsValid)
        {
            _logger.LogError("Falha ao indexar jogo {JogoId}: {DebugInfo}", jogoEntity.Id, response.DebugInformation);
            throw new ApplicationException("Falha ao indexar o jogo no Elasticsearch.");
        }
    }

    public async Task<IReadOnlyCollection<JogoEntity>> BuscarJogosAsync(string termoDeBusca)
    {
        var response = await _elasticClient.SearchAsync<JogoEntity>(s => s
            .Query(q => q
                .MultiMatch(mm => mm
                    .Query(termoDeBusca)
                    .Fields(f => f
                        .Field(p => p.Nome, boost: 3)
                        .Field(p => p.Descricao)
                    )
                    .Fuzziness(Fuzziness.Auto)
                )
            )
        );

        if (!response.IsValid)
        {
            _logger.LogError("Falha na busca no Elasticsearch: {DebugInfo}", response.DebugInformation);

            throw new ApplicationException("Falha ao buscar jogos no Elasticsearch.");
        }

        return response.Documents;
    }

    public async Task<IReadOnlyCollection<JogoEntity>> BuscarSugestaodeJogosAsync(List<Guid> jogosPossuidosIds)
    {
        var response = await _elasticClient.SearchAsync<JogoEntity>(s => s
            .Query(q => q
                .Bool(b => b 
                    .Should(sh => sh 
                        .MoreLikeThis(mlt => mlt
                            .Fields(f => f.Field(ff => ff.Nome).Field(ff => ff.Descricao)) 
                            .Like(l => l
                                .Document(d => d
                                    .Index("jogos")
                                    .Id(jogosPossuidosIds.First()) 
                                )
                            )
                            .MinTermFrequency(1) 
                            .MinDocumentFrequency(1) 
                        )
                    )
                    .MustNot(mn => mn
                        .Ids(i => i.Values(jogosPossuidosIds))
                    )
                )
            ).Size(5));

        return response.Documents;
    }
}
