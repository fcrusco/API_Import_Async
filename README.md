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


### 3. Configurar a String de Conexão (`appsettings.json`)
Crie ou edite o arquivo `appsettings.json` na raiz do projeto e adicione a sua string de conexão no formato EZConnect:
```json
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
