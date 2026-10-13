# 🚀 API de Importação de Arquivos TXT para Oracle (.NET 10)

Esta API foi desenvolvida em **C# .NET 10** utilizando **Minimal APIs** e **Entity Framework Core**. O projeto conecta-se a uma instância do **Oracle Cloud** para mapear a tabela existente `TB_CLIENTE` e processa a importação de um arquivo `.txt` delimitado por ponto e vírgula (`;`), salvando os dados de forma assíncrona.

A aplicação adota uma arquitetura em camadas (Models, Data, Services e Endpoints) para garantir separação de responsabilidades e organização do código.


## 📋 Pré-requisitos

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- [Visual Studio Code (VS Code)](https://code.visualstudio.com/) ou IDE da sua preferência.
- Instância de banco de dados **Oracle Cloud** contendo a tabela `TB_CLIENTE` com as colunas: `ID_CLIENTE` (gerada por Sequence/nextval), `NOME`, `SOBRENOME` e `EMAIL`.

## Minimal APIs
Abordagem simplificada e leve para construir APIs HTTP em ASP.NET Core, permitindo a criação de rotas com poucas linhas de código. Elas eliminam a necessidade de código estrutural, dispensando a criação de pastas específicas, classes base herdadas ou atributos de roteamento. A configuração e as rotas são definidas diretamente no arquivo Program.cs por meio de expressões lambda e métodos de extensão (como app.MapGet e app.MapPost).

Foram introduzidas oficialmente no .NET 6, lançado em novembro de 2021.

O ecossistema .NET era frequentemente criticado por ser muito verboso, exigindo muita configuração inicial apenas para colocar um endpoint simples no ar. Enquanto isso, frameworks em outras linguagens atraíam desenvolvedores pela agilidade e simplicidade. A Microsoft lançou as Minimal APIs para tornar o C# competitivo no desenvolvimento ágil de microsserviços, diminuindo a barreira de entrada para iniciantes e oferecendo uma arquitetura com altíssima performance e menor consumo de recursos, já que não carrega toda a pipeline pesada do padrão MVC.

São utilizadas para o desenvolvimento de microsserviços, aplicações nativas em nuvem (cloud-native), funções serverless (como Azure Functions), protótipos ágeis e APIs de propósito único (como um importador de arquivos ou um serviço de envio de e-mails). Elas atendem perfeitamente cenários onde o foco é a velocidade de execução e o baixo uso de memória.

Quando usar Minimal APIs
Microsserviços rápidos e focados: Ideal para serviços independentes que fazem apenas uma coisa específica (como o nosso importador de arquivos TXT). Sem a necessidade de criar várias pastas e arquivos base, você consegue ver o fluxo inteiro do serviço em um único arquivo, o que é perfeito para publicar em contêineres ou funções em nuvem (Serverless).

APIs simples e operações CRUD básicas: Se o seu objetivo é apenas conectar ao banco de dados para criar, ler, atualizar e apagar registros sem lógicas complexas, usar controladores tradicionais gera um excesso de código desnecessário (over-engineering). Com as Minimal APIs, você liga a URL da requisição direto na ação do banco de dados em poucas linhas.

Alta performance e economia financeira em nuvem: Como essa abordagem pula toda a "esteira" pesada do MVC tradicional (ignorando motores de roteamento antigos e formatadores complexos), a API gasta muito menos memória RAM, usa menos CPU. Na nuvem, onde você paga por consumo, isso significa economia direta.

Prototipagem ágil com segurança: Você ganha velocidade de programação, mas sem abrir mão da segurança do C#. O compilador continua avisando sobre erros antes do código rodar, permitindo testar e validar ideias rapidamente sem o estresse de bugs de tipagem.

Quando usar API Controllers (Abordagem Tradicional, desde 2009 - .NET Framework 3.5)
Sistemas grandes e monolíticos: Quando uma API cresce e passa a ter dezenas ou centenas de endpoints, registrar tudo solto no Program.cs se torna um caos insustentável. Os controladores forçam uma organização visual e arquitetural clara: você cria uma classe ClientesController para agrupar tudo de clientes e um PedidosController para pedidos, facilitando a vida de uma equipe com muitos programadores.

Aplicação de regras em lote (Atributos e Filtros): Se você precisa que 20 rotas diferentes exijam que o usuário seja um "Administrador", nos controladores basta colocar uma única etiqueta [Authorize(Roles="Admin")] no topo da classe, e todas as rotas herdam a regra automaticamente. Nas Minimal APIs, você teria que repetir essa exigência rota por rota.

Injeção de dependência inteligente: Em um controlador, você solicita o acesso ao banco de dados ou a um serviço de e-mail apenas uma vez, no topo do arquivo. A partir daí, todos os métodos daquele controlador podem usar o serviço. Nas Minimal APIs, você precisa injetar esses serviços nos parâmetros de cada uma das rotas repetidamente.

Versionamento avançado de API: Se você está criando uma API pública que outras empresas vão consumir e precisa manter versões simultâneas rodando (exemplo: a rota /v1/clientes funciona de um jeito e a /v2/clientes de outro), o ecossistema de controladores possui bibliotecas muito mais maduras e fáceis de configurar para lidar com esse roteamento complexo.

## 🧠 Minimal APIs vs API Controllers

Este projeto utiliza **Minimal APIs**, uma abordagem introduzida em versões recentes do .NET para simplificar o desenvolvimento. Abaixo encontram-se as principais diferenças didáticas entre esta abordagem e os tradicionais **API Controllers**:


| Característica | Minimal APIs (Utilizado neste projeto) | API Controllers (Abordagem Tradicional) |
| :--- | :--- | :--- |
| **Paradigma** | Funcional (baseado em expressões Lambda e métodos de extensão). | Orientado a Objetos (baseado em Classes e Herança). |
| **Complexidade e Código** | Requer muito pouco código (baixo *boilerplate*). Não exige classes base ou atributos de roteamento. | Exige mais código estrutural (herdar de `ControllerBase`, usar atributos como `[ApiController]`, `[Route]`). |
| **Performance e Overhead** | Ligeiramente superior. Possui menos camadas de abstração na pipeline, consumindo menos memória. | Ciclo de vida mais pesado, pois passa por toda a estrutura do ASP.NET Core MVC (filtros de ação, formatadores complexos). |
| **Organização** | As rotas são mapeadas diretamente no objeto `WebApplication` (ex: `app.MapGet`). Em projetos maiores, exige separação manual (como fizemos na pasta `Endpoints`). | Organização natural baseada em pastas e classes. Cada controlador agrupa rotas sob o mesmo prefixo de forma automática. |
| **Cenário Ideal** | Microsserviços rápidos, APIs de propósito único (como este importador) e funções nativas na cloud. | APIs monolíticas grandes, versionamento complexo e aplicações que partilham lógica extensiva com MVC. |

**Referências Oficiais da Microsoft:**
* [Visão Geral de Minimal APIs](https://learn.microsoft.com/pt-br/aspnet/core/fundamentals/minimal-apis/overview)


## 🏗️ Estrutura do Projeto

O projeto foi organizado da seguinte forma para evitar sobreposição de classes e facilitar a manutenção:

```text
📂 ImportadorOracleEF
 ┣ 📂 Models
 ┃ ┗ 📜 Cliente.cs              # Entidade que representa a tabela no banco
 ┣ 📂 Data
 ┃ ┗ 📜 AppDbContext.cs         # Configuração do EF Core e mapeamento da tabela
 ┣ 📂 Services
 ┃ ┗ 📜 ClienteImportService.cs # Regra de negócio para leitura do arquivo e inserção
 ┣ 📂 Endpoints
 ┃ ┗ 📜 ClienteEndpoints.cs     # Definição das rotas GET e POST da API
 ┣ 📜 Program.cs                # Inicialização da aplicação e injeção de dependências
 ┗ 📜 appsettings.json          # Arquivo de configuração (String de conexão)
```


## 🛠️ Passo a Passo da Criação e Instalação (CLI)

Abra o terminal e execute os comandos abaixo para recriar o ambiente:


### 1. Criar o Projeto Web
```bash
dotnet new web -n ImportadorOracleEF
cd ImportadorOracleEF
```


### 2. Instalar os Pacotes Necessários
```bash
# Provedor do Oracle para o Entity Framework Core
dotnet add package Oracle.EntityFrameworkCore

# Ferramenta para gerar a interface de documentação da API
dotnet add package Swashbuckle.AspNetCore

# Scalar
dotnet add package Microsoft.AspNetCore.OpenApi
dotnet add package Scalar.AspNetCore

# Pastas
mkdir Models Data Services Endpoints

# Abrir no VSCode
code .
```


### 3. Conteúdo
Caso não tenha, crie a tabela no Oracle
```
CREATE TABLE TB_CLIENTE (
    ID_CLIENTE NUMBER(10,0) GENERATED BY DEFAULT AS IDENTITY NOT NULL,
    NOME NVARCHAR2(2000 CHAR),
    SOBRENOME NVARCHAR2(2000 CHAR),
    EMAIL NVARCHAR2(2000 CHAR),
    PRIMARY KEY (ID_CLIENTE)
);
```

Data/AppDbContext
```
// Importa o namespace onde a classe Cliente está localizada.
using ImportadorOracleEF.Models;
// Importa as ferramentas principais do Entity Framework Core.
using Microsoft.EntityFrameworkCore;

// Define o endereço lógico desta classe no projeto (camada de dados).
namespace ImportadorOracleEF.Data;

// Cria a classe de contexto do banco de dados, herdando as funções do EF Core.
public class AppDbContext : DbContext
{
    // Construtor que recebe as configurações de conexão e repassa para a classe base do EF Core.
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    // Representa a tabela de clientes no banco de dados para podermos consultá-la ou alterá-la.
    public DbSet<Cliente> Clientes => Set<Cliente>();

    // Método usado para sobrescrever as regras de mapeamento padrão do EF Core.
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Vincula a classe Cliente à tabela física "TB_CLIENTE" no Oracle.
        modelBuilder.Entity<Cliente>().ToTable("TB_CLIENTE");
        
        // Define que a propriedade IdCliente será a Chave Primária (Primary Key) da tabela.
        modelBuilder.Entity<Cliente>().HasKey(c => c.IdCliente);
        
        // Inicia as configurações específicas para o campo IdCliente.
        modelBuilder.Entity<Cliente>().Property(c => c.IdCliente)
            // Relaciona a propriedade à coluna com nome exato "ID_CLIENTE" no banco.
            .HasColumnName("ID_CLIENTE")
            // Avisa ao EF Core que o banco vai gerar esse valor automaticamente na inserção (usando Sequence).
            .ValueGeneratedOnAdd();
            
        // Mapeia a propriedade Nome para a coluna "NOME" e limita o tamanho máximo para 2000 caracteres.
        modelBuilder.Entity<Cliente>().Property(c => c.Nome).HasColumnName("NOME").HasMaxLength(2000);
        
        // Mapeia a propriedade Sobrenome para a coluna "SOBRENOME" e limita para 2000 caracteres.
        modelBuilder.Entity<Cliente>().Property(c => c.Sobrenome).HasColumnName("SOBRENOME").HasMaxLength(2000);
        
        // Mapeia a propriedade Email para a coluna "EMAIL" e limita para 2000 caracteres.
        modelBuilder.Entity<Cliente>().Property(c => c.Email).HasColumnName("EMAIL").HasMaxLength(2000);
    }
}
```

Models/Cliente.cs
```
namespace ImportadorOracleEF.Models;

public class Cliente
{
    public int IdCliente { get; set; }
    public string? Nome { get; set; }
    public string? Sobrenome { get; set; }
    public string? Email { get; set; }
}
```

Services/ClienteImportService.cs
```
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
```

Endpoints/ClienteEndpoints.cs
```
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
```


Crie ou edite o arquivo `appsettings.Development.json` na raiz do projeto e adicione a sua string de conexão:
```json - appsettings.Development.json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*",
  "ConnectionStrings": {
    "OracleDb": "User Id=SEU_USUARIO;Password=SUA_SENHA;Data Source=SEU_HOST:1521/SEU_SERVICE_NAME;"
  }
}
```


## 🚀 Como Executar

Com os arquivos criados e configurados em suas respectivas pastas, inicie o servidor:
```bash
dotnet run
```
No terminal, será exibida a URL de escuta (exemplo: `http://localhost:5000`). Acesse a interface interativa adicionando `/swagger` ao final da URL (ex: `http://localhost:5000/swagger`).

ou

No terminal, será exibida a URL de escuta (exemplo: http://localhost:5000). Acesse a nova interface interativa do Scalar adicionando /scalar/v1 ao final da URL (ex: http://localhost:5000/scalar/v1).

## 🧪 Como Testar via Swagger

### 1. Formato do Arquivo de Teste
Crie um arquivo de texto local (ex: `clientes.txt`). Os dados devem estar separados por ponto e vírgula na ordem: `Nome;Sobrenome;Email`. Não envie o ID, pois ele é gerado automaticamente pelo Oracle.

**Exemplo de conteúdo do `clientes.txt`:**
```text
Fabio;Crusco;fabio@email.com
Sandra;Paiao;sandra@email.com
Lucas;Almeida;lucas.almeida@email.com
```


### 2. Importando os Dados (Endpoint POST)
1. Abra a página do Swagger no navegador.
2. Expanda a rota **`POST /api/clientes/importar`**.
3. Clique no botão **"Try it out"**.
4. No campo `arquivo`, clique em **"Escolher arquivo"** e selecione o seu `clientes.txt`.
5. Clique em **"Execute"**.
6. O retorno (HTTP 200) informará a quantidade de registos importados com sucesso.


### 3. Consultando os Dados (Endpoint GET)
1. Expanda a rota **`GET /api/clientes`** no Swagger.
2. Clique em **"Try it out"** e depois em **"Execute"**.
3. O Swagger retornará um JSON contendo todos os clientes registados na tabela `TB_CLIENTE` do Oracle Cloud, incluindo os IDs gerados pelas Sequences.


## 🧪 Como Testar via Scalar

### 1. Formato do Arquivo de Teste
Crie um arquivo de texto local (ex: `clientes.txt`). Os dados devem estar separados por ponto e vírgula na ordem: `Nome;Sobrenome;Email`. Não envie o ID, pois ele é gerado automaticamente pelo Oracle.

**Exemplo de conteúdo do `clientes.txt`:**
```text
Fabio;Crusco;fabio@email.com
Sandra;Paiao;sandra@email.com
Lucas;Almeida;lucas.almeida@email.com
```

### 2. Importando os Dados (Endpoint POST)
1. Abra a interface do Scalar no navegador (acessando `/scalar/v1`).
2. No menu de navegação, localize e clique na rota **`POST /api/clientes/importar`**.
3. No painel interativo (à direita ou abaixo), clique no botão **"Test Request"**.
4. Na aba **Body** (que deve estar como `multipart/form-data`), localize o campo referente ao `arquivo`, clique para anexar e selecione o seu `clientes.txt`.
5. Clique no botão azul **"Send"**.
6. O retorno (HTTP 200 OK) aparecerá no painel de resposta, informando a quantidade de registros importados com sucesso.

### 3. Consultando os Dados (Endpoint GET)
1. Retorne ao menu de navegação e clique na rota **`GET /api/clientes`**.
2. Clique em **"Test Request"** e, em seguida, no botão **"Send"**.
3. O Scalar retornará um JSON contendo todos os clientes registrados na tabela `TB_CLIENTE` do Oracle Cloud, incluindo os IDs gerados pelas Sequences.


### Comparativo
| Característica | C# .NET (Minimal APIs) | Python (FastAPI / Flask) | Go (Gin / net/http) | Java (Spring Boot / Javalin) |
| :--- | :--- | :--- | :--- | :--- |
| **Foco e Filosofia** | Alta performance com baixo *boilerplate*. Traz o poder do ecossistema corporativo do .NET para uma sintaxe funcional e direta. | Simplicidade extrema e desenvolvimento rápido. FastAPI brilha ao integrar documentação automática e tipagem via *Type Hints*. | Simplicidade procedural e concorrência nativa (Goroutines). Foco em consumo mínimo de recursos e deploy em binário único. | Estrutura robusta e opinativa (Spring). Javalin tenta trazer a leveza funcional, mas o ecossistema ainda é amplamente orientado a anotações e classes. |
| **Boilerplate (Código Inicial)** | Muito Baixo. Apenas 3 a 4 linhas no `Program.cs` são suficientes para subir um endpoint. | Muito Baixo. Em Flask ou FastAPI, um endpoint é criado com poucas linhas e um decorador `@app.route`. | Baixo. Exige configuração explícita de rotas e tratamento de erros manual, mas sem necessidade de classes complexas. | Alto (Spring) a Médio (Javalin). Spring exige configuração de classes, anotações e injeção de dependência mais verbosa. |
| **Performance e Execução** | Altíssima. O compilador JIT e as otimizações recentes do .NET colocam a linguagem no topo dos benchmarks (ex: TechEmpower). | Baixa a Média. Sendo interpretado (GIL), perde em concorrência bruta, embora o FastAPI (Starlette/Uvicorn) mitigue isso com *async*. | Altíssima. Compilado diretamente para código de máquina, possui um dos tempos de inicialização (*cold start*) mais rápidos do mercado. | Alta. A JVM (Java Virtual Machine) é extremamente otimizada, mas sofre com tempo de inicialização lento (*cold start* alto), exceto se usar GraalVM. |
| **Tipagem e Segurança** | Estática e Forte. O compilador detecta erros antes da execução, garantindo contratos rígidos. | Dinâmica (opcionalmente tipada). Tipagem baseada em dicas (*Type Hints*), o que acelera o código, mas permite falhas em tempo de execução. | Estática e Forte. Tipagem rigorosa, sem suporte a polimorfismo clássico, priorizando a composição de interfaces. | Estática e Forte. Altamente baseada em tipagem rigorosa, interfaces e contratos orientados a objetos. |
| **Ecossistema e Dependências** | Centralizado e oficial (NuGet). A Microsoft fornece a maioria dos pacotes essenciais (EF Core, Auth, Logging) com integração perfeita. | Fragmentado e gigantesco (Pip). Ferramentas excelentes para manipulação de dados, mas exige escolha manual de bibliotecas (ex: SQLAlchemy, Pydantic). | Minimalista. A biblioteca padrão (`net/http`) é tão poderosa que muitos projetos recusam frameworks de terceiros. | Massivo e maduro (Maven/Gradle). Há uma biblioteca consolidada para absolutamente qualquer integração imaginável. |
| **Concorrência** | `async/await` maduro e baseado em *Task Parallel Library (TPL)*. Escala muito bem sob carga pesada. | `async/await` suportado no FastAPI, mas limitado em processamento paralelo real devido ao *Global Interpreter Lock (GIL)*. | *Goroutines* nativas e canais. É o padrão-ouro atual para lidar com milhares de conexões simultâneas com baixo uso de RAM. | *Threads* clássicas pesadas (Spring MVC) ou programação reativa complexa (WebFlux). As *Virtual Threads* do Java 21 estão mudando este cenário. |
| **Cenário Ideal** | Microsserviços corporativos, APIs de alta performance e integração com o ecossistema Azure/Microsoft. | Prototipagem rápida, integração com IA/Machine Learning e scripts de automação. | Infraestrutura cloud nativa (Docker/Kubernetes), proxies e microsserviços que exigem concorrência extrema e baixo custo de hardware. | Sistemas legados gigantescos, aplicações monolíticas corporativas e ecossistemas complexos com regras de negócio profundas. |
