# 🚀 API de Importação de Arquivos TXT para Oracle Cloud (.NET 10)

Esta API foi desenvolvida em **C# .NET 10** utilizando **Minimal APIs** e **Entity Framework Core**. O projeto conecta-se a uma instância do **Oracle Cloud** para mapear a tabela existente `TB_CLIENTE` e processa a importação de um arquivo `.txt` delimitado por ponto e vírgula (`;`), salvando os dados de forma assíncrona.

A aplicação adota uma arquitetura em camadas (Models, Data, Services e Endpoints) para garantir separação de responsabilidades e organização do código.


## 📋 Pré-requisitos

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- [Visual Studio Code (VS Code)](https://code.visualstudio.com/) ou IDE da sua preferência.
- Instância de banco de dados **Oracle Cloud** contendo a tabela `TB_CLIENTE` com as colunas: `ID_CLIENTE` (gerada por Sequence/nextval), `NOME`, `SOBRENOME` e `EMAIL`.


## 🧠 Minimal APIs vs API Controllers

Este projeto utiliza **Minimal APIs**, uma abordagem introduzida em versões recentes do .NET para simplificar o desenvolvimento. Abaixo encontram-se as principais diferenças didáticas entre esta abordagem e os tradicionais **API Controllers**:

enriqueça ainda mais este comparativo

adicione as versoes do .net em que cada tipo de api nasceu

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

# Pastas
mkdir Models Data Services Endpoints

# Abrir no VSCode
code .
```


### 3. Conteúdo
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

Crie ou edite o arquivo `appsettings.json` na raiz do projeto e adicione a sua string de conexão:
```json - appsettings.json
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
