using MinimalApi.Dominio.Entidades;
using MinimalApi.Dominio.Interfaces;
using MinimalApi.DTOs;
using MinimalApi.Infraestrutura.Database;

namespace MinimalApi.Dominio.Servicos;

public class VeiculoServico : IVeiculoServico
{
    private readonly ApiContext _context;
    public VeiculoServico(ApiContext context)
    {
        _context = context;
    }

    public void Apagar(Veiculo veiculo)
    {
        _context.Remove(veiculo);
        _context.SaveChanges();
    }

    public void Atualizar(Veiculo veiculo)
    {
       _context.Update(veiculo);
       _context.SaveChanges();
    }

    public Veiculo? BuscaPorId(int id)
    {
        return _context.Veiculos
            .FirstOrDefault(veiculo => veiculo.Id == id);
    }

    public void Incluir(Veiculo veiculo)
    {
       _context.Add(veiculo);
       _context.SaveChanges();
    }

    public List<Veiculo> ListarTodos(int pagina = 1, string? nome = null, string? marca = null)
    {
        var veiculos = _context.Veiculos.AsQueryable();

        if (!string.IsNullOrEmpty(nome))
        {
            veiculos = veiculos.Where(v => v.Nome.ToUpper().Contains(nome.ToUpper()));
        }

        const int itensPorPagina = 10;
        
        veiculos = veiculos.Skip((pagina - 1) * itensPorPagina).Take(itensPorPagina);

        return veiculos.ToList();
    }
}
