using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MinimalApi.Dominio.Entidades;
using MinimalApi.Dominio.Interfaces;
using MinimalApi.Dominio.ModelViews;
using MinimalApi.Dominio.Servicos;
using MinimalApi.DTOs;
using MinimalApi.Infraestrutura.Database;

#region Builder
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<IAdministradorServico, AdministradorServico>();
builder.Services.AddScoped<IVeiculoServico, VeiculoServico>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApiContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

var app = builder.Build();
#endregion

#region Home
app.MapGet("/", () => Results.Json(new Home())).WithTags("Home");
#endregion

#region Administradores
app.MapPost("/administradores/login", ([FromBody] LoginDTO loginDTO, IAdministradorServico administradorServico) =>
{
    var admin = administradorServico.Login(loginDTO);

    if (admin != null)
    {
        return Results.Ok("Logado com sucesso!");
    }

    return Results.BadRequest("Usuário ou senha inválidos!");
}).WithTags("Administradores");
#endregion

#region Veiculos
ErrosDeValidacao validacaoDTO(VeiculoDTO veiculoDTO)
{
    var validationErrors = new ErrosDeValidacao{
        Mensagens = new List<string>()
    };

    if (string.IsNullOrWhiteSpace(veiculoDTO.Nome))
        validationErrors.Mensagens.Add("O campo Nome é obrigatório!");

    if (string.IsNullOrWhiteSpace(veiculoDTO.Marca))
        validationErrors.Mensagens.Add("O campo Marca é obrigatório!");

    if (veiculoDTO.Ano < 1950)
        validationErrors.Mensagens.Add("O campo Ano deve ser maior que 1950!");
    
    return validationErrors;
}

app.MapPost("/veiculos", ([FromBody] VeiculoDTO veiculoDTO, IVeiculoServico veiculoServico) =>
{
    var validacao = validacaoDTO(veiculoDTO);
    if (validacao.Mensagens.Count > 0)
        return Results.BadRequest(validacao);

    var veiculo = new Veiculo{
        Nome = veiculoDTO.Nome,
        Marca = veiculoDTO.Marca,
        Ano = veiculoDTO.Ano
    };
    veiculoServico.Incluir(veiculo);
    
    return Results.Created($"/veiculos/{veiculo.Id}", veiculo);
}).WithTags("Veiculos");

app.MapGet("/veiculos", ([FromQuery] int pagina, IVeiculoServico veiculoServico) =>
{
    var veiculos = veiculoServico.ListarTodos(pagina);
    
    return Results.Ok(veiculos);
}).WithTags("Veiculos");

app.MapGet("/veiculos/{id}", ([FromRoute] int id, IVeiculoServico veiculoServico) =>
{
    var veiculo = veiculoServico.BuscaPorId(id);

    if (veiculo == null)
        return Results.NotFound();
    
    return Results.Ok(veiculo);
}).WithTags("Veiculos");

app.MapPut("/veiculos/{id}", ([FromRoute] int id, VeiculoDTO veiculoDTO, IVeiculoServico veiculoServico) =>
{
    var veiculo = veiculoServico.BuscaPorId(id);
    if (veiculo == null) 
        return Results.NotFound();

    var validacao = validacaoDTO(veiculoDTO);
    if (validacao.Mensagens.Count > 0)
        return Results.BadRequest(validacao);
    
    veiculo.Nome = veiculoDTO.Nome;
    veiculo.Marca = veiculoDTO.Marca;
    veiculo.Ano = veiculoDTO.Ano;

    veiculoServico.Atualizar(veiculo);
    
    return Results.NoContent();
}).WithTags("Veiculos");

app.MapDelete("/veiculos/{id}", ([FromRoute] int id, IVeiculoServico veiculoServico) =>
{
    var veiculo = veiculoServico.BuscaPorId(id);
    if (veiculo == null) 
        return Results.NotFound();
    
    veiculoServico.Apagar(veiculo);
    
    return Results.NoContent();
}).WithTags("Veiculos");
#endregion

#region App
app.UseSwagger();
app.UseSwaggerUI();
app.Run();
#endregion