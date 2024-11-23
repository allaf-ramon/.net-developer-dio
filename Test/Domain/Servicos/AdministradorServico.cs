using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MinimalApi.Dominio.Entidades;
using MinimalApi.Dominio.Servicos;
using MinimalApi.Infraestrutura.Database;

namespace Test.Domain.Servicos;

[TestClass]
public class AdministradorServicoTest
{
    private ApiContext CriarContextoDeTeste()
    {
        var basePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        var fullPath = Path.GetFullPath(Path.Combine(basePath, "..", "..", ".."));
        var builder = new ConfigurationBuilder()
            .SetBasePath(fullPath)
            .AddJsonFile("appsettings.json", false, true)
            .AddEnvironmentVariables();

        var configuration = builder.Build();
        return new ApiContext(configuration);
    }

    [TestMethod]
    public void TesteDeInsercaoDeAdministrador()
    {
        // Arrange
        var contexto = CriarContextoDeTeste();
        contexto.Database.ExecuteSqlRaw("TRUNCATE TABLE Administradores");

        var admin = new Administrador();
        admin.Email = "teste@teste.com";
        admin.Senha = "123456";
        admin.Perfil = "Admin";

        var administradorServico = new AdministradorServico(contexto);

        // Act
        administradorServico.Incluir(admin);

        // Assert
        Assert.AreEqual(1, contexto.Administradores.Count());
    
    }
    [TestMethod]
    public void TesteDeInsercaoEBuscarAdministrador()
    {
        // Arrange
        var contexto = CriarContextoDeTeste();
        contexto.Database.ExecuteSqlRaw("TRUNCATE TABLE Administradores");

        var admin = new Administrador();
        admin.Email = "teste@teste.com";
        admin.Senha = "123456";
        admin.Perfil = "Admin";

        var administradorServico = new AdministradorServico(contexto);

        // Act
        administradorServico.Incluir(admin);
        var adminDoBanco = administradorServico.BuscaPorId(admin.Id);

        // Assert
        Assert.AreEqual(1, adminDoBanco.Id);
    }


}