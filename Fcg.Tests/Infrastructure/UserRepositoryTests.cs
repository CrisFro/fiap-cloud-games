using Fcg.Infrastructure.Data;
using Fcg.Infrastructure.Repositories;
using Fcg.Domain.Entities;
using Fcg.Infrastructure.Tables;
using Fcg.Infrastructure.Tests.Mocks; 
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using User = Fcg.Infrastructure.Tables.User; 
using Game = Fcg.Infrastructure.Tables.Game; 
using Promotion = Fcg.Infrastructure.Tables.Promotion; 
using UserGaming = Fcg.Infrastructure.Tables.UserGaming; 


namespace Fcg.Infrastructure.Tests.Repositories
{
    [Trait("Domain-infrastructure", "User Repository")]
    public class UserRepositoryTests
    {
        [Fact]
        public async Task CreateUserAsync_AddsUserToDatabase()
        {
            // Arrange
            var users = new List<User>();
            var mockSet = users.BuildMockDbSet();
            mockSet.Setup(m => m.Add(It.IsAny<User>())).Callback<User>(users.Add);
            
            var options = new DbContextOptionsBuilder<FcgDbContext>()
                .Options;

        
            var mockContext = new Mock<FcgDbContext>(options);
            mockContext.Setup(c => c.Users).Returns(mockSet.Object);
            mockContext.Setup(m => m.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

            var repository = new UserRepository(mockContext.Object);
            var newUser = new Fcg.Domain.Entities.User(Guid.NewGuid(), "Non Existent", "nonexistent@example.com", "hash", new List<Fcg.Domain.Entities.UserGaming>(), "Player");

            // Act
            var createdId = await repository.CreateUserAsync(newUser);

            // Assert
            Assert.NotEqual(Guid.Empty, createdId);
            mockSet.Verify(m => m.Add(It.IsAny<User>()), Times.Once());
            mockContext.Verify(m => m.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once());
            Assert.Single(users);
            Assert.Equal(newUser.Email, users.First().Email);
        }


        [Fact]
        public async Task GetUserByEmailAsync_ReturnsUser_WhenEmailExists()
        {
            // Arrange
            var game1 = new Game { Id = Guid.NewGuid(), Title = "Game 1", Description = "D1", Genre = "A", Price = 10m, CreatedAt = DateTime.UtcNow };
            var game2 = new Game { Id = Guid.NewGuid(), Title = "Game 2", Description = "D2", Genre = "B", Price = 20m, CreatedAt = DateTime.UtcNow };

            var userGaming1 = new UserGaming { Id = Guid.NewGuid(), GameId = game1.Id, Game = game1, PurchasedDate = DateTime.UtcNow };
            var userGaming2 = new UserGaming { Id = Guid.NewGuid(), GameId = game2.Id, Game = game2, PurchasedDate = DateTime.UtcNow };

            var existingUser = new User
            {
                Id = Guid.NewGuid(),
                Name = "Existing User",
                Email = "existing@example.com",
                PasswordHash = "hash1",
                Role = "Player",
                Library = new List<UserGaming> { userGaming1, userGaming2 }
            };
            // Define o User para cada UserGaming após criar o UserGaming
            userGaming1.User = existingUser;
            userGaming2.User = existingUser;

            var users = new List<User> { existingUser };
            var mockUserSet = users.BuildMockDbSet();
            mockUserSet.Setup(m => m.Add(It.IsAny<User>())).Callback<User>(users.Add);

            // --- CORREÇÃO AQUI ---
            var options = new DbContextOptionsBuilder<FcgDbContext>().Options;
            var mockContext = new Mock<FcgDbContext>(options); // Passando 'options'
            // --- FIM DA CORREÇÃO ---

            mockContext.Setup(c => c.Users).Returns(mockUserSet.Object);

            var repository = new UserRepository(mockContext.Object);

            // Act
            var result = await repository.GetUserByEmailAsync("existing@example.com");

            // Assert
            Assert.NotNull(result);
            Assert.Equal(existingUser.Id, result.Id);
            Assert.Equal(existingUser.Email, result.Email);
            Assert.NotNull(result.Library);
            Assert.Equal(2, result.Library.Count());
            Assert.Contains(result.Library, ug => ug.Game.Title == "Game 1");
        }

        [Fact]
        public async Task GetUserByEmailAsync_ReturnsNull_WhenEmailDoesNotExist()
        {
            // Arrange
            var users = new List<User>();
            var mockUserSet = users.BuildMockDbSet();

            // A linha abaixo 'mockSet' parece redundante, 'mockUserSet' já existe.
            // Eu manteria apenas 'mockUserSet' para clareza.
            // var mockSet = users.BuildMockDbSet();
            // mockSet.Setup(m => m.Add(It.IsAny<User>())).Callback<User>(users.Add);

            // --- CORREÇÃO AQUI ---
            var options = new DbContextOptionsBuilder<FcgDbContext>().Options;
            var mockContext = new Mock<FcgDbContext>(options); // Passando 'options'
            // --- FIM DA CORREÇÃO ---

            mockContext.Setup(c => c.Users).Returns(mockUserSet.Object);

            var repository = new UserRepository(mockContext.Object);

            // Act
            var result = await repository.GetUserByEmailAsync("nonexistent@example.com");

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetUserByIdAsync_ReturnsUser_WhenIdExists()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var game1 = new Game { Id = Guid.NewGuid(), Title = "Game X", Description = "DX", Genre = "X", Price = 10m, CreatedAt = DateTime.UtcNow };
            var userGaming1 = new UserGaming { Id = Guid.NewGuid(), GameId = game1.Id, Game = game1, PurchasedDate = DateTime.UtcNow };

            var existingUser = new User
            {
                Id = userId,
                Name = "User By Id",
                Email = "id@example.com",
                PasswordHash = "hash2",
                Role = "Admin",
                Library = new List<UserGaming> { userGaming1 }
            };
            userGaming1.User = existingUser;

            var users = new List<User> { existingUser };
            var mockUserSet = users.BuildMockDbSet();
            
            // --- CORREÇÃO AQUI ---
            var options = new DbContextOptionsBuilder<FcgDbContext>().Options;
            var mockContext = new Mock<FcgDbContext>(options); // Passando 'options'
            // --- FIM DA CORREÇÃO ---

            mockContext.Setup(c => c.Users).Returns(mockUserSet.Object);

            var repository = new UserRepository(mockContext.Object);

            // Act
            var result = await repository.GetUserByIdAsync(userId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(userId, result.Id);
            Assert.Equal(existingUser.Name, result.Name);
            Assert.NotNull(result.Library);
            Assert.Single(result.Library);
            Assert.Equal("Game X", result.Library.First().Game.Title);
        }

        [Fact]
        public async Task GetUserByIdAsync_ReturnsNull_WhenIdDoesNotExist()
        {
            // Arrange
            var users = new List<User>();
            var mockUserSet = users.BuildMockDbSet();
            
            // --- CORREÇÃO AQUI ---
            var options = new DbContextOptionsBuilder<FcgDbContext>().Options;
            var mockContext = new Mock<FcgDbContext>(options); // Passando 'options'
            // --- FIM DA CORREÇÃO ---

            mockContext.Setup(c => c.Users).Returns(mockUserSet.Object);

            var repository = new UserRepository(mockContext.Object);

            // Act
            var result = await repository.GetUserByIdAsync(Guid.NewGuid());

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task UpdateUserLibraryAsync_AddsGamesToLibrary()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var userEntity = new User { Id = userId, Name = "User With Library", Email = "lib@example.com", PasswordHash = "hash3", Role = "Player", Library = new List<UserGaming>() };
            var gamesData = new List<Game>
            {
                new Game { Id = Guid.NewGuid(), Title = "Game 1", Description = "D1", Genre = "A", Price = 10m, CreatedAt = DateTime.UtcNow },
                new Game { Id = Guid.NewGuid(), Title = "Game 2", Description = "D2", Genre = "B", Price = 20m, CreatedAt = DateTime.UtcNow }
            };

            var users = new List<User> { userEntity };
            var userGamingsInDb = new List<UserGaming>(); // Simula o estado inicial do DB

            var mockUserSet = users.BuildMockDbSet();
            var mockUserGamingSet = userGamingsInDb.BuildMockDbSet();

            // Simula AddRange para UserGamings
            mockUserGamingSet.Setup(m => m.AddRange(It.IsAny<IEnumerable<UserGaming>>()))
                .Callback<IEnumerable<UserGaming>>(newItems => userGamingsInDb.AddRange(newItems));

            // --- CORREÇÃO AQUI ---
            var options = new DbContextOptionsBuilder<FcgDbContext>().Options;
            var mockContext = new Mock<FcgDbContext>(options); // Passando 'options'
            // --- FIM DA CORREÇÃO ---

            mockContext.Setup(c => c.Users).Returns(mockUserSet.Object);
            mockContext.Setup(c => c.UserGamings).Returns(mockUserGamingSet.Object);
            mockContext.Setup(m => m.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(It.IsAny<int>());

            var repository = new UserRepository(mockContext.Object);
            //var domainUser = new Fcg.Domain.Entities.User(userId, userEntity.Name, userEntity.Email, userEntity.PasswordHash, new List<Fcg.Domain.Entities.UserGaming>(), Fcg.Domain.Enums.UserRole.Player);
            
            // ATENÇÃO: Verifique a criação deste `domainUser`. Se o UserRepository busca o usuário por ID
            // antes de atualizar a biblioteca, este `domainUser` deveria ser o `userEntity`
            // convertido para a entidade de domínio.
            // Vou usar o `userId` que você já definiu e o `userEntity` existente para que o método
            // de repositório encontre o usuário correto, se ele procurar por ID.
            var domainUser = new Fcg.Domain.Entities.User(userId, userEntity.Name, userEntity.Email, userEntity.PasswordHash, new List<Fcg.Domain.Entities.UserGaming>(), userEntity.Role);

            domainUser.AddGameToLibrary(new Fcg.Domain.Entities.Game(gamesData[0].Id, gamesData[0].Title, gamesData[0].Description, gamesData[0].Genre, gamesData[0].Price, gamesData[0].CreatedAt));
            domainUser.AddGameToLibrary(new Fcg.Domain.Entities.Game(gamesData[1].Id, gamesData[1].Title, gamesData[1].Description, gamesData[1].Genre, gamesData[1].Price, gamesData[1].CreatedAt));

            // Act
            await repository.UpdateUserLibraryAsync(domainUser);

            // Assert
            mockUserGamingSet.Verify(m => m.AddRange(It.IsAny<IEnumerable<UserGaming>>()), Times.Once());
            mockContext.Verify(m => m.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once());
            Assert.Equal(2, userGamingsInDb.Count); // Verifica que foram adicionados à lista simulada
            Assert.Contains(userGamingsInDb, ug => ug.GameId == gamesData[0].Id);
            Assert.Contains(userGamingsInDb, ug => ug.GameId == gamesData[1].Id);
        }

        [Fact]
        public async Task UpdateUserLibraryAsync_HandlesMixedAddAndRemoveOperations()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var gameExisting = new Game { Id = Guid.NewGuid(), Title = "Game Existing", Description = "DE", Genre = "Existing", Price = 10m, CreatedAt = DateTime.UtcNow };
            var gameToAdd = new Game { Id = Guid.NewGuid(), Title = "Game To Add", Description = "DTA", Genre = "Add", Price = 20m, CreatedAt = DateTime.UtcNow };
            var gameToRemove = new Game { Id = Guid.NewGuid(), Title = "Game To Remove", Description = "DTR", Genre = "Remove", Price = 30m, CreatedAt = DateTime.UtcNow };

            var userGamingExisting = new UserGaming { Id = Guid.NewGuid(), UserId = userId, GameId = gameExisting.Id, Game = gameExisting, PurchasedDate = DateTime.UtcNow };
            var userGamingToRemove = new UserGaming { Id = Guid.NewGuid(), UserId = userId, GameId = gameToRemove.Id, Game = gameToRemove, PurchasedDate = DateTime.UtcNow };

            var userGamingsInDb = new List<UserGaming> { userGamingExisting, userGamingToRemove }; // Simula o estado inicial do DB
            var users = new List<User> { new User { Id = userId, Name = "User Mixed Ops", Email = "mixed@example.com", PasswordHash = "hash4", Role = "Player", Library = userGamingsInDb } };
            var gamesData = new List<Game> { gameExisting, gameToAdd, gameToRemove }; // Dados para jogos


            var mockUserSet = users.BuildMockDbSet();
            var mockUserGamingSet = userGamingsInDb.BuildMockDbSet();

            mockUserGamingSet.Setup(m => m.AddRange(It.IsAny<IEnumerable<UserGaming>>()))
                .Callback<IEnumerable<UserGaming>>(newItems => userGamingsInDb.AddRange(newItems));
            mockUserGamingSet.Setup(m => m.RemoveRange(It.IsAny<IEnumerable<UserGaming>>()))
                .Callback<IEnumerable<UserGaming>>(itemsToRemove => userGamingsInDb.RemoveAll(ug => itemsToRemove.Any(r => r.Id == ug.Id)));

            // --- CORREÇÃO AQUI ---
            var options = new DbContextOptionsBuilder<FcgDbContext>().Options;
            var mockContext = new Mock<FcgDbContext>(options); // Passando 'options'
            // --- FIM DA CORREÇÃO ---

            mockContext.Setup(c => c.Users).Returns(mockUserSet.Object);
            mockContext.Setup(c => c.Games).Returns(gamesData.BuildMockDbSet().Object); // Se o repositório usar Games diretamente, mocke
            mockContext.Setup(c => c.UserGamings).Returns(mockUserGamingSet.Object);
            mockContext.Setup(m => m.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(It.IsAny<int>());

            var repository = new UserRepository(mockContext.Object);

            // ATENÇÃO: Assim como no teste anterior, certifique-se de que este `domainUser`
            // representa o usuário correto que o repositório tentará encontrar ou atualizar.
            // Para "mixed ops", você precisa que o `domainUser` comece com a biblioteca *atual*
            // do usuário no banco (simulada por `userGamingsInDb`) e então aplique as mudanças.
            // Estou criando um novo UserDomain com o ID do usuário existente e a biblioteca atual.
            var domainUser = new Fcg.Domain.Entities.User(
                userId,
                users.First().Name, // Pegando o nome do usuário simulado
                users.First().Email, // Pegando o email
                users.First().PasswordHash,
                // Clonar a lista para que as operações de domínio não afetem diretamente userGamingsInDb antes do método do repositório
                userGamingsInDb.Select(ug => new Fcg.Domain.Entities.UserGaming(ug.Id, null, new Fcg.Domain.Entities.Game(ug.Game.Id, ug.Game.Title, ug.Game.Description, ug.Game.Genre, ug.Game.Price, ug.Game.CreatedAt), ug.PurchasedDate)).ToList(),
                users.First().Role
            );


            // Simula operações no domínio
            domainUser.AddGameToLibrary(new Fcg.Domain.Entities.Game(gameToAdd.Id, gameToAdd.Title, gameToAdd.Description, gameToAdd.Genre, gameToAdd.Price, gameToAdd.CreatedAt));
            // AQUI ESTÁ A CORREÇÃO: Passando o Guid do Game
            domainUser.RemoveGameFromLibrary(gameToRemove.Id);

            // Act
            await repository.UpdateUserLibraryAsync(domainUser);

            // Assert
            mockUserGamingSet.Verify(m => m.AddRange(It.IsAny<IEnumerable<UserGaming>>()), Times.Once());
            mockUserGamingSet.Verify(m => m.RemoveRange(It.IsAny<IEnumerable<UserGaming>>()), Times.Once());
            mockContext.Verify(m => m.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once());
            Assert.Equal(2, userGamingsInDb.Count);
            Assert.Contains(userGamingsInDb, ug => ug.GameId == gameExisting.Id);
            Assert.Contains(userGamingsInDb, ug => ug.GameId == gameToAdd.Id);
            Assert.DoesNotContain(userGamingsInDb, ug => ug.GameId == gameToRemove.Id);
        }

        [Fact]
        public async Task UpdateUserRoleAsync_UpdatesRoleCorrectly()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var userEntity = new User { Id = userId, Name = "Role Update User", Email = "role@example.com", PasswordHash = "hash5", Role = "Player" };

            // Converte para IQueryable (ainda útil se houver outros métodos no repositório que usam LINQ,
            // mas para FindAsync, o mock direto é o mais importante).
            var users = new List<User> { userEntity }.AsQueryable();

            var mockUserSet = new Mock<DbSet<User>>();

            // Configura o DbSet mockado para simular uma consulta LINQ (bom para outros métodos)
            mockUserSet.As<IQueryable<User>>().Setup(m => m.Provider).Returns(users.Provider);
            mockUserSet.As<IQueryable<User>>().Setup(m => m.Expression).Returns(users.Expression);
            mockUserSet.As<IQueryable<User>>().Setup(m => m.ElementType).Returns(users.ElementType);
            mockUserSet.As<IQueryable<User>>().Setup(m => m.GetEnumerator()).Returns(users.GetEnumerator());

            // *** ESSA É A CORREÇÃO PRINCIPAL: MOCKANDO FindAsync DIRETAMENTE ***
            // Como seu repositório usa FindAsync(userId), precisamos configurar esse método específico
            // para retornar o userEntity que você criou para o teste.
            mockUserSet.Setup(m => m.FindAsync(userId)).ReturnsAsync(userEntity);
            // Ou, se você quiser ser mais genérico e garantir que qualquer FindAsync seja coberto:
            // mockUserSet.Setup(m => m.FindAsync(It.IsAny<object[]>())).ReturnsAsync((object[] ids) => users.FirstOrDefault(u => u.Id == (Guid)ids[0]));
            // No seu caso específico com 'userId' como único parâmetro, a primeira opção é mais simples e direta.

            var options = new DbContextOptionsBuilder<FcgDbContext>().Options;
            var mockContext = new Mock<FcgDbContext>(options);

            mockContext.Setup(c => c.Users).Returns(mockUserSet.Object);
            mockContext.Setup(m => m.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

            var repository = new UserRepository(mockContext.Object);
            var newRole = "Admin";

            // Act
            await repository.UpdateUserRoleAsync(userId, newRole);

            // Assert
            // Verifica se SaveChangesAsync foi chamado no contexto mockado.
            mockContext.Verify(m => m.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once());

            // Verifica se o método FindAsync foi chamado no DbSet mockado com o userId correto.
            mockUserSet.Verify(m => m.FindAsync(userId), Times.Once());

            // Verifica se a propriedade Role da *sua entidade mockada* foi atualizada.
            // Isso confirma que o repositório encontrou a entidade e a modificou.
            Assert.Equal(newRole, userEntity.Role);
        }

        [Fact]
        public async Task UpdateUserRoleAsync_ThrowsException_WhenUserNotFound()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var users = new List<User>(); // Lista vazia, usuário não será encontrado

            var mockUserSet = users.BuildMockDbSet();
            mockUserSet.Setup(m => m.Find(It.IsAny<Guid>())).Returns((User)null); // Simula Find retornando null

            // --- CORREÇÃO AQUI ---
            var options = new DbContextOptionsBuilder<FcgDbContext>().Options;
            var mockContext = new Mock<FcgDbContext>(options); // Passando 'options'
            // --- FIM DA CORREÇÃO ---

            mockContext.Setup(c => c.Users).Returns(mockUserSet.Object);
            // Nao configuramos SaveChangesAsync porque a exceção deve ser lançada antes

            var repository = new UserRepository(mockContext.Object);
            var nonExistentUserId = Guid.NewGuid();

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => repository.UpdateUserRoleAsync(nonExistentUserId, "Admin"));
            mockContext.Verify(m => m.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never()); // Não deve chamar SaveChangesAsync
        }

        [Fact]
        public async Task UpdateUserProfileAsync_UpdatesNameAndEmailCorrectly()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var userEntity = new User { Id = userId, Name = "Old Name", Email = "old@example.com", PasswordHash = "hash6", Role = "Player", Library = new List<UserGaming>() };

            // Convert the list to an IQueryable for proper DbSet mocking
            var users = new List<User> { userEntity }.AsQueryable();

            // Mock the DbSet for LINQ operations
            var mockUserSet = new Mock<DbSet<User>>();
            mockUserSet.As<IQueryable<User>>().Setup(m => m.Provider).Returns(users.Provider);
            mockUserSet.As<IQueryable<User>>().Setup(m => m.Expression).Returns(users.Expression);
            mockUserSet.As<IQueryable<User>>().Setup(m => m.ElementType).Returns(users.ElementType);
            mockUserSet.As<IQueryable<User>>().Setup(m => m.GetEnumerator()).Returns(users.GetEnumerator());

            // If your repository uses FindAsync(Guid), you might also need this:
            mockUserSet.Setup(m => m.FindAsync(userId)).ReturnsAsync(userEntity);


            var options = new DbContextOptionsBuilder<FcgDbContext>().Options;
            var mockContext = new Mock<FcgDbContext>(options);

            mockContext.Setup(c => c.Users).Returns(mockUserSet.Object);
            mockContext.Setup(m => m.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

            var repository = new UserRepository(mockContext.Object);

            // No need for complex conversions if you're just updating properties of userEntity
            // You should create a Domain.User here for the *input* to the repository method.
            var updatedUserDomain = new Fcg.Domain.Entities.User(
                userId,
                "New Name", // Novo nome a ser testado
                "new@example.com", // Novo email a ser testado
                userEntity.PasswordHash, // Keep existing hash
                new List<Fcg.Domain.Entities.UserGaming>(), // Assuming library isn't being updated, or convert if needed
                userEntity.Role // Keep existing role
            );

            // Act
            await repository.UpdateUserProfileAsync(updatedUserDomain); // Assuming the method takes a Domain.User object

            // Assert
            mockContext.Verify(m => m.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once());
            Assert.Equal("New Name", userEntity.Name); // Check if the NAME of the mocked entity was updated
            Assert.Equal("new@example.com", userEntity.Email); // Check if the EMAIL of the mocked entity was updated
        }

        [Fact]
        public async Task UpdateUserProfileAsync_ThrowsException_WhenUserNotFound()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var users = new List<User>(); // Lista vazia

            var mockUserSet = users.BuildMockDbSet();
            mockUserSet.Setup(m => m.Find(It.IsAny<Guid>())).Returns((User)null);

            // --- CORREÇÃO AQUI ---
            var options = new DbContextOptionsBuilder<FcgDbContext>().Options;
            var mockContext = new Mock<FcgDbContext>(options); // Passando 'options'
            // --- FIM DA CORREÇÃO ---

            mockContext.Setup(c => c.Users).Returns(mockUserSet.Object);

            var repository = new UserRepository(mockContext.Object);
            var nonExistentUserDomain = new Fcg.Domain.Entities.User(Guid.NewGuid(), "Non Existent", "nonexistent@example.com", "hash", new List<Fcg.Domain.Entities.UserGaming>(), "Player");

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => repository.UpdateUserProfileAsync(nonExistentUserDomain));
            mockContext.Verify(m => m.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never());
        }
    }
}