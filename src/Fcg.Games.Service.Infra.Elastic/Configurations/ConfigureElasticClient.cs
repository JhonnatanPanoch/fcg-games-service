using Fcg.Games.Service.Infra.Elastic.Clients.Jogo;
using Fcg.Games.Service.Infra.Elastic.Clients.Jogo.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using OpenSearch.Client;

namespace Fcg.Games.Service.Infra.Elastic.Configurations;
public class ConfigureElasticClient
{
    public static void Configure(
        IServiceCollection services,
        string connectionString,
        string user,
        string pass)
    {
        var settings = new ConnectionSettings(new Uri(connectionString))
            .DefaultIndex("jogos")
            .BasicAuthentication(user, pass);

        var client = new OpenSearchClient(settings);

        services.AddSingleton<IOpenSearchClient>(client);
        services.AddTransient<IJogoElastic, JogoElastic>();
    }
}
