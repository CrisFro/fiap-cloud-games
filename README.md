# 🎮 FIAP Cloud Games - Tech Challenge Fase 1

API REST em .NET 8 para cadastro de usuários e gerenciamento de biblioteca de jogos, com autenticação JWT e controle de acesso por roles.

---

## 🚀 Começando

### Pré-requisitos

- [.NET 8 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)
- SQL Server (local ou via Docker)
- Visual Studio 2022 / VS Code ou outro IDE compatível

### Configuração

1. Clone o repositório:

```bash
git clone https://github.com/seu-usuario/fiap-cloud-games.git
cd fiap-cloud-games
```

2. Configure a string de conexão em `appsettings.Development.json`:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=localhost;Database=FiapCloudGamesDb;Trusted_Connection=True;"
}
```

3. Crie o banco de dados e aplique as migrations:

```bash
dotnet ef database update --project src/FiapCloudGames.Infrastructure
```

4. Rode a API:

```bash
dotnet run --project src/FiapCloudGames.API
```

A API estará disponível em `https://localhost:5001`.

---

## 🧪 Testes

- Testes unitários com xUnit e Moq
- Testes comportamentais com BDDfy

Executar todos os testes:

```bash
dotnet test
```

---

## 🔑 Autenticação

- JWT baseado em roles: `User` e `Admin`
- Usuário Admin: pode criar jogos, promoções e gerenciar usuários
- Usuário Comum: pode visualizar apenas sua biblioteca

---

## 🗂 Estrutura do Projeto

```plaintext
src/
├── FiapCloudGames.API           # Controllers, Middleware, Swagger
├── FiapCloudGames.Application   # Services, DTOs, Interfaces
├── FiapCloudGames.Domain        # Entidades, Enums, Regras de negócio
├── FiapCloudGames.Infrastructure# Repositórios, DbContext, JWT Service
├── FiapCloudGames.Tests         # Testes unitários e BDD
```

---

## 📖 Documentação

Após iniciar a API, a documentação Swagger estará disponível em:

https://localhost:5001/swagger
