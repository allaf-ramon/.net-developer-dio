using MinimalApi.Dominio.Entidades;

namespace Domain.Tests.Entidades;

[TestClass]

public class AdministradorTest
{
    [TestMethod]
    public void TestarPropriedadesComValoresNulos()
    {
        var admin = new Administrador();
        
        Assert.AreEqual(0, admin.Id);
        Assert.IsNull(admin.Email);
        Assert.IsNull(admin.Senha);
        Assert.IsNull(admin.Perfil);
    }

    [TestMethod]
    public void TestarAlteracaoPropriedades()
    {
        var admin = new Administrador
        {
            Id = 1,
            Email = "admin@teste.com",
            Senha = "123456",
            Perfil = "Admin"
        };

        admin.Email = "novo@teste.com";
        admin.Senha = "654321";
        admin.Perfil = "SuperAdmin";

        Assert.AreEqual("novo@teste.com", admin.Email);
        Assert.AreEqual("654321", admin.Senha);
        Assert.AreEqual("SuperAdmin", admin.Perfil);
    }

    [TestMethod]
    public void TestarPropriedadesComValoresVazios()
    {
        var admin = new Administrador
        {
            Id = 1,
            Email = string.Empty,
            Senha = string.Empty,
            Perfil = string.Empty
        };

        Assert.AreEqual(1, admin.Id);
        Assert.AreEqual(string.Empty, admin.Email);
        Assert.AreEqual(string.Empty, admin.Senha);
        Assert.AreEqual(string.Empty, admin.Perfil);
    }
}
