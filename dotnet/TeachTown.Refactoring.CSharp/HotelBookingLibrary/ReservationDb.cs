using HotelReservationLibrary.DomainObjects;
using Microsoft.Data.SqlClient;

namespace HotelReservationLibrary
{
    public static class ReservationDb
    {
        public static IReservationHandler ReservationHandler = new ReservationHandler();
        /// <summary>
        /// Before calling this function, the reservation object should be validated and the price should be determined.
        /// </summary>
        /// <param name="reservation"></param>
        /// <returns></returns>
        public static long AddReservation(Reservation reservation)
        {
           return ReservationHandler.AddReservation(reservation);
        }
    }

    public interface IReservationHandler
    {
        long AddReservation(Reservation reservation);
    }

    public class ReservationHandler : IReservationHandler
    {
        public long AddReservation(Reservation reservation)
        {
            var newReservationId = 0L;
            using (var connection = new SqlConnection("ConnectionString"))
            {
                connection.Open();
                // Add reservation to database
                var command = connection.CreateCommand();
                command.CommandText = "INSERT INTO Reservations (GuestFirstName, GuestLastName, GuestEmail, CheckInDate, CheckOutDate, NumberOfAdditionalGuests, RoomType, SmokingOrNonSmoking, Total) " +
                    "VALUES (@GuestFirstName, @GuestLastName, @GuestEmail, @CheckInDate, @CheckOutDate, @NumberOfAdditionalGuests, @RoomType, @SmokingOrNonSmoking, @Total)";
                command.Parameters.AddWithValue("@GuestFirstName", reservation.GuestFirstName);
                command.Parameters.AddWithValue("@GuestLastName", reservation.GuestLastName);
                command.Parameters.AddWithValue("@GuestEmail", reservation.guestEmail);
                command.Parameters.AddWithValue("@CheckInDate", reservation.CheckInDate);
                command.Parameters.AddWithValue("@CheckOutDate", reservation.CheckOutDate);
                command.Parameters.AddWithValue("@NumberOfAdditionalGuests", reservation.NumberOfAdditionalGuests);
                command.Parameters.AddWithValue("@RoomType", reservation.RoomType);
                command.Parameters.AddWithValue("@SmokingOrNonSmoking", reservation.SmokingOrNonSmoking);
                command.Parameters.AddWithValue("@Total", reservation.Total);
                command.ExecuteNonQuery();

                // Execute the command and get the new reservation ID.  This assumes the Id field is the first column in the table.
                newReservationId = Convert.ToInt64(command.ExecuteScalar());
            }


            return newReservationId;
        }
    }

    
}
