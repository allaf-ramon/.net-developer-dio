var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.MapPost("/Login", (MinimalApi.DTOs.LoginDTO loginDTO) => 
{
    if (loginDTO.Email == "email@email" && loginDTO.Senha == "12345")
        return Results.Ok("Logado com sucesso!");

    return Results.BadRequest("Usuário ou senha inválidos!");
});

app.Run();
