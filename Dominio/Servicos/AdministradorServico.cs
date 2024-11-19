using MinimalApi.Dominio.Entidades;
using MinimalApi.Dominio.Interfaces;
using MinimalApi.DTOs;
using MinimalApi.Infraestrutura.Database;

namespace MinimalApi.Dominio.Servicos;

public class AdministradorServico : IAdministradorServico
{
    private readonly ApiContext _context;
    public AdministradorServico(ApiContext context)
    {
        _context = context;
    }

    public Administrador? Login(LoginDTO loginDTO)
    {
        return _context.Administradores
            .Where(admin => admin.Email == loginDTO.Email &&
                            admin.Senha == loginDTO.Senha)
            .FirstOrDefault();
    }
}