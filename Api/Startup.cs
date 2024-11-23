using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using MinimalApi;
using MinimalApi.Dominio.Entidades;
using MinimalApi.Dominio.Enuns;
using MinimalApi.Dominio.Interfaces;
using MinimalApi.Dominio.ModelViews;
using MinimalApi.Dominio.Servicos;
using MinimalApi.DTOs;
using MinimalApi.Infraestrutura.Database;

public class Startup
{
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
        key = Configuration.GetSection("Jwt").ToString() ?? "";
    }

    private string key;
    public IConfiguration Configuration { get; }

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddAuthorization();
        
        if (string.IsNullOrEmpty(key))
            throw new Exception("Chave JWT não encontrada");

        services.AddAuthentication(option => {
            option.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            option.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(option => {
            option.TokenValidationParameters = new TokenValidationParameters{
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key))
            };
            
        });
        services.AddScoped<IAdministradorServico, AdministradorServico>();
        services.AddScoped<IVeiculoServico, VeiculoServico>();
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(Options => {
            Options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme{
                Description = "Insira o token JWT",
                Name = "Authorization",
                Scheme = "Bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.Http
            });
            Options.AddSecurityRequirement(new OpenApiSecurityRequirement{
                {
                    new OpenApiSecurityScheme{
                        Reference = new OpenApiReference{
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    new string[]{}
                }
            });
        });
        services.AddCors(options =>
        {
            options.AddPolicy("CorsPolicy",
                builder => builder
                    .AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader()
            );
        });

        var connectionString = Configuration.GetConnectionString("DefaultConnection");
        services.AddDbContext<ApiContext>(options =>
            options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        app.UseSwagger();
        app.UseSwaggerUI();

        app.UseRouting();

        app.UseAuthentication();
        app.UseAuthorization();

        app.UseEndpoints(endpoints =>
        {
            #region Home
            endpoints.MapGet("/", () => Results.Json(new Home())).AllowAnonymous().WithTags("Home");
            #endregion

            #region Administradores
            string gerarToken(Administrador administrador)
            {
                if (string.IsNullOrEmpty(key))
                    return string.Empty;

                var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
                var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

                var claims = new List<Claim>()
                {
                    new Claim("Email", administrador.Email),
                    new Claim("Perfil", administrador.Perfil),
                    new Claim(ClaimTypes.Role, administrador.Perfil)
                };

                var Token = new JwtSecurityToken(
                    claims: claims,
                    expires: DateTime.Now.AddMinutes(120),
                    signingCredentials: credentials
                );

                return new JwtSecurityTokenHandler().WriteToken(Token);
            }

            endpoints.MapPost("/administradores/login", ([FromBody] LoginDTO loginDTO, IAdministradorServico administradorServico) =>
            {
                var admin = administradorServico.Login(loginDTO);

                if (admin != null)
                {
                    string token = gerarToken(admin);
                    return Results.Ok(new AdministradorLogado
                    {
                        Email = admin.Email,
                        Perfil = admin.Perfil,
                        Token = token
                    });
                }

                return Results.BadRequest("Usuário ou senha inválidos!");
            }).AllowAnonymous().WithTags("Administradores");

            endpoints.MapPost("/administradores", ([FromBody] AdministradorDTO administradorDTO, IAdministradorServico administradorServico) =>
            {
                var validacao = new ErrosDeValidacao
                {
                    Mensagens = new List<string>()
                };

                if (string.IsNullOrWhiteSpace(administradorDTO.Email))
                    validacao.Mensagens.Add("O campo Email é obrigatório!");

                if (string.IsNullOrWhiteSpace(administradorDTO.Senha))
                    validacao.Mensagens.Add("O campo Senha é obrigatório!");

                if (administradorDTO.Perfil == null)
                    validacao.Mensagens.Add("O campo Perfil é obrigatório!");

                if (validacao.Mensagens.Count > 0)
                    return Results.BadRequest(validacao);

                var administrador = new Administrador
                {
                    Email = administradorDTO.Email,
                    Senha = administradorDTO.Senha,
                    Perfil = administradorDTO.Perfil?.ToString() ?? Perfil.Editor.ToString(),
                };

                administradorServico.Incluir(administrador);

                return Results.Created($"/administradores/{administrador.Id}",
                new AdministradorModelView
                {
                    Id = administrador.Id,
                    Email = administrador.Email,
                    Perfil = administrador.Perfil
                });
            }).RequireAuthorization().WithTags("Administradores");

            endpoints.MapGet("/administradores", ([FromQuery] int? pagina, IAdministradorServico administradorServico) =>
            {
                var admModelView = new List<AdministradorModelView>();
                var administradores = administradorServico.ListarTodos(pagina ?? 1);
                foreach (var adm in administradores)
                {
                    admModelView.Add(new AdministradorModelView
                    {
                        Id = adm.Id,
                        Email = adm.Email,
                        Perfil = adm.Perfil
                    });
                }
                return Results.Ok(admModelView);
            }).RequireAuthorization()
            .RequireAuthorization(new AuthorizeAttribute { Roles = "Admin" })
            .WithTags("Administradores");

            endpoints.MapGet("/administradores/{id}", ([FromRoute] int id, IAdministradorServico administradorServico) =>
            {
                var administrador = administradorServico.BuscaPorId(id);
                if (administrador == null)
                    return Results.NotFound();

                return Results.Ok(new AdministradorModelView
                {
                    Id = administrador.Id,
                    Email = administrador.Email,
                    Perfil = administrador.Perfil
                });
            }).RequireAuthorization()
            .RequireAuthorization(new AuthorizeAttribute { Roles = "Admin" })
            .WithTags("Administradores");
            #endregion

            #region Veiculos
            ErrosDeValidacao validacaoDTO(VeiculoDTO veiculoDTO)
            {
                var validationErrors = new ErrosDeValidacao
                {
                    Mensagens = new List<string>()
                };

                if (string.IsNullOrWhiteSpace(veiculoDTO.Nome))
                    validationErrors.Mensagens.Add("O campo Nome é obrigatório!");

                if (string.IsNullOrWhiteSpace(veiculoDTO.Marca))
                    validationErrors.Mensagens.Add("O campo Marca é obrigatório!");

                if (veiculoDTO.Ano < 1950)
                    validationErrors.Mensagens.Add("O campo Ano deve ser maior que 1950!");

                return validationErrors;
            }

            endpoints.MapPost("/veiculos", ([FromBody] VeiculoDTO veiculoDTO, IVeiculoServico veiculoServico) =>
            {
                var validacao = validacaoDTO(veiculoDTO);
                if (validacao.Mensagens.Count > 0)
                    return Results.BadRequest(validacao);

                var veiculo = new Veiculo
                {
                    Nome = veiculoDTO.Nome,
                    Marca = veiculoDTO.Marca,
                    Ano = veiculoDTO.Ano
                };
                veiculoServico.Incluir(veiculo);

                return Results.Created($"/veiculos/{veiculo.Id}", veiculo);
            }).RequireAuthorization()
            .RequireAuthorization(new AuthorizeAttribute { Roles = "Admin, Editor" })
            .WithTags("Veiculos");

            endpoints.MapGet("/veiculos", ([FromQuery] int pagina, IVeiculoServico veiculoServico) =>
            {
                var veiculos = veiculoServico.ListarTodos(pagina);

                return Results.Ok(veiculos);
            }).RequireAuthorization().WithTags("Veiculos");

            endpoints.MapGet("/veiculos/{id}", ([FromRoute] int id, IVeiculoServico veiculoServico) =>
            {
                var veiculo = veiculoServico.BuscaPorId(id);

                if (veiculo == null)
                    return Results.NotFound();

                return Results.Ok(veiculo);
            }).RequireAuthorization()
            .RequireAuthorization(new AuthorizeAttribute { Roles = "Admin, Editor" })
            .WithTags("Veiculos");

            endpoints.MapPut("/veiculos/{id}", ([FromRoute] int id, VeiculoDTO veiculoDTO, IVeiculoServico veiculoServico) =>
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
            }).RequireAuthorization()
            .RequireAuthorization(new AuthorizeAttribute { Roles = "Admin" })
            .WithTags("Veiculos");

            endpoints.MapDelete("/veiculos/{id}", ([FromRoute] int id, IVeiculoServico veiculoServico) =>
            {
                var veiculo = veiculoServico.BuscaPorId(id);
                if (veiculo == null)
                    return Results.NotFound();

                veiculoServico.Apagar(veiculo);

                return Results.NoContent();
            }).RequireAuthorization()
            .RequireAuthorization(new AuthorizeAttribute { Roles = "Admin" })
            .WithTags("Veiculos");
            #endregion
        });
    }
}