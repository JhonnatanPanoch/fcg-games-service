# FCG Games Service

Microsserviço responsável pelo gerenciamento e consulta do catálogo de jogos.

## Sobre o projeto
Este serviço foi construído visando alta performance de leitura. Utilizamos .NET 9 e aplicamos o padrão de CQRS simplificado, onde as leituras mais pesadas podem ser direcionadas ao Elasticsearch para evitar gargalos no banco relacional (PostgreSQL).

## Infraestrutura e Escalabilidade
Para atender aos requisitos de escalabilidade do Tech Challenge:
- **Docker**: Imagens otimizadas com multistage build (baseada em Alpine/Linux reduzido).
- **Kubernetes**: Configuramos HPA (Horizontal Pod Autoscaler) monitorando o uso de CPU. Se a carga subir acima de 70%, o cluster sobe novas réplicas automaticamente.
- **Observabilidade**: Logs estruturados com Serilog enviados para o New Relic.

## Endpoints e Permissões

| Recurso | Método | Rota | Permissão | Descrição |
| :--- | :--- | :--- | :--- | :--- |
| **Jogos** | `GET` | `/api/v1/jogos` | Autenticado | Lista todos os jogos |
| | `GET` | `/api/v1/jogos/{id}` | Autenticado | Detalhes de um jogo |
| | `GET` | `/api/v1/jogos/search` | Autenticado | Busca por termo (query param) |
| | `POST` | `/api/v1/jogos` | **Admin** | Cadastra novo jogo |
| | `PUT` | `/api/v1/jogos/{id}` | **Admin** | Atualiza jogo |
| | `DELETE` | `/api/v1/jogos/{id}` | **Admin** | Remove jogo |
| **Promoções** | `GET` | `/api/v1/promocoes` | Autenticado | Lista promoções ativas |
| | `POST` | `/api/v1/promocoes` | **Admin** | Cria promoção |
| | `DELETE` | `/api/v1/promocoes/{id}` | **Admin** | Remove promoção |
| **Sugestões** | `GET` | `/api/v1/sugestoes` | Autenticado | Jogos baseados no perfil do usuário |
| **Métricas** | `GET` | `/api/v1/metricas/mais-populares` | Autenticado | Top jogos mais acessados |

## Como executar
O projeto depende de um banco PostgreSQL e do Elasticsearch.
As configurações de conexão ficam no `appsettings.json` ou podem ser sobrescritas via variáveis de ambiente no container (ConfigMaps).

Para subir via Docker Compose (na raiz da solução):
`docker-compose up -d`