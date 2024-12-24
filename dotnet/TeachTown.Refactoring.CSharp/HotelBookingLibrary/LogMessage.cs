using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelReservationLibrary
{
    /// <summary>
    /// The purpose of this class is to log messages.  Instead of having each class handle logging messages, classes
    /// can call functions here to handle messages.  That way, if we ever want to change how we handle logging messages,
    /// we only have to update this one class.
    /// 
    /// Currently, this logs messages to the Console, but in the future, it could log to a file or a database.
    /// </summary>
    public class LogMessage
    {
        public List<string> ErrorMessages = new List<string>();

        /// <summary>
        /// Logs an error message to the console.
        /// </summary>
        /// <param name="message"></param>
        public void LogError(string message)
        {
            ErrorMessages.Add(message);
            Console.WriteLine("ERROR: " + message);
        }
    }
}
