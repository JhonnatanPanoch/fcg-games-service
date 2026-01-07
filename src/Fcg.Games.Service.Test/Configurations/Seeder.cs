using Fcg.Games.Service.Domain.Entities;
using Fcg.Games.Service.Infra.Contexts;

namespace Fcg.Games.Service.Test.Configurations;

public static class Seeder
{
    private static readonly object _lock = new();

    public static AppDbContext Seed(this AppDbContext db)
    {
        lock (_lock)
        {
            if (!db.Jogos.Any())
            {
                db.Jogos.Add(
                    new JogoEntity()
                    {
                        Id = new Guid("32e60c8c-d5bb-4378-afdd-28c07cc71b91"),
                        Nome = "Jogo 1",
                        Descricao = "Descrição do Jogo 1",
                        Preco = 15M,
                        Ativo = true,
                        Promocoes = null
                    });

                db.Jogos.Add(
                    new JogoEntity()
                    {
                        Id = new Guid("ef29ddbc-350d-4fa3-8a96-96cd828e9b4e"),
                        Nome = "Jogo 2",
                        Descricao = "Descrição do Jogo 2",
                        Preco = 25.5M,
                        Ativo = true,
                        Promocoes = null
                    });

                db.Jogos.Add(
                    new JogoEntity()
                    {
                        Id = new Guid("8fcc98f3-0729-4d9b-9771-0be9a6255410"),
                        Nome = "Jogo 3",
                        Descricao = "Descrição do Jogo 2 - Específico para remoção nos testes automatizados",
                        Preco = 25.5M,
                        Ativo = true,
                        Promocoes = null
                    });

                db.SaveChanges();
            }
        }

        return db;
    }
}
