using HotelReservationLibrary.DomainObjects;
using HotelReservationLibrary.ExternalAPIs;

namespace HotelReservationLibrary
{
    public class ReservationService
    {
        public LogMessage Logger = new LogMessage();
        /// <summary>
        /// Validates the incoming reservation and then saves it to the database.
        /// If successful, returns the reservation's booking number.  Otherwise, returns 0.
        /// </summary>
        /// <param name="reservationContract"></param>
        /// <returns></returns>
        public long BookReservation(Reservation reservationContract)
        {
            ArgumentNullException.ThrowIfNull(reservationContract);

            if (!ValidateReservation(reservationContract))
            {
                return 0;
            }

            if (!DeterminePrice(reservationContract))
            {
                return 0;
            }

            //Save the reservation to the database
            return ReservationDb.AddReservation(reservationContract);
        }

        /// <summary>Logger.LogError
        /// Performs validations on the incoming reservation object.
        /// If any errors are found, they are written to the console.
        /// Returns true if all is good.  Returns false if there are any issues.
        /// </summary>
        /// <param name="reservationContract"></param>
        /// <returns></returns>
        private bool ValidateReservation(Reservation reservationContract)
        {
            ArgumentNullException.ThrowIfNull(reservationContract);

            if (string.IsNullOrEmpty(reservationContract.GuestFirstName))
            {
                Logger.LogError("GuestFirstname was null or empty");
                return false;
            }

            if (string.IsNullOrEmpty(reservationContract.GuestLastName))
            {
                Logger.LogError("GuestLastName was null or empty");
                return false;
            }

            if (string.IsNullOrEmpty(reservationContract.guestEmail))
            {
                Logger.LogError("guestEmail was null or empty");
                return false;
            }

            //To do: expand this check
            if (!reservationContract.guestEmail.Contains('@'))
            {
                Logger.LogError("guestEmail did not meet email matching validations");
                return false;
            }

            if (reservationContract.CheckOutDate < reservationContract.CheckInDate)
            {
                Logger.LogError("Checkout date must be after Checkin date.  It cannot be before or equal to Checkin date.");
                return false;
            }

            //ToDo:  Why do we only allow 2 additional guests?
            if (reservationContract.NumberOfAdditionalGuests > 2)
            {
                Logger.LogError("Too man additional guests");
                return false;
            }

            if (reservationContract.RoomType != Reservation.SINGLE_ROOM
                && reservationContract.RoomType != Reservation.DOUBLE_ROOM
                && reservationContract.RoomType != Reservation.SUITE)
            {
                Logger.LogError("Unrecognized room type");
                return false;
            }

            if (reservationContract.SmokingOrNonSmoking != "Smoking" && reservationContract.SmokingOrNonSmoking != "Non-Smoking")
            {
                Logger.LogError("Unrecognized smoking option");
                return false;
            }

            return true;
        }


        /// <summary>
        /// Determines the price of the reservation and then saves it to the database.
        /// </summary>
        /// <param name="reservationContract"></param>
        /// <returns></returns>
        private bool DeterminePrice(Reservation reservationContract)
        {
            //TODO:  I dont like the fact that these are hard coded.  This should be in a configuration file?
            if (reservationContract.RoomType == Reservation.SINGLE_ROOM)
            {
                reservationContract.PricePerNight = 100;
            }
            else if (reservationContract.RoomType == Reservation.DOUBLE_ROOM)
            {
                reservationContract.PricePerNight = 200;
            }
            else if (reservationContract.RoomType == Reservation.SUITE)
            {
                reservationContract.PricePerNight = 300;
            }
            else
            {
                return false;
            }


            //If the room is smoking, add 5% to the price
            if (reservationContract.SmokingOrNonSmoking == Reservation.SMOKING)
            {
                reservationContract.PricePerNight *= 1.05;
            }

            reservationContract.Total = reservationContract.PricePerNight * (reservationContract.CheckOutDate - reservationContract.CheckInDate).Days;

            //Charge more for extreme weather because of the additional cost of heating or cooling the rooms.
            var weatherService = new ExternalWeatherApi();
            try
            {
                var forecast = weatherService.GetForecast(
                    DateOnly.FromDateTime(reservationContract.CheckInDate),
                    DateOnly.FromDateTime(reservationContract.CheckOutDate));

                if (forecast.Summary == "Freezing" || forecast.Summary == "Sweltering")
                {
                    reservationContract.Total *= 1.2;
                }
            }
            catch (Exception ex)
            {
                //QUESTION:  Should we fail the reservation if we cannot get the weather forecast?
                Logger.LogError("Error getting weather forecast: " + ex.Message);
                return false;
            }

            return true;

        }
    }
}