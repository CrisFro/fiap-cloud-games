using Fcg.Api;
using Fcg.Application.Requests;
using Fcg.Tests;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using TechTalk.SpecFlow;
using Xunit;

namespace Fcg.Tests.StepDefinitions
{
    [Binding]
    public class LoginSteps : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _client;
        private HttpResponseMessage? _response;

        // Helper records to make deserializing responses cleaner
        private record LoginSuccessResponse(string Token, Guid UserId, string Email, string Message);
        private record LoginErrorResponse(string Message);

        public LoginSteps(CustomWebApplicationFactory factory)
        {
            _client = factory.CreateClient();
        }

        [Given(@"que um usuário registrado existe com o email ""(.*)"" e a senha ""(.*)""")]
        public async Task GivenARegisteredUserExistsWithEmailAndPassword(string email, string password)
        {
            // We use the user creation endpoint to ensure our test user exists.
            // This makes the test self-contained and independent.
            var createUserRequest = new CreateUserRequest
            {
                Name = "Login Test User",
                Email = email,
                Password = password
            };
            
            var creationResponse = await _client.PostAsJsonAsync("/api/users", createUserRequest);

            // The purpose of this step is to ensure the user exists.
            // A successful creation (2xx) is good.
            // A conflict (409) is also acceptable, as it means the user already exists.
            // Any other error status indicates a problem with the test setup.
            if (!creationResponse.IsSuccessStatusCode && creationResponse.StatusCode != HttpStatusCode.Conflict)
            {
                var errorContent = await creationResponse.Content.ReadAsStringAsync();
                Assert.Fail($"[GIVEN] Failed to ensure user exists. API returned {creationResponse.StatusCode}. Body: {errorContent}");
            }
        }

        [When(@"eu envio uma requisição de login com o email ""(.*)"" e a senha ""(.*)""")]
        public async Task WhenISendALoginRequestWithEmailAndPassword(string email, string password)
        {
            var loginRequest = new LoginRequest { Email = email, Password = password };
            _response = await _client.PostAsJsonAsync("/api/login", loginRequest);
        }

        [Then(@"a API deve responder com o status '([^']*)'")]
        public void ThenTheAPIShouldRespondWithAnStatus(string expectedStatus)
        {
            // This step is reusable across different feature tests.
            var expectedStatusCode = Enum.Parse<HttpStatusCode>(expectedStatus, true);
            Assert.Equal(expectedStatusCode, _response!.StatusCode);
        }

        [Then(@"a resposta deve conter um token JWT válido")]
        public async Task ThenTheResponseShouldContainAValidJWTToken()
        {
            Assert.NotNull(_response);
            LoginSuccessResponse? loginResponse = null;
            try
            {
                loginResponse = await _response.Content.ReadFromJsonAsync<LoginSuccessResponse>();
            }
            catch (System.Text.Json.JsonException ex)
            {
                var body = await _response.Content.ReadAsStringAsync();
                Assert.Fail($"Failed to deserialize successful login response. Status: {_response.StatusCode}. Body: '{body}'. Exception: {ex.Message}");
            }

            Assert.NotNull(loginResponse);
            Assert.False(string.IsNullOrWhiteSpace(loginResponse.Token));
        }

        [Then(@"a resposta da mensagem deve indicar credenciais inválidas")]
        public async Task ThenTheResponseMessageShouldIndicateInvalidCredentials()
        {
            Assert.NotNull(_response);
            LoginErrorResponse? errorResponse = null;
            try
            {
                errorResponse = await _response.Content.ReadFromJsonAsync<LoginErrorResponse>();
            }
            catch (System.Text.Json.JsonException ex)
            {
                var body = await _response.Content.ReadAsStringAsync();
                Assert.Fail($"Failed to deserialize error login response. Status: {_response.StatusCode}. Body: '{body}'. Exception: {ex.Message}");
            }

            Assert.NotNull(errorResponse);
            Assert.Equal("Usuário ou senha inválidos.", errorResponse.Message);
        }
    }
}