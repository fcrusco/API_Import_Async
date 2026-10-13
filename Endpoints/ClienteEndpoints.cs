// Importa o contexto de acesso ao banco de dados.
using ImportadorOracleEF.Data;
// Importa o serviço que contém a regra de negócio da importação.
using ImportadorOracleEF.Services;
// Importa as extensões do Entity Framework Core, necessárias para o método ToListAsync().
using Microsoft.EntityFrameworkCore;

// Define o endereço lógico desta classe no projeto (camada de rotas/endpoints).
namespace ImportadorOracleEF.Endpoints;

// Classe estática utilizada para organizar as rotas da API em métodos de extensão.
public static class ClienteEndpoints
{
    // Método de extensão para o WebApplication, permitindo agrupar as rotas fora do Program.cs.
    public static void MapClienteEndpoints(this WebApplication app)
    {
        // Mapeia uma requisição HTTP POST para a rota "/api/clientes/importar". Injeta o arquivo e o serviço automaticamente.
        app.MapPost("/api/clientes/importar", async (IFormFile arquivo, ClienteImportService importService) =>
        {
            // Valida se o arquivo foi enviado na requisição e se contém algum dado. Se falhar, retorna erro HTTP 400.
            if (arquivo is null || arquivo.Length == 0) return Results.BadRequest("Arquivo inválido ou vazio.");
            
            // Abre um canal de leitura (stream) do arquivo em memória para processamento.
            using var stream = arquivo.OpenReadStream();
            
            // Repassa o stream para a camada de serviço processar o texto e inserir no banco. Aguarda a finalização (await).
            var resultado = await importService.ImportarAsync(stream);
            
            // Retorna HTTP 200 (OK) enviando um objeto JSON com a mensagem de retorno e o número de linhas salvas.
            return Results.Ok(new { Mensagem = resultado.Mensagem, Registros = resultado.Sucesso });
        })
        // Desliga a validação de formulários (CSRF), essencial para testar o upload via Swagger ou Postman sem bloqueios.
        .DisableAntiforgery(); 

        // Mapeia uma requisição HTTP GET para a rota "/api/clientes". Injeta a conexão com o banco de dados.
        app.MapGet("/api/clientes", async (AppDbContext db) =>
        {
            // Executa um "SELECT * FROM TB_CLIENTE" de forma assíncrona e carrega os dados em uma lista.
            var clientes = await db.Clientes.ToListAsync();
            
            // Retorna a lista completa convertida automaticamente em JSON com status HTTP 200 (OK).
            return Results.Ok(clientes);
        });
    }
}