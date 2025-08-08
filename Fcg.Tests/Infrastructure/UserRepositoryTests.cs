using Fcg.Domain.Entities;
using Fcg.Infrastructure.Data;
using Fcg.Infrastructure.Repositories;
using Fcg.Infrastructure.Tests.Mocks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Moq;
using System.Linq.Expressions;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Game = Fcg.Infrastructure.Tables.Game;
using User = Fcg.Infrastructure.Tables.User;
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
        public async Task GetUserByEmailAsync_ReturnsUser_WhenEmailExists_InMemory()
        {
            // Arrange
            var game1 = new Game { Id = Guid.NewGuid(), Title = "Game 1", Description = "D1", Genre = 1, Price = 10m, CreatedAt = DateTime.UtcNow };
            var game2 = new Game { Id = Guid.NewGuid(), Title = "Game 2", Description = "D2", Genre = 2, Price = 20m, CreatedAt = DateTime.UtcNow };

            var userGaming1 = new UserGaming { Id = Guid.NewGuid(), GameId = game1.Id, Game = game1, PurchasedDate = DateTime.UtcNow };
            var userGaming2 = new UserGaming { Id = Guid.NewGuid(), GameId = game2.Id, Game = game2, PurchasedDate = DateTime.UtcNow };

            var existingUser = new User
            {
                Id = Guid.NewGuid(),
                Name = "Existing User",
                Email = "existing@example.com",
                PasswordHash = "hash1",
                Role = "Player",
                // A biblioteca será populada quando o usuário for adicionado ao contexto
                Library = new List<UserGaming> { userGaming1, userGaming2 }
            };
            // Não é necessário setar userGaming1.User = existingUser; e userGaming2.User = existingUser;
            // O EF Core In-Memory se encarregará disso quando as entidades forem adicionadas e salvas.

            // 1. Configurar as opções do DbContext para usar o provedor em memória
            var options = new DbContextOptionsBuilder<FcgDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // Nome único para isolamento de teste
                .Options;

            // 2. Usar um bloco 'using' para garantir que o contexto seja descartado
            // e popular o banco de dados em memória com os dados necessários
            using (var context = new FcgDbContext(options))
            {
                // Adicione as entidades principais e as relacionadas
                context.Games.Add(game1);
                context.Games.Add(game2);
                context.Users.Add(existingUser);
                // Se as UserGaming não forem adicionadas automaticamente via navegação, adicione-as explicitamente
                context.UserGamings.Add(userGaming1);
                context.UserGamings.Add(userGaming2);

                await context.SaveChangesAsync(); // Persiste os dados no banco de dados em memória
            }

            // 3. Criar um NOVO contexto para a fase Act (o teste de fato)
            using (var context = new FcgDbContext(options))
            {
                var repository = new UserRepository(context);

                // Act
                var result = await repository.GetUserByEmailAsync("existing@example.com");

                // Assert
                Assert.NotNull(result);
                Assert.Equal(existingUser.Id, result.Id);
                Assert.Equal(existingUser.Email, result.Email);
                Assert.NotNull(result.Library);
                Assert.Equal(2, result.Library.Count());
                Assert.Contains(result.Library, ug => ug.Game.Title == "Game 1");
                Assert.Contains(result.Library, ug => ug.Game.Title == "Game 2"); // Adicionado para cobrir game2
            }
        }

        [Fact]
        public async Task GetUserByEmailAsync_ReturnsNull_WhenEmailDoesNotExist_InMemory()
        {
            // Arrange
            var nonExistentEmail = "nonexistent@example.com";

            // Configurar as opções do DbContext para usar o provedor em memória
            // Um nome de banco de dados único garante isolamento entre os testes
            var options = new DbContextOptionsBuilder<FcgDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            // Para este teste, o banco de dados em memória deve estar vazio
            // ou não conter o email que estamos procurando.
            // O bloco 'using' cria e garante o descarte do contexto.
            using (var context = new FcgDbContext(options))
            {
                // Não é necessário adicionar nenhum dado, pois estamos testando a ausência.
                // Se você tivesse outros dados no DB para outros testes no mesmo DbContext,
                // certifique-se de que o email "nonexistent@example.com" não esteja lá.
                // Como estamos usando Guid.NewGuid().ToString() para o databaseName,
                // cada teste terá um banco de dados em memória isolado e vazio por padrão.

                var repository = new UserRepository(context);

                // Act
                var result = await repository.GetUserByEmailAsync(nonExistentEmail);

                // Assert
                Assert.Null(result);
            }
        }

        [Fact]
        public async Task GetUserByIdAsync_ReturnsUser_WhenIdExists_InMemory()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var game1 = new Game { Id = Guid.NewGuid(), Title = "Game X", Description = "DX", Genre = 1, Price = 10m, CreatedAt = DateTime.UtcNow };
            var userGaming1 = new UserGaming { Id = Guid.NewGuid(), GameId = game1.Id, Game = game1, PurchasedDate = DateTime.UtcNow };

            var existingUser = new User
            {
                Id = userId,
                Name = "User By Id",
                Email = "id@example.com",
                PasswordHash = "hash2",
                Role = "Admin",
                Library = new List<UserGaming> { userGaming1 } // A Library agora será populada pelo In-Memory DB
            };
            // Com In-Memory, você não precisa setar a referência inversa userGaming1.User = existingUser;
            // o EF Core fará isso para você ao persistir.

            // 1. Configurar as opções do DbContext para usar o provedor em memória
            // Um nome de banco de dados único garante isolamento entre os testes
            var options = new DbContextOptionsBuilder<FcgDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            // 2. Usar um bloco 'using' para garantir que o contexto seja descartado
            // e popular o banco de dados em memória
            using (var context = new FcgDbContext(options))
            {
                context.Users.Add(existingUser);
                context.Games.Add(game1); // Certifique-se de adicionar também as entidades Game se elas forem novas e não existirem no DB
                context.UserGamings.Add(userGaming1); // Adicione a entrada UserGaming também, se ela não for adicionada via User.Library automaticamente.

                // Salve as mudanças para que os dados existam no DB em memória
                await context.SaveChangesAsync();
            }

            // 3. Criar um NOVO contexto para a execução da lógica do repositório
            // Isso simula o comportamento real de obter um contexto fresco para cada operação
            using (var context = new FcgDbContext(options))
            {
                var repository = new UserRepository(context);

                // Act
                var result = await repository.GetUserByIdAsync(userId);

                // Assert
                Assert.NotNull(result);
                Assert.Equal(userId, result.Id);
                Assert.Equal(existingUser.Name, result.Name);
                Assert.NotNull(result.Library);
                Assert.Single(result.Library);
                // Certifique-se de que o Game dentro de UserGaming está populado
                Assert.Equal("Game X", result.Library.First().Game.Title);
            }
        }








        public class TestAsyncQueryProvider<TEntity> : IAsyncQueryProvider
        {
            private readonly IQueryProvider _inner;

            internal TestAsyncQueryProvider(IQueryProvider inner)
            {
                _inner = inner;
            }

            public IQueryable CreateQuery(Expression expression)
            {
                return new TestAsyncEnumerable<TEntity>(expression);
            }

            public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
            {
                return new TestAsyncEnumerable<TElement>(expression);
            }

            public object Execute(Expression expression)
            {
                return _inner.Execute(expression);
            }

            public TResult Execute<TResult>(Expression expression)
            {
                return _inner.Execute<TResult>(expression);
            }

            public TResult ExecuteAsync<TResult>(Expression expression, CancellationToken cancellationToken = default)
            {
                // Tenta obter o resultado de forma síncrona, e envolve em Task.FromResult se necessário
                var result = Execute<TResult>(expression);
                var expectedResultType = typeof(Task<TResult>);

                // Se o resultado já é um Task, retorne-o diretamente.
                // Isso é importante para cenários onde o EF Core pode otimizar e retornar um Task já concluído.
                if (expectedResultType.IsAssignableFrom(result.GetType()))
                {
                    return result;
                }
                // Caso contrário, envolve o resultado síncrono em um Task para simular um método assíncrono.
                return (TResult)(object)Task.FromResult(result);
            }
        }
        public class TestAsyncEnumerable<T> : EnumerableQuery<T>, IAsyncEnumerable<T>, IQueryable<T>
        {
            public TestAsyncEnumerable(IEnumerable<T> enumerable) : base(enumerable) { }
            public TestAsyncEnumerable(Expression expression) : base(expression) { }

            public IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = default)
            {
                return new TestAsyncEnumerator<T>(this.AsEnumerable().GetEnumerator());
            }
        }
        public class TestAsyncEnumerator<T> : IAsyncEnumerator<T>
        {
            private readonly IEnumerator<T> _inner;

            public TestAsyncEnumerator(IEnumerator<T> inner)
            {
                _inner = inner;
            }

            public T Current => _inner.Current;

            public ValueTask DisposeAsync()
            {
                _inner.Dispose();
                return new ValueTask();
            }

            public ValueTask<bool> MoveNextAsync()
            {
                return new ValueTask<bool>(_inner.MoveNext());
            }
        }

        [Fact]
        public async Task GetUserByIdAsync_ReturnsNull_WhenIdDoesNotExist_InMemory()
        {
            // Arrange
            var nonExistentId = Guid.NewGuid();

            // Configurar as opções do DbContext para usar o provedor em memória
            // Um nome de banco de dados único garante isolamento entre os testes
            var options = new DbContextOptionsBuilder<FcgDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            // Não precisamos popular o banco de dados para este teste,
            // pois queremos que ele esteja vazio para garantir que o usuário não exista.
            // O bloco 'using' aqui garante que o contexto é criado e descartado corretamente.
            using (var context = new FcgDbContext(options))
            {
                // O repositório opera em um contexto vazio (ou seja, sem o ID que estamos procurando)
                var repository = new UserRepository(context);

                // Act
                var result = await repository.GetUserByIdAsync(nonExistentId);

                // Assert
                Assert.Null(result);
            }
        }

        [Fact]
        public async Task UpdateUserLibraryAsync_AddsGamesToLibrary()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var userEntity = new User { Id = userId, Name = "User With Library", Email = "lib@example.com", PasswordHash = "hash3", Role = "Player", Library = new List<UserGaming>() };
            var gamesData = new List<Game>
            {
                new Game { Id = Guid.NewGuid(), Title = "Game 1", Description = "D1", Genre = 1, Price = 10m, CreatedAt = DateTime.UtcNow },
                new Game { Id = Guid.NewGuid(), Title = "Game 2", Description = "D2", Genre = 2, Price = 20m, CreatedAt = DateTime.UtcNow }
            };

            var users = new List<User> { userEntity };
            var userGamingsInDb = new List<UserGaming>(); // Simula o estado inicial do DB

            var mockUserSet = users.BuildMockDbSet();
            var mockUserGamingSet = userGamingsInDb.BuildMockDbSet();

            // Simula AddRange para UserGamings
            mockUserGamingSet.Setup(m => m.AddRange(It.IsAny<IEnumerable<UserGaming>>()))
                .Callback<IEnumerable<UserGaming>>(newItems => userGamingsInDb.AddRange(newItems));

            var options = new DbContextOptionsBuilder<FcgDbContext>().Options;
            var mockContext = new Mock<FcgDbContext>(options);

            mockContext.Setup(c => c.Users).Returns(mockUserSet.Object);
            mockContext.Setup(c => c.UserGamings).Returns(mockUserGamingSet.Object);
            mockContext.Setup(m => m.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(It.IsAny<int>());

            var repository = new UserRepository(mockContext.Object);

            // Instancia o domainUser com os dados do userEntity para que o repositório o encontre.
            var domainUser = new Fcg.Domain.Entities.User(userId, userEntity.Name, userEntity.Email, userEntity.PasswordHash, new List<Fcg.Domain.Entities.UserGaming>(), userEntity.Role);

            domainUser.AddGameToLibrary(new Fcg.Domain.Entities.Game(gamesData[0].Id, gamesData[0].Title, gamesData[0].Description, (GenreEnum)gamesData[0].Genre, gamesData[0].Price, gamesData[0].CreatedAt));
            domainUser.AddGameToLibrary(new Fcg.Domain.Entities.Game(gamesData[1].Id, gamesData[1].Title, gamesData[1].Description, (GenreEnum)gamesData[1].Genre, gamesData[1].Price, gamesData[1].CreatedAt));

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
            var gameExisting = new Game { Id = Guid.NewGuid(), Title = "Game Existing", Description = "DE", Genre = (int)GenreEnum.Outro, Price = 10m, CreatedAt = DateTime.UtcNow };
            var gameToAdd = new Game { Id = Guid.NewGuid(), Title = "Game To Add", Description = "DTA", Genre = (int)GenreEnum.Outro, Price = 20m, CreatedAt = DateTime.UtcNow };
            var gameToRemove = new Game { Id = Guid.NewGuid(), Title = "Game To Remove", Description = "DTR", Genre = (int)GenreEnum.Outro, Price = 30m, CreatedAt = DateTime.UtcNow };

            var userGamingExisting = new UserGaming { Id = Guid.NewGuid(), UserId = userId, GameId = gameExisting.Id, Game = gameExisting, PurchasedDate = DateTime.UtcNow };
            var userGamingToRemove = new UserGaming { Id = Guid.NewGuid(), UserId = userId, GameId = gameToRemove.Id, Game = gameToRemove, PurchasedDate = DateTime.UtcNow };

            var userGamingsInDb = new List<UserGaming> { userGamingExisting, userGamingToRemove }; // Simula o estado inicial do DB

            // O userEntity deve ter a biblioteca inicial para que o repositório possa comparar
            var userEntity = new User { Id = userId, Name = "User Mixed Ops", Email = "mixed@example.com", PasswordHash = "hash4", Role = "Player", Library = userGamingsInDb };
            var users = new List<User> { userEntity };
            var gamesData = new List<Game> { gameExisting, gameToAdd, gameToRemove }; // Dados para jogos


            var mockUserSet = users.BuildMockDbSet();
            var mockUserGamingSet = userGamingsInDb.BuildMockDbSet();

            mockUserGamingSet.Setup(m => m.AddRange(It.IsAny<IEnumerable<UserGaming>>()))
                .Callback<IEnumerable<UserGaming>>(newItems => userGamingsInDb.AddRange(newItems));
            mockUserGamingSet.Setup(m => m.RemoveRange(It.IsAny<IEnumerable<UserGaming>>()))
                .Callback<IEnumerable<UserGaming>>(itemsToRemove => userGamingsInDb.RemoveAll(ug => itemsToRemove.Any(r => r.Id == ug.Id)));

            var options = new DbContextOptionsBuilder<FcgDbContext>().Options;
            var mockContext = new Mock<FcgDbContext>(options);

            mockContext.Setup(c => c.Users).Returns(mockUserSet.Object);
            mockContext.Setup(c => c.Games).Returns(gamesData.BuildMockDbSet().Object); // Se o repositório usar Games diretamente, mocke
            mockContext.Setup(c => c.UserGamings).Returns(mockUserGamingSet.Object);
            mockContext.Setup(m => m.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(It.IsAny<int>());

            var repository = new UserRepository(mockContext.Object);

            // Criar o domainUser com o estado atual da biblioteca para simular as operações de domínio
            // Primeiro, crie a instância do User de domínio
            var domainUser = new Fcg.Domain.Entities.User(
                userId,
                userEntity.Name,
                userEntity.Email,
                userEntity.PasswordHash,
                new List<Fcg.Domain.Entities.UserGaming>(), // Comece com uma lista vazia aqui, os jogos serão adicionados abaixo
                userEntity.Role
            );

            // Agora, adicione os jogos existentes à biblioteca do domainUser, garantindo que o UserGaming
            // tenha a referência correta ao domainUser.
            foreach (var ugInDb in userGamingsInDb)
            {
                // Certifique-se de que o Game de domínio também está correto
                var domainGame = new Fcg.Domain.Entities.Game(ugInDb.Game.Id, ugInDb.Game.Title, ugInDb.Game.Description, (GenreEnum)ugInDb.Game.Genre, ugInDb.Game.Price, ugInDb.Game.CreatedAt);

                // Use o construtor que aceita User e Game, ou o construtor com ID e o próprio domainUser
                var domainUserGaming = new Fcg.Domain.Entities.UserGaming(ugInDb.Id, domainUser, domainGame, ugInDb.PurchasedDate);

                // Adicione diretamente à lista interna (_library) da entidade User para simular o estado inicial
                // Isso é um pouco mais "hacky" para o teste, pois simula o carregamento do banco de dados
                // mas é necessário porque AddGameToLibrary() tem a lógica de "já existe".
                // Uma alternativa mais limpa seria se a entidade User tivesse um construtor de "rehidratação"
                // que aceita a lista de UserGaming já populada.
                typeof(Fcg.Domain.Entities.User)
                    .GetField("_library", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                    .SetValue(domainUser, userGamingsInDb.Select(ug => new Fcg.Domain.Entities.UserGaming(ug.Id, domainUser,
                        new Fcg.Domain.Entities.Game(ug.Game.Id, ug.Game.Title, ug.Game.Description, (GenreEnum)ug.Game.Genre, ug.Game.Price, ug.Game.CreatedAt), ug.PurchasedDate)).ToList());
            }


            // Simula operações no domínio
            domainUser.AddGameToLibrary(new Fcg.Domain.Entities.Game(gameToAdd.Id, gameToAdd.Title, gameToAdd.Description, (GenreEnum)gameToAdd.Genre, gameToAdd.Price, gameToAdd.CreatedAt));
            domainUser.RemoveGameFromLibrary(gameToRemove.Id); // Passando o Guid do Game

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

            var users = new List<User> { userEntity }.AsQueryable();

            var mockUserSet = new Mock<DbSet<User>>();

            mockUserSet.As<IQueryable<User>>().Setup(m => m.Provider).Returns(users.Provider);
            mockUserSet.As<IQueryable<User>>().Setup(m => m.Expression).Returns(users.Expression);
            mockUserSet.As<IQueryable<User>>().Setup(m => m.ElementType).Returns(users.ElementType);
            mockUserSet.As<IQueryable<User>>().Setup(m => m.GetEnumerator()).Returns(users.GetEnumerator());

            // CORREÇÃO PRINCIPAL: MOCKANDO FindAsync DIRETAMENTE
            mockUserSet.Setup(m => m.FindAsync(userId)).ReturnsAsync(userEntity);

            var options = new DbContextOptionsBuilder<FcgDbContext>().Options;
            var mockContext = new Mock<FcgDbContext>(options);

            mockContext.Setup(c => c.Users).Returns(mockUserSet.Object);
            mockContext.Setup(m => m.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

            var repository = new UserRepository(mockContext.Object);
            var newRole = "Admin";

            // Act
            await repository.UpdateUserRoleAsync(userId, newRole);

            // Assert
            mockContext.Verify(m => m.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once());
            mockUserSet.Verify(m => m.FindAsync(userId), Times.Once());
            Assert.Equal(newRole, userEntity.Role);
        }

        [Fact]
        public async Task UpdateUserRoleAsync_ThrowsException_WhenUserNotFound()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var users = new List<User>(); // Lista vazia, usuário não será encontrado

            var mockUserSet = users.BuildMockDbSet();
            // Simula FindAsync retornando null (ou Find se o repositório usar Find)
            mockUserSet.Setup(m => m.FindAsync(It.IsAny<object[]>())).ReturnsAsync((User)null);
            // Se o seu repositório usa .Find(), use:
            // mockUserSet.Setup(m => m.Find(It.IsAny<Guid>())).Returns((User)null);


            var options = new DbContextOptionsBuilder<FcgDbContext>().Options;
            var mockContext = new Mock<FcgDbContext>(options);

            mockContext.Setup(c => c.Users).Returns(mockUserSet.Object);

            var repository = new UserRepository(mockContext.Object);
            var nonExistentUserId = Guid.NewGuid();

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => repository.UpdateUserRoleAsync(nonExistentUserId, "Admin"));
            mockContext.Verify(m => m.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never());
        }

        [Fact]
        public async Task UpdateUserProfileAsync_UpdatesNameAndEmailCorrectly()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var userEntity = new User { Id = userId, Name = "Old Name", Email = "old@example.com", PasswordHash = "hash6", Role = "Player", Library = new List<UserGaming>() };

            var users = new List<User> { userEntity }.AsQueryable();

            var mockUserSet = new Mock<DbSet<User>>();
            mockUserSet.As<IQueryable<User>>().Setup(m => m.Provider).Returns(users.Provider);
            mockUserSet.As<IQueryable<User>>().Setup(m => m.Expression).Returns(users.Expression);
            mockUserSet.As<IQueryable<User>>().Setup(m => m.ElementType).Returns(users.ElementType);
            mockUserSet.As<IQueryable<User>>().Setup(m => m.GetEnumerator()).Returns(users.GetEnumerator());

            // Se seu repositório usa FindAsync(Guid), isso é necessário:
            mockUserSet.Setup(m => m.FindAsync(userId)).ReturnsAsync(userEntity);

            var options = new DbContextOptionsBuilder<FcgDbContext>().Options;
            var mockContext = new Mock<FcgDbContext>(options);

            mockContext.Setup(c => c.Users).Returns(mockUserSet.Object);
            mockContext.Setup(m => m.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

            var repository = new UserRepository(mockContext.Object);

            // Objeto de domínio com as novas informações para atualização
            var updatedUserDomain = new Fcg.Domain.Entities.User(
                userId,
                "New Name",
                "new@example.com",
                userEntity.PasswordHash,
                new List<Fcg.Domain.Entities.UserGaming>(),
                userEntity.Role
            );

            // Act
            await repository.UpdateUserProfileAsync(updatedUserDomain);

            // Assert
            mockContext.Verify(m => m.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once());
            Assert.Equal("New Name", userEntity.Name);
            Assert.Equal("new@example.com", userEntity.Email);
        }

        [Fact]
        public async Task UpdateUserProfileAsync_ThrowsException_WhenUserNotFound()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var users = new List<User>(); // Lista vazia

            var mockUserSet = users.BuildMockDbSet();
            // Simula FindAsync retornando null
            mockUserSet.Setup(m => m.FindAsync(It.IsAny<object[]>())).ReturnsAsync((User)null);
            // Se o seu repositório usa .Find(), use:
            // mockUserSet.Setup(m => m.Find(It.IsAny<Guid>())).Returns((User)null);


            var options = new DbContextOptionsBuilder<FcgDbContext>().Options;
            var mockContext = new Mock<FcgDbContext>(options);

            mockContext.Setup(c => c.Users).Returns(mockUserSet.Object);

            var repository = new UserRepository(mockContext.Object);
            var nonExistentUserDomain = new Fcg.Domain.Entities.User(Guid.NewGuid(), "Non Existent", "nonexistent@example.com", "hash", new List<Fcg.Domain.Entities.UserGaming>(), "Player");

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => repository.UpdateUserProfileAsync(nonExistentUserDomain));
            mockContext.Verify(m => m.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never());
        }
    }
}