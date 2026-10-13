// Importa o contexto do banco de dados para realizar as operações de persistência.
using ImportadorOracleEF.Data;
// Importa o modelo (entidade) Cliente para podermos instanciar os dados lidos.
using ImportadorOracleEF.Models;

// Define o endereço lógico desta classe no projeto (camada de serviços/regras de negócio).
namespace ImportadorOracleEF.Services;

// Classe responsável por isolar a regra de negócio de processamento e importação do arquivo de clientes.
public class ClienteImportService
{
    // Variável privada e somente leitura para armazenar a conexão ativa com o banco de dados.
    private readonly AppDbContext _db;

    // Construtor que recebe o contexto do banco de dados via Injeção de Dependência.
    public ClienteImportService(AppDbContext db)
    {
        // Atribui o contexto injetado à variável local da classe.
        _db = db;
    }

    // Método assíncrono que recebe o fluxo de dados (Stream) do arquivo e retorna uma tupla com a quantidade de sucessos e uma mensagem.
    public async Task<(int Sucesso, string Mensagem)> ImportarAsync(Stream stream)
    {
        // Cria uma lista vazia na memória para agrupar todos os clientes lidos antes de ir ao banco (inserção em lote).
        var clientesParaInserir = new List<Cliente>();
        
        // Inicia o leitor do arquivo a partir do stream. O 'using' garante a liberação da memória (Dispose) ao final da execução.
        using var reader = new StreamReader(stream);

        // Lê o arquivo linha por linha de forma assíncrona até atingir o final (evita estourar a memória com arquivos muito grandes).
        while (await reader.ReadLineAsync() is string linha)
        {
            // Ignora a iteração atual se a linha estiver completamente vazia ou contiver apenas espaços em branco.
            if (string.IsNullOrWhiteSpace(linha)) continue;

            // Divide a linha lida em pequenos pedaços (array) usando o ponto e vírgula (;) como delimitador.
            var dados = linha.Split(';');
            
            // Verifica se a linha tem pelo menos as 3 informações esperadas (Nome, Sobrenome, Email).
            if (dados.Length >= 3)
            {
                // Cria um novo objeto Cliente, preenche os dados e o adiciona na nossa lista de memória.
                clientesParaInserir.Add(new Cliente
                {
                    // Pega o primeiro item (índice 0), remove espaços nas bordas (Trim) e atribui ao Nome.
                    Nome = dados[0].Trim(),
                    
                    // Pega o segundo item (índice 1), limpa espaços e atribui ao Sobrenome.
                    Sobrenome = dados[1].Trim(),
                    
                    // Pega o terceiro item (índice 2), limpa espaços e atribui ao Email.
                    Email = dados[2].Trim()
                });
            }
        }

        // Após ler o arquivo inteiro, verifica se a lista tem ao menos um cliente válido para ser salvo.
        if (clientesParaInserir.Any())
        {
            // Adiciona a lista completa ao rastreador do EF Core (prepara os comandos INSERT otimizados para múltiplos registros).
            await _db.Clientes.AddRangeAsync(clientesParaInserir);
            
            // Vai até o banco de dados Oracle Cloud e executa a gravação física (Commit).
            await _db.SaveChangesAsync();
            
            // Retorna o total de clientes inseridos com sucesso e uma mensagem de feedback.
            return (clientesParaInserir.Count, "Importação concluída com sucesso no Oracle Cloud!");
        }

        // Se o código chegar aqui, significa que o arquivo estava vazio ou sem nenhuma linha formatada corretamente. Retorna zero e um aviso.
        return (0, "Nenhum dado válido encontrado no arquivo.");
    }
}