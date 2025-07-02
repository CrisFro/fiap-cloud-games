# FIAP Cloud Games - API

Projeto desenvolvido como parte do **Tech Challenge** da FIAP.  
Essa API permite o cadastro e autenticação de usuários, bem como a futura gestão de jogos adquiridos por cada um.

## Tecnologias Utilizadas

- ASP.NET Core 8 (Web API)
- Entity Framework Core
- DDD (Domain-Driven Design)
- JWT (em breve)
- Swagger para documentação
- SQL Server

## Estrutura de Pastas

- `Fcg.Api` - Camada de apresentação (Controllers, Program.cs)
- `Fcg.Application` - Camada de aplicação (DTOs, Interfaces, Serviços)
- `Fcg.Domain` - Camada de domínio (Entidades)
- `Fcg.Infrastructure` - Camada de infraestrutura (DbContext, Repositórios)

## Funcionalidades atuais

- Registro de usuário com:
  - Nome 
  - E-mail válido
  - Senha
- Validações básicas com `DataAnnotations`
- Swagger configurado

## Como rodar

1. Clonar o repositório:
   ```bash
   git clone https://github.com/CrisFro/fiap-cloud-games.git