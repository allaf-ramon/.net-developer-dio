# Minimal API

Uma API minimalista desenvolvida em .NET 8 para gerenciamento de veículos e administradores do sistema.

## Funcionalidades Principais
- Autenticação e autorização de administradores
- Gerenciamento de veículos (CRUD)
- Listagem paginada de registros
- Perfis de acesso (Admin e Editor)

## Tecnologias Utilizadas
- .NET 8
- Entity Framework Core
- MySQL
- JWT para autenticação
- MSTest para testes
- Swagger/OpenAPI

## Instalação e Configuração

1. Clone o repositório:
   git clone https://github.com/seu-usuario/minimal-api.git

2. Configure a string de conexão no arquivo appsettings.json:

3. Execute as migrações do banco de dados:
   dotnet ef database update

4. Execute o projeto:
   dotnet run

## Estrutura do Projeto
- /Api - Código fonte principal da API
- /Test - Testes automatizados
- /Domain - Regras de negócio e entidades
- /Infrastructure - Acesso a dados e configurações

## Testes
Execute os testes usando o comando:
dotnet test

## Endpoints Disponíveis

### Administradores
- POST /administradores/login - Login no sistema
- GET /administradores - Lista todos administradores (requer perfil Admin)
- POST /administradores - Cria novo administrador

### Veículos
- GET /veiculos - Lista todos veículos
- GET /veiculos/{id} - Busca veículo por ID
- POST /veiculos - Adiciona novo veículo
- PUT /veiculos/{id} - Atualiza veículo existente
- DELETE /veiculos/{id} - Remove veículo

## Perfis de Acesso
- **Admin**: Acesso total ao sistema
- **Editor**: Acesso limitado ao gerenciamento de veículos
