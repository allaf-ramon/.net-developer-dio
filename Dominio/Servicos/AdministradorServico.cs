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
    public string Administrador { get;}

    public Administrador? BuscaPorId(int id)
    {
        return _context.Administradores.FirstOrDefault(a => a.Id == id);
    }

    public bool Delete(int id)
    {
        var administrador = _context.Administradores.FirstOrDefault(a => a.Id == id);
        if (administrador == null)
            return false;
        
        _context.Administradores.Remove(administrador);
        _context.SaveChanges();
        return true;
    }

    public Administrador Incluir(Administrador administrador)
    {
       _context.Administradores.Add(administrador);
       _context.SaveChanges();
       return administrador;
    }

    public List<Administrador> ListarTodos(int pagina)
    {
        var query = _context.Administradores.AsQueryable();

        int itensPorPagina = 10;

        if (pagina != null)
            query = query.Skip((pagina - 1) * itensPorPagina).Take(itensPorPagina);

        return query.ToList();
    }

    public Administrador? Login(LoginDTO loginDTO)
    {
        return _context.Administradores
            .Where(admin => admin.Email == loginDTO.Email &&
                            admin.Senha == loginDTO.Senha)
            .FirstOrDefault();
    }
}