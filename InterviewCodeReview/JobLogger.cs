using System;
using System.Data;
using System.Data.SqlClient;
using System.Text;

namespace InterviewCodeReview
{
    public static class JobLogger
    {
        private static bool _logToFile;
        private static bool _logToConsole;
        private static bool _logToDatabase;
        private static bool _logMessage;
        private static bool _logWarning;
        private static bool _logError;
        private static bool _initialized;

        public static void SetOptions(bool logToFile, bool logToConsole, bool logToDatabase, bool logMessage, bool logWarning, bool logError)
        {
            _logToFile = logToFile;
            _logToConsole = logToConsole;
            _logToDatabase = logToDatabase;
            _logMessage = logMessage;
            _logWarning = logWarning;
            _logError = logError;

            _initialized = true;
        }

        public static void Log(string message, MessageType messageType)
        {
            Validate(message);

            message = message.Trim();

            if (_logToFile)
            {
                LogToFile(message, messageType);
            }

            if (_logToDatabase)
            {
                LogToDatabase(message, messageType);
            }

            if (_logToConsole)
            {
                LogToConsole(message, messageType);
            }
        }

        private static void Validate(string message)
        {
            if (!_initialized)
            {
                throw new Exception("You should set the initial parameters.");
            }

            if (string.IsNullOrEmpty(message))
            {
                throw new Exception("Message can't be a null noe empty string.");
            }

            if (!_logToConsole && !_logToFile && !_logToDatabase)
            {
                throw new Exception("You must choose where to log to.");
            }

            if (!_logMessage && !_logWarning && !_logError)
            {
                throw new Exception("You must choose what to type of message to log. One at least.");
            }
        }

        private static void LogToConsole(string message, MessageType messageType)
        {
            if (_logError && messageType == MessageType.Error)
            {
                Console.ForegroundColor = ConsoleColor.Red;
            }

            if (_logWarning && messageType == MessageType.Warning)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
            }

            if (_logMessage && messageType == MessageType.Message)
            {
                Console.ForegroundColor = ConsoleColor.White;
            }

            Console.WriteLine(DateTime.Now.ToShortDateString() + message);
        }

        private static void LogToFile(string message, MessageType messageType)
        {
            var wholeMessage = new StringBuilder();
            var fileName = System.Configuration.ConfigurationManager.AppSettings["LogFileDirectory"] + "LogFile" + DateTime.Now.ToString("ddMMyyyy") + ".txt";

            if (!System.IO.File.Exists(fileName))
            {
                System.IO.File.Create(fileName);
            }
            else
            {
                wholeMessage.Append(System.IO.File.ReadAllText(fileName));
            }

            if (_logError && messageType == MessageType.Error)
            {
                wholeMessage.Append(DateTime.Now.ToShortDateString() + message);
            }

            if (_logWarning && messageType == MessageType.Warning)
            {
                wholeMessage.Append(DateTime.Now.ToShortDateString() + message);
            }

            if (_logMessage && messageType == MessageType.Message)
            {
                wholeMessage.Append(DateTime.Now.ToShortDateString() + message);
            }

            System.IO.File.WriteAllText(fileName, wholeMessage.ToString());
        }

        private static void LogToDatabase(string message, MessageType messageType)
        {
            var connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
            var connection = new SqlConnection(connectionString);

            try
            {
                connection.Open();

                int msgType = 0;

                if (_logMessage && messageType == MessageType.Message)
                {
                    msgType = 1;
                }

                if (_logError && messageType == MessageType.Error)
                {
                    msgType = 2;
                }

                if (_logWarning && messageType == MessageType.Warning)
                {
                    msgType = 3;
                }

                var dml = String.Format("INSERT INTO Log VALUES('{0}', {1})", message, msgType);
                var command = new SqlCommand(dml, connection);

                command.ExecuteNonQuery();
            }
            catch (SqlException sqlEx)
            {
                throw new ApplicationException("There was an inconvenient while interacting with the database:" + sqlEx.Message);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }
            }           
        }
    }

    public enum MessageType
    {
        Message,
        Warning,
        Error
    }
}
