using AutoFixture;
using FluentAssertions;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using VacationRental.Data.Models;
using VacationRental.Services.Models;
using Xunit;

namespace VacationRental.Api.Tests
{
    [Collection("Integration")]
    public class RentalTests
    {
        private readonly HttpClient _client;
        private readonly Fixture _fixture;
        

        public RentalTests(IntegrationFixture integrationFixture)
        {
            _client = integrationFixture.Client;
            _fixture = new Fixture();
        }

        [Fact]
        public async Task GetRental_ItemDoesNotExist()
        {
            // Arrange
            var rentalId = _fixture.Create<int>();

            // Act
            var actualResponse = await _client.GetAsync($"{ApiConstants.RENTAL_URI}{rentalId}");
            var actualContent = await actualResponse.Content.ReadAsStringAsync();

            // Assert
            actualResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            actualContent.Should().Be("Rental not found");
        }

        [Fact] 
        public async Task GetRental_ItemDoesExist()
        {
            // Arrange
            var rental = _fixture.Create<RentalBindingModel>();

            // Act
            using var postResponse = await _client.PostAsJsonAsync(ApiConstants.RENTAL_URI, rental);
            var postResult = await postResponse.Content.ReadAsAsync<ResourceIdViewModel>();

            var actualResponse = await _client.GetAsync($"{ApiConstants.RENTAL_URI}{postResult.Id}");
            var actualContent = await actualResponse.Content.ReadAsAsync<RentalViewModel>();

            // Assert
            actualResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            actualContent.Should().BeEquivalentTo(rental);
        }

        [Fact]
        public async Task CreateRentals_NightMustBePositive()
        {
            // Arrange
            var rental = _fixture
                            .Build<RentalBindingModel>()
                            .With(x => x.Units, 0)
                            .Create();

            // Act
            using var actualResponse = await _client.PostAsJsonAsync(ApiConstants.RENTAL_URI, rental);
            var actualContent = await actualResponse.Content.ReadAsStringAsync();

            // Assert
            actualResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            actualContent.Should().BeEquivalentTo("Nights must be positive");
        }

        [Fact]
        public async Task CreateRentals_PreparationTimeMustBePositive()
        {
            // Arrange
            var rental = _fixture
                            .Build<RentalBindingModel>()
                            .With(x => x.PreparationTimeInDays, 0)
                            .Create();

            // Act
            using var actualResponse = await _client.PostAsJsonAsync(ApiConstants.RENTAL_URI, rental);
            var actualContent = await actualResponse.Content.ReadAsStringAsync();

            // Assert
            actualResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            actualContent.Should().BeEquivalentTo("Preparation time must be positive");
        }

        [Fact]
        public async Task UpdateRentals_PositiveResult()
        {
            // Arrange
            var postRental = _fixture
                            .Create<RentalBindingModel>();

            var updateRental = _fixture
                            .Build<RentalViewModel>()
                            .Without(x => x.Id)
                            .Create();

            // Act
            using var postResponse = await _client.PostAsJsonAsync(ApiConstants.RENTAL_URI, postRental);
            var resourceId = await postResponse.Content.ReadAsAsync<ResourceIdViewModel>();
            updateRental.Id = resourceId.Id;

            using var updateResponse = await _client.PutAsJsonAsync(ApiConstants.RENTAL_URI, updateRental);

            using var getResponse = await _client.GetAsync($"{ApiConstants.RENTAL_URI}{updateRental.Id}");
            var getContent = await getResponse.Content.ReadAsAsync<RentalViewModel>();

            // Assert
            postResponse.StatusCode.Should().Be(HttpStatusCode.Created);
            updateResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            getContent.Should().BeEquivalentTo(updateRental);
        }

        [Fact]
        public async Task UpdateRentals_NightMustBePositive()
        {
            // Arrange
            var postRental = _fixture
                            .Create<RentalBindingModel>();

            var updateRental = _fixture
                            .Build<RentalViewModel>()
                            .Without(x => x.Id)
                            .Without(x => x.Units)
                            .Create();

            // Act
            using var postResponse = await _client.PostAsJsonAsync(ApiConstants.RENTAL_URI, postRental);
            var resourceId = await postResponse.Content.ReadAsAsync<ResourceIdViewModel>();
            updateRental.Id = resourceId.Id;

            using var updateResponse = await _client.PutAsJsonAsync(ApiConstants.RENTAL_URI, updateRental);
            var updateContent = await updateResponse.Content.ReadAsStringAsync();

            // Assert
            postResponse.StatusCode.Should().Be(HttpStatusCode.Created);
            updateResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            updateContent.Should().BeEquivalentTo("Nights must be positive");
        }

        [Fact]
        public async Task UpdateRentals_PreparationTimeMustBePositive()
        {
            // Arrange
            var postRental = _fixture
                            .Create<RentalBindingModel>();

            var updateRental = _fixture
                            .Build<RentalViewModel>()
                            .Without(x => x.Id)
                            .Without(x => x.PreparationTimeInDays)
                            .Create();

            // Act
            using var postResponse = await _client.PostAsJsonAsync(ApiConstants.RENTAL_URI, postRental);
            var resourceId = await postResponse.Content.ReadAsAsync<ResourceIdViewModel>();
            updateRental.Id = resourceId.Id;

            using var updateResponse = await _client.PutAsJsonAsync(ApiConstants.RENTAL_URI, updateRental);
            var updateContent = await updateResponse.Content.ReadAsStringAsync();

            // Assert
            postResponse.StatusCode.Should().Be(HttpStatusCode.Created);
            updateResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            updateContent.Should().BeEquivalentTo("Preparation time must be positive");
        }

        [Fact]
        public async Task GivenCompleteRequest_WhenPostRental_ThenAGetReturnsTheCreatedRental()
        {
            var request = new RentalBindingModel
            {
                Units = 25,
                PreparationTimeInDays = 1
            };

            ResourceIdViewModel postResult;
            using (var postResponse = await _client.PostAsJsonAsync(ApiConstants.RENTAL_URI, request))
            {
                Assert.True(postResponse.IsSuccessStatusCode);
                postResult = await postResponse.Content.ReadAsAsync<ResourceIdViewModel>();
            }

            using (var getResponse = await _client.GetAsync($"{ApiConstants.RENTAL_URI}{postResult.Id}"))
            {
                Assert.True(getResponse.IsSuccessStatusCode);

                var getResult = await getResponse.Content.ReadAsAsync<RentalViewModel>();
                Assert.Equal(request.Units, getResult.Units);
            }
        }
    }
}
