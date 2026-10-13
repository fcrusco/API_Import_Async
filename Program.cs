// Importa o namespace que contém a configuração de acesso ao banco de dados (AppDbContext).
using ImportadorOracleEF.Data;
// Importa o namespace que contém a classe com as regras de negócio da importação (ClienteImportService).
using ImportadorOracleEF.Services;
// Importa o namespace que contém o mapeamento das rotas da nossa API (ClienteEndpoints).
using ImportadorOracleEF.Endpoints;
// Importa o Entity Framework Core para habilitar o método de conexão com o banco (UseOracle).
using Microsoft.EntityFrameworkCore;

// Inicializa o construtor da aplicação web, preparando o terreno para adicionarmos configurações e serviços.
var builder = WebApplication.CreateBuilder(args);

// Adiciona o serviço que permite à aplicação descobrir quais rotas (endpoints) existem. É essencial para o Swagger.
builder.Services.AddEndpointsApiExplorer();

// Adiciona o serviço gerador do Swagger, que criará a documentação automática da nossa API.
builder.Services.AddSwaggerGen();

// Acessa o arquivo appsettings.json de forma segura e captura a string de conexão armazenada na chave "OracleDb".
var connectionString = builder.Configuration.GetConnectionString("OracleDb");

// Registra o contexto de banco de dados (AppDbContext) na aplicação, dizendo para ele usar o provedor do Oracle e a string capturada.
builder.Services.AddDbContext<AppDbContext>(options => options.UseOracle(connectionString));

// Registra o serviço ClienteImportService no injetor de dependências da API. 
// O padrão AddScoped garante que o serviço será instanciado uma vez por cada requisição HTTP recebida.
builder.Services.AddScoped<ClienteImportService>();

// Finaliza a configuração e constrói a aplicação de fato, deixando-a pronta para rodar.
var app = builder.Build();

// Verifica se o ambiente em que a API está rodando é o de Desenvolvimento (como o seu ambiente local no VS Code).
if (app.Environment.IsDevelopment())
{
    // Habilita a geração do arquivo de especificação do Swagger (um arquivo JSON descrevendo a API).
    app.UseSwagger();
    // Habilita a interface visual e interativa do Swagger para que possamos testar pelo navegador.
    app.UseSwaggerUI();
}

// Chama o método de extensão que criamos na pasta Endpoints para "pendurar" as rotas /api/clientes na aplicação.
app.MapClienteEndpoints();

// Inicia o servidor da API, mantendo-a em execução e aguardando as requisições dos usuários.
app.Run();