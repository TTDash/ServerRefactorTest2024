using Moq;
using NUnit.Framework;
using NSubstitute;
using Xunit;
using HotelReservationLibrary.DomainObjects;
using FluentAssertions;

namespace HotelReservationLibrary.Tests
{
    // TODO: Add tests for the ReservationService class
    // Some nuget packages have already been installed for you but feel free to use your preferred testing framework
    // - NUnit
    // - Moq
    // - FluentAssertions
    public class ReservationServiceTests
    {
        private Mock<IReservationHandler> _reservationDb;
        private ReservationService _reservationService;

        [SetUp]
        public void Setup()
        {
            //Using NSubstitute, we can swamp out ReservationDB.ReservationHandler for our mock object
            ReservationDb.ReservationHandler = Substitute.For<IReservationHandler>(); // NSubstitute

            _reservationDb = new Mock<IReservationHandler>();
            _reservationDb.Setup(db => db.AddReservation(It.IsAny<Reservation>())).Returns(DateTime.Now.Ticks);
            ReservationDb.ReservationHandler = _reservationDb.Object;

            _reservationService = new ReservationService();
        }

        /// <summary>
        /// Saves a valid reservation via mock.  Since it is valid, we should get a non-zero 
        /// result.
        /// </summary>
        [Test]
        public void AddReservation_Test()
        {
            //Arrange
            var reservation = new Reservation()
            {
                GuestFirstName = "Bobby",
                GuestLastName = "Tables",
                guestEmail = "wearehiring@teachtown.com",
                CheckInDate = new DateTime(2022, 1, 1),
                CheckOutDate = new DateTime(2022, 1, 8),
                NumberOfAdditionalGuests = 1,
                RoomType = "Single",
                SmokingOrNonSmoking = "Non-Smoking"
            };

            // Act

            var result = _reservationService.BookReservation(reservation);

            //Assert
            result.Should().BeGreaterThan(0);
        }

        [Test]
        public void AddReservation_InvalidReservation_Test()
        {
            //Arrange
            var reservation = new Reservation();

            // Act
            var result = _reservationService.BookReservation(reservation);

            //Assert
            result.Should().Be(0);
            _reservationService.Logger.ErrorMessages.Count.Should().Be(1);
            _reservationService.Logger.ErrorMessages.FirstOrDefault().Should().Be("GuestFirstname was null or empty");
        }





    }
}
