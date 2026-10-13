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