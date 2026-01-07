using Fcg.Games.Service.Domain.Entities;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Fcg.Games.Service.Infra.Contexts;
public static class MigrationExtension
{
    public static async Task<WebApplication> ApplyMigrationsWithSeedsAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        if (!dbContext.Database.IsRelational())
            return app;

        dbContext.Database.Migrate();

        if (dbContext.Jogos?.Count() == 0)
        {
            var jogoId = Guid.NewGuid();
            var promocaoId = Guid.NewGuid();

            // Jogo
            await dbContext.Jogos.AddAsync(new JogoEntity
            {
                Id = jogoId,
                Nome = "Elden Ring",
                Descricao = "Jogo de ação e aventura em mundo aberto",
                Preco = 299.90m,
                Ativo = true
            });

            await dbContext.Jogos.AddAsync(new JogoEntity
            {
                Id = Guid.NewGuid(),
                Nome = "Minecraft",
                Descricao = "Jogo de ação e aventura de construção sandbox",
                Preco = 155.50m,
                Ativo = true
            });

            // Promoção
            await dbContext.Promocoes.AddAsync(new PromocaoEntity
            {
                Id = promocaoId,
                JogoId = jogoId,
                PrecoPromocional = 199.90m,
                DataInicio = DateTime.UtcNow.AddDays(-5),
                DataFim = DateTime.UtcNow.AddDays(5)
            });

            await dbContext.SaveChangesAsync();
        }

        return app;
    }
}