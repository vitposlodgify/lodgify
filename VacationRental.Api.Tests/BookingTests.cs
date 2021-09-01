using AutoFixture;
using FluentAssertions;
using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using VacationRental.Data.Models;
using VacationRental.Services.Models;
using Xunit;

namespace VacationRental.Api.Tests
{
    [Collection("Integration")]
    public class BookingTests
    {
        private readonly HttpClient _client;
        private readonly Fixture _fixture;

        public BookingTests(IntegrationFixture fixture)
        {
            _client = fixture.Client;
            _fixture = new Fixture();
        }

        [Fact]
        public async Task CreateBookingAsync_NigtsMustBePositive()
        {
            // Arrage
            var postRentalRequest = _fixture.Create<RentalBindingModel>();
            var postBookingRequest = _fixture
                                        .Build<BookingBindingModel>()
                                        .Without(x => x.RentalId)
                                        .Without(x => x.Nights)
                                        .Create();

            using var postRentalResponse = await _client.PostAsJsonAsync(ApiConstants.RENTAL_URI, postRentalRequest);
            var postRentalResult = await postRentalResponse.Content.ReadAsAsync<ResourceIdViewModel>();

            postBookingRequest.RentalId = postRentalResult.Id;

            // Act
            var postBookingResponse = await _client.PostAsJsonAsync(ApiConstants.BOOKING_URI, postBookingRequest);
            var postBookingContent = await postBookingResponse.Content.ReadAsStringAsync();

            // Assert
            postRentalResponse.StatusCode.Should().Be(HttpStatusCode.Created);
            postBookingResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            postBookingContent.Should().Be("Nigts must be positive");
        }

        [Fact]
        public async Task CreateBookingAsync_RentalNotFound()
        {
            // Arrage
            var postBookingRequest = _fixture
                                        .Build<BookingBindingModel>()
                                        .Create();

            // Act
            var postBookingResponse = await _client.PostAsJsonAsync(ApiConstants.BOOKING_URI, postBookingRequest);
            var postBookingContent = await postBookingResponse.Content.ReadAsStringAsync();

            // Assert
            postBookingResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            postBookingContent.Should().Be("Rental not found");
        }

        [Fact]
        public async Task GivenCompleteRequest_WhenPostBooking_ThenAGetReturnsTheCreatedBooking()
        {
            var postRentalRequest = new RentalBindingModel
            {
                Units = 4,
                PreparationTimeInDays = 1
            };

            ResourceIdViewModel postRentalResult;
            using (var postRentalResponse = await _client.PostAsJsonAsync(ApiConstants.RENTAL_URI, postRentalRequest))
            {
                Assert.True(postRentalResponse.IsSuccessStatusCode);
                postRentalResult = await postRentalResponse.Content.ReadAsAsync<ResourceIdViewModel>();
            }

            var postBookingRequest = new BookingBindingModel
            {
                RentalId = postRentalResult.Id,
                Nights = 3,
                Start = new DateTime(2001, 01, 01)
            };

            ResourceIdViewModel postBookingResult;
            using (var postBookingResponse = await _client.PostAsJsonAsync(ApiConstants.BOOKING_URI, postBookingRequest))
            {
                Assert.True(postBookingResponse.IsSuccessStatusCode);
                postBookingResult = await postBookingResponse.Content.ReadAsAsync<ResourceIdViewModel>();
            }

            using (var getBookingResponse = await _client.GetAsync($"{ApiConstants.BOOKING_URI}{postBookingResult.Id}"))
            {
                Assert.True(getBookingResponse.IsSuccessStatusCode);

                var getBookingResult = await getBookingResponse.Content.ReadAsAsync<BookingViewModel>();
                Assert.Equal(postBookingRequest.RentalId, getBookingResult.RentalId);
                Assert.Equal(postBookingRequest.Nights, getBookingResult.Nights);
                Assert.Equal(postBookingRequest.Start, getBookingResult.Start);
            }
        }

        [Fact]
        public async Task GivenCompleteRequest_WhenPostBooking_ThenAPostReturnsErrorWhenThereIsOverbooking()
        {
            var postRentalRequest = new RentalBindingModel
            {
                Units = 1,
                PreparationTimeInDays = 3
            };

            ResourceIdViewModel postRentalResult;
            using (var postRentalResponse = await _client.PostAsJsonAsync(ApiConstants.RENTAL_URI, postRentalRequest))
            {
                Assert.True(postRentalResponse.IsSuccessStatusCode);
                postRentalResult = await postRentalResponse.Content.ReadAsAsync<ResourceIdViewModel>();
            }

            var postBooking1Request = new BookingBindingModel
            {
                RentalId = postRentalResult.Id,
                Nights = 3,
                Start = new DateTime(2002, 01, 01)
            };

            using (var postBooking1Response = await _client.PostAsJsonAsync(ApiConstants.BOOKING_URI, postBooking1Request))
            {
                Assert.True(postBooking1Response.IsSuccessStatusCode);
            }

            var postBooking2Request = new BookingBindingModel
            {
                RentalId = postRentalResult.Id,
                Nights = 1,
                Start = new DateTime(2002, 01, 02)
            };

            using (var postBooking2Response = await _client.PostAsJsonAsync(ApiConstants.BOOKING_URI, postBooking2Request))
            {
                Assert.True(!postBooking2Response.IsSuccessStatusCode);
                Assert.True(postBooking2Response.StatusCode == HttpStatusCode.BadRequest);
                var message = await postBooking2Response.Content.ReadAsStringAsync();
                Assert.True(message.Equals("Not available"));
            }
        }
    }
}
