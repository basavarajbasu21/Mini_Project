using System;
using System.Linq;
using System.Configuration;
using System.Data.SqlClient;
using Train_Reservation;
using Microsoft.SqlServer.Server;

namespace Train_Reservation
{
    class Program
    {
        static string TrainsConnectionString = ConfigurationManager.ConnectionStrings["Train_Reservation"].ConnectionString;
        static Train_ReservationDataContext db = new Train_ReservationDataContext(TrainsConnectionString);

        static string GetCustomerUsername()
        {
            Console.Write("Enter your username: ");
            return Console.ReadLine();
        }

        public class BookedTicket
        {
            public string BookingID { get; set; }
            public string TrainNumber { get; set; }
            public string Class { get; set; }
            public string PassengerName { get; set; }
            public int PassengerAge { get; set; }
            public string PassengerGender { get; set; }
            public string IsCanceled { get; set; }
        }

        static void Main(string[] args)
        {
            Console.WriteLine("------------- Welcome To Train Reservation System --------------");
            Console.WriteLine();

            Console.WriteLine("Are you an admin or a customer?");
            Console.WriteLine("1. Admin");
            Console.WriteLine("2. Customer");
            Console.WriteLine("3. Exit");
            Console.WriteLine();

            Console.Write("Enter your choice: ");

            int choice = int.Parse(Console.ReadLine());
            Console.WriteLine("---------------------------------------------");
            Console.WriteLine();

            switch (choice)
            {
                case 1:
                    AdminMenu();
                    break;
                case 2:
                    string username = GetCustomerUsername(); // Get username from input
                    CheckCustomer();
                    CustomerMenu(username);
                    break;

                case 3:
                    Environment.Exit(0);
                    break;
                default:
                    Console.WriteLine("Invalid choice!");
                    break;
            }
        }

        static bool AdminLogin()
        {
            string adminUsername = "admin";
            string adminPassword = "admin123";

            Console.WriteLine("------------- Admin Login --------------");
            Console.WriteLine();
            Console.Write("Enter Admin Username: ");
            string usernameInput = Console.ReadLine();
            Console.Write("Enter Admin Password: ");
            string passwordInput = Console.ReadLine();

            return usernameInput == adminUsername && passwordInput == adminPassword;
        }

        static void AdminMenu()
        {
            if (AdminLogin())
            {
                Console.WriteLine("Login Successful!");
                Console.WriteLine();

                Console.WriteLine("---------- WelCome To Admin --------------");
                Console.WriteLine();

                Console.WriteLine("Admin Menu:");
                Console.WriteLine("1. Add Train");
                Console.WriteLine("2. Update Train");
                Console.WriteLine("3. Delete Train");
                Console.WriteLine("4. Activate Train");
                Console.WriteLine("5. Deactivate Train");
                Console.WriteLine("6. View All Trains");
                Console.WriteLine("7. Exit");

                Console.WriteLine();
                Console.Write("Enter your choice:");
                int choice = int.Parse(Console.ReadLine());

                switch (choice)
                {
                    case 1:
                        AddTrain();
                        break;
                    case 2:
                        UpdateTrain();
                        break;
                    case 3:
                        DeleteTrain();
                        break;
                    case 4:
                        Console.Write("Enter Train Number to activate: ");
                        string activateTrainNumber = Console.ReadLine();
                        ActivateTrain(activateTrainNumber);
                        break;
                    case 5:
                        Console.Write("Enter Train Number to deactivate: ");
                        string deactivateTrainNumber = Console.ReadLine();
                        DeactivateTrain(deactivateTrainNumber);
                        break;

                    case 6:
                        ViewAllTrains();
                        break;
                    case 7:
                        Environment.Exit(0);
                        break;
                    default:
                        Console.WriteLine("Invalid choice!");
                        break;
                }
            }
            else
            {
                Console.WriteLine("Invalid Credentials. Access Denied.");
            }
            Console.WriteLine();
            Console.WriteLine("--------------------------------------------");
            Console.WriteLine();
            Console.ReadLine();
        }

        static void CheckCustomer()
        {
            Console.WriteLine("---------- Train Reservation ----------------");
            Console.WriteLine();

            Console.WriteLine("Are you a new customer or an existing customer?");
            Console.WriteLine("1. Existing Customer");
            Console.WriteLine("2. New Customer");
            Console.WriteLine("3. Exit");

            Console.WriteLine("---------------------------------------------");
            Console.WriteLine();

            Console.Write("Enter your choice: ");
            int choice = int.Parse(Console.ReadLine());
            Console.WriteLine("---------------------------------------------");
            Console.WriteLine();

            switch (choice)
            {
                case 1:
                    CustomerLogin();
                    break;
                case 2:
                    CreateNewCustomer();
                    // After creating a new account, prompt the user to login again
                    CustomerLogin();
                    break;
                case 3:
                    Environment.Exit(0);
                    break;
                default:
                    Console.WriteLine("Invalid choice!");
                    break;
            }
        }

        static bool CheckExistingCustomer(string username, string password)
        {
            // Check if the entered credentials exist in the Customers table
            using (SqlConnection connection = new SqlConnection(TrainsConnectionString))
            {
                connection.Open();
                string selectQuery = "SELECT COUNT(*) FROM Customers WHERE Username = @Username AND Password = @Password";
                SqlCommand command = new SqlCommand(selectQuery, connection);
                command.Parameters.AddWithValue("@Username", username);
                command.Parameters.AddWithValue("@Password", password);
                int count = (int)command.ExecuteScalar();
                return count > 0;
            }

        }


        static void CreateNewCustomer()
        {
            Console.WriteLine("Create New Account:");
            Console.Write("Enter Username: ");
            string username = Console.ReadLine();
            Console.Write("Enter Password: ");
            string password = Console.ReadLine();
            Console.WriteLine();

            // Check if the username already exists in the database
            bool usernameExists = CheckUsernameExists(username);

            if (usernameExists)
            {
                Console.WriteLine("Username already exists. Please Login with same Username and Password.");
                Console.WriteLine();
                Console.WriteLine("---------------------------------------------");
                Console.WriteLine();

            }
            else
            {
                // Insert the new customer record into the Customers table
                using (SqlConnection connection = new SqlConnection(TrainsConnectionString))
                {
                    connection.Open();
                    string insertQuery = "INSERT INTO Customers (Username, Password) VALUES (@Username, @Password)";
                    SqlCommand command = new SqlCommand(insertQuery, connection);
                    command.Parameters.AddWithValue("@Username", username);
                    command.Parameters.AddWithValue("@Password", password);
                    int rowsAffected = command.ExecuteNonQuery();
                    if (rowsAffected > 0)
                    {
                        Console.WriteLine("New account created successfully!");
                    }
                    else
                    {
                        Console.WriteLine("Failed to create a new account. Please try again.");
                        // Handle failure to create a new account
                    }
                    Console.WriteLine("---------------------------------------------");
                    Console.WriteLine();
                }
            }
        }

        // Method to check if the username already exists in the database
        static bool CheckUsernameExists(string username)
        {
            using (SqlConnection connection = new SqlConnection(TrainsConnectionString))
            {
                connection.Open();
                string selectQuery = "SELECT COUNT(*) FROM Customers WHERE Username = @Username";
                SqlCommand command = new SqlCommand(selectQuery, connection);
                command.Parameters.AddWithValue("@Username", username);
                int count = (int)command.ExecuteScalar();
                return count > 0;
            }
        }

        static bool CustomerLogin()
        {
            Console.WriteLine("Customer Login:");
            Console.Write("Enter Username: ");
            string username = Console.ReadLine();
            Console.Write("Enter Password: ");
            string password = Console.ReadLine();

            // Check if the entered credentials exist in the Customers table
            bool isExistingCustomer = CheckExistingCustomer(username, password);

            if (isExistingCustomer)
            {
                Console.WriteLine("Login Successful!");
                Console.WriteLine();

                // Return true indicating successful login
                return true;
            }
            else
            {
                Console.WriteLine("Login Failed. No existing customer found with the provided credentials.");
                return false;
            }

        }

        static void CustomerMenu(string username)
        {
            // Implement customer menu
            Console.WriteLine();
            Console.WriteLine("---------- Train Reservation ----------------");
            Console.WriteLine();

            // Customer is logged in, display menu options
            Console.WriteLine("Customer Menu:");
            Console.WriteLine("1. Book Ticket");
            Console.WriteLine("2. Cancel Ticket");
            Console.WriteLine("3. View Booked Tickets");
            Console.WriteLine("4. Exit");

            Console.WriteLine("---------------------------------------------");
            Console.WriteLine();

            Console.Write("Enter your choice: ");
            int choice = int.Parse(Console.ReadLine());

            switch (choice)
            {
                case 1:
                    BookTicket(username);
                    break;
                case 2:
                    string bookingID = GetBookingID(); // Obtain the bookingID
                    CancelTicket(username, bookingID);
                    break;

                case 3:
                    ViewBookedTickets(username);
                    break;
                case 4:
                    Environment.Exit(0);
                    break;
                default:
                    Console.WriteLine("Invalid choice!");
                    break;
            }
        }

        static void BookTicket(string username)
        {
            Console.WriteLine("------------- Train Reservation -----------");
            Console.WriteLine();

            // Assuming 'db' is a valid database context
            Console.Write("Enter From city: ");
            string departureCity = Console.ReadLine();
            var FromTrains = from train in db.Train_Books
                             where train.From == departureCity
                             select train;

            Console.Write("Enter To city: ");
            string arrivalCity = Console.ReadLine();
            var ToTrains = from train in db.Train_Books
                           where train.To == arrivalCity
                           select train;

            if (!FromTrains.Any())
            {
                Console.WriteLine($"No train available from {departureCity} to {arrivalCity}.");
                Console.ReadLine();
                return;
            }
            Console.WriteLine();

            Console.WriteLine($"Trains Departing from {departureCity} to {arrivalCity}:");
            Console.WriteLine();
            Console.WriteLine("----------------------------------------------------------------------------------------------------------------------------------------------------------------------");
            Console.WriteLine("| Train No | Train Name         | From           | To       | Class            |Available Berth | Departure Time | Arrival Time | Travel Duration | Runs On | Price   |");
            Console.WriteLine("-----------------------------------------------------------------------------------------------------------------------------------------------------------------------");

            foreach (var train in FromTrains)
            {
                Console.WriteLine($"| {train.Train_No,-8} | {train.Train_Name,-18} | {train.From,-8} | {train.To,-8} | {train.Class,-18} | {train.Available_Berth,-13} | {train.Departure_Time,-13} | {train.Arrival_Time,-13} | {train.Travel_Duration,-13} | {train.Runs_On,-13} | {train.Price,-7} |");
            }

            Console.WriteLine("------------------------------------------------------------------------------------------------------------------------------------------------------------------------");
            Console.WriteLine();


            Console.Write("Enter train number: ");
            Console.WriteLine();

            string trainNumber = Console.ReadLine();
            var trainClasses = from trainClass in db.Train_Books
                               where trainClass.Train_No == trainNumber
                               select trainClass.Class;

            Console.WriteLine($"Available Classes for Train No {trainNumber}:");
            foreach (var trainClass in trainClasses)
            {
                Console.WriteLine(trainClass);
            }
            Console.WriteLine();
            Console.WriteLine("--------------------------------------------");
            Console.WriteLine();

            Console.WriteLine("Select the Train Class:");
            Console.WriteLine();

            var classOptions = FromTrains.Select(train => train.Class).Distinct().ToList();
            for (int i = 0; i < classOptions.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {classOptions[i]}");
            }
            Console.WriteLine();
            Console.Write("Enter your choice: ");
            int classChoice;
            if (!int.TryParse(Console.ReadLine(), out classChoice) || classChoice < 1 || classChoice > classOptions.Count)
            {
                Console.WriteLine("Invalid choice.");
                return;
            }
            string className = classOptions[classChoice - 1];

            Console.WriteLine("--------------------------------------------");
            Console.WriteLine();

            var classInfo = from tClass in db.Train_Books
                            where tClass.Train_No == trainNumber && tClass.Class == className
                            select new
                            {
                                tClass.Total_Berth,
                                tClass.Available_Berth,
                                tClass.Price // Include price in the query
                            };

            foreach (var info in classInfo)
            {
                Console.WriteLine($"Class: {className}, Available Berth: {info.Available_Berth}");
            }

            Console.WriteLine("--------------------------------------------");
            Console.WriteLine();


            Console.Write("Enter the number of seats to book: ");
            Console.WriteLine();

            int seatsToBook;
            if (!int.TryParse(Console.ReadLine(), out seatsToBook))
            {
                Console.WriteLine("Invalid input for number of seats.");
                return;
            }

            string bookingID = GenerateBookingID();
            Console.WriteLine("Generated Booking ID: " + bookingID);

            decimal totalPrice = 0;
            for (int i = 0; i < seatsToBook; i++)
            {
                bookingID = GenerateBookingID(); // Generate a unique booking ID for each seat

                Console.WriteLine($"Passenger {i + 1} Details:");
                Console.WriteLine();

                Console.Write("Enter Passenger Name: ");
                string passengerName = Console.ReadLine();

                Console.Write("Enter Passenger Age: ");
                int passengerAge;
                if (!int.TryParse(Console.ReadLine(), out passengerAge))
                {
                    Console.WriteLine("Invalid input for passenger age.");
                    return;
                }

                Console.Write("Enter Passenger Gender (M/F): ");
                string passengerGender = Console.ReadLine().ToUpper();
                Console.WriteLine();

                decimal? basePrice = classInfo.FirstOrDefault()?.Price;

                if (basePrice.HasValue)
                {
                    decimal passengerPrice = CalculatePassengerPrice(basePrice.Value, passengerAge, passengerGender);
                    totalPrice += passengerPrice;

                    Console.WriteLine($"Ticket Price for Passenger {passengerName}: ${passengerPrice}");
                }
                else
                {
                    Console.WriteLine("Price not found for the selected class.");
                    return;
                }

                Console.WriteLine($"Booking ID for Passenger {passengerName}: {bookingID}");
                Console.WriteLine();


                // Now, we'll insert the booking details into the database
                using (SqlConnection connection = new SqlConnection(TrainsConnectionString))
                {
                    connection.Open();
                    string insertQuery = "INSERT INTO BookedTickets (BookingID, TrainNumber, Class, PassengerName, PassengerAge, PassengerGender, Username) VALUES (@BookingID, @TrainNumber, @Class, @PassengerName, @PassengerAge, @PassengerGender, @Username)";
                    SqlCommand command = new SqlCommand(insertQuery, connection);
                    command.Parameters.AddWithValue("@BookingID", bookingID);
                    command.Parameters.AddWithValue("@TrainNumber", trainNumber);
                    command.Parameters.AddWithValue("@Class", className);
                    command.Parameters.AddWithValue("@PassengerName", passengerName);
                    command.Parameters.AddWithValue("@PassengerAge", passengerAge);
                    command.Parameters.AddWithValue("@PassengerGender", passengerGender);
                    command.Parameters.AddWithValue("@Username", username); // Add username parameter
                    command.ExecuteNonQuery();
                }
            }

            Console.WriteLine();
            Console.WriteLine($"Total Price for {seatsToBook} Seats: ${totalPrice}");
            Console.WriteLine("-----------------------------------------------------------");
            Console.WriteLine();

            var classToUpdate = db.Train_Books.FirstOrDefault(t => t.Train_No == trainNumber && t.Class == className);
            if (classToUpdate != null)
            {
                classToUpdate.Available_Berth -= seatsToBook;
                db.SubmitChanges();
            }

            //Console.WriteLine($"After Booking Available Berth for Class {className}: {classToUpdate.Available_Berth}");
            //Console.WriteLine();

            var selectedTrain = FromTrains.FirstOrDefault(t => t.To == arrivalCity);
            if (selectedTrain != null)
            {
                Console.WriteLine($"Successfully {seatsToBook} Seats has been Booked, From {departureCity} To {arrivalCity}...");
                Console.WriteLine();
                Console.WriteLine("---------- Happy Journey -----------");
                Console.ReadLine();
            }
            else
            {
                Console.WriteLine($"No train available from {departureCity} to {arrivalCity}.");
            }
            Console.WriteLine();
        }

        static string GenerateBookingID()
        {
            Random random = new Random();
            int randomNumber = random.Next(1000, 10000);
            string bookingID = randomNumber.ToString("D4");
            return bookingID;
        }
        private static decimal CalculatePassengerPrice(decimal basePrice, int passengerAge, string passengerGender)
        {
            decimal passengerPrice = basePrice;

            if (passengerAge >= 60)
            {
                passengerPrice *= 0.5m; // 50% discount for senior citizens
                Console.WriteLine("50% discount for senior citizens");

            }
            else if (passengerAge < 10)
            {
                passengerPrice *= 0.7m; // 30% discount for children
                Console.WriteLine("30% discount for children");

            }

            if (passengerGender == "F")
            {
                passengerPrice *= 0.8m; // 20% discount for female passengers
                Console.WriteLine("20% discount for female passengers");

            }

            return passengerPrice;
        }

        private static decimal CalculatePassengerPrice2(decimal basePrice, int passengerAge, string passengerGender)
        {
            decimal passengerPrice = basePrice;

            if (passengerAge >= 60)
            {
                passengerPrice *= 0.5m; // 50% discount for senior citizens
                Console.WriteLine("50% discount for senior citizens");
            }
            else if (passengerAge < 10)
            {
                passengerPrice *= 0.7m; // 30% discount for children
                Console.WriteLine("30% discount for children");
            }

            if (passengerGender == "F")
            {
                passengerPrice *= 0.8m; // 20% discount for female passengers
                Console.WriteLine("20% discount for female passengers");
            }

            return passengerPrice;
        }

        static string GetBookingID()
        {
            Console.Write("Enter the Booking ID: ");
            return Console.ReadLine();
        }
        static void CancelTicket(string username, string bookingID)
        {
            try
            {
                // Check if the booking exists, is associated with the provided username, and is not already canceled
                using (SqlConnection connection = new SqlConnection(TrainsConnectionString))
                {
                    connection.Open();
                    string selectQuery = "SELECT * FROM BookedTickets WHERE BookingID = @BookingID AND Username = @Username AND IsCanceled = '0'";
                    SqlCommand selectCommand = new SqlCommand(selectQuery, connection);
                    selectCommand.Parameters.AddWithValue("@BookingID", bookingID);
                    selectCommand.Parameters.AddWithValue("@Username", username);
                    SqlDataReader reader = selectCommand.ExecuteReader();

                    if (reader.HasRows)
                    {
                        // Display booking details
                        Console.WriteLine("Booking Details:");
                        Console.WriteLine("----------------");
                        while (reader.Read())
                        {
                            Console.WriteLine($"Booking ID: {reader["BookingID"]}");
                            Console.WriteLine($"Train Number: {reader["TrainNumber"]}");
                            Console.WriteLine($"Class: {reader["Class"]}");
                            Console.WriteLine($"Passenger Name: {reader["PassengerName"]}");
                            Console.WriteLine($"Passenger Age: {reader["PassengerAge"]}");
                            Console.WriteLine($"Passenger Gender: {reader["PassengerGender"]}");
                            Console.WriteLine();
                        }

                        // Close the SqlDataReader before executing the UPDATE query
                        reader.Close();

                        // Prompt for confirmation
                        Console.Write("Are you sure you want to cancel this ticket? (Y/N): ");
                        string confirmation = Console.ReadLine().ToUpper();
                        if (confirmation == "Y")
                        {
                            // Update the cancellation status in the database
                            string updateQuery = "UPDATE BookedTickets SET IsCanceled = '1' WHERE BookingID = @BookingID";
                            SqlCommand updateCommand = new SqlCommand(updateQuery, connection);
                            updateCommand.Parameters.AddWithValue("@BookingID", bookingID);
                            int rowsAffected = updateCommand.ExecuteNonQuery();

                            if (rowsAffected > 0)
                            {
                                Console.WriteLine($"Ticket with ID {bookingID} has been canceled successfully.");
                            }
                            else
                            {
                                Console.WriteLine($"Failed to cancel ticket with ID {bookingID}.");
                            }
                        }
                        else
                        {
                            Console.WriteLine("Ticket cancellation aborted.");
                        }

                    }
                    else
                    {
                        Console.WriteLine($"Booking with ID {bookingID} does not exist, is not associated with your account, or is already canceled.");
                    }
                    Console.ReadLine();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred while canceling the ticket:");
                Console.WriteLine(ex.Message);
                Console.ReadLine();
            }
        }

        private static decimal CalculateRefund(decimal basePrice, int ticketsToCancel)
        {
            // Assuming no refund for partially used tickets
            return basePrice * ticketsToCancel;
        }

        static void ViewBookedTickets(string Usernaqme)
        {
            Console.Write("Enter Train Number: ");
            string trainNumber = Console.ReadLine();

            Console.Write("Enter Class: ");
            string ticketClass = Console.ReadLine();
            Console.WriteLine("---------------------------------------------");
            Console.WriteLine();

            var bookedTickets = db.Train_Books.Where(t => t.Train_No == trainNumber && t.Class == ticketClass);

            if (bookedTickets.Any())
            {
                Console.WriteLine("Booked Tickets: ");
                foreach (var ticket in bookedTickets)
                {
                    Console.WriteLine($"Train Name: {ticket.Train_Name}");
                    Console.WriteLine($"Class: {ticket.Class}");
                    Console.WriteLine($"From: {ticket.From} To: {ticket.To}");
                    Console.WriteLine($"Departure at {ticket.Departure_Time} Arrival at {ticket.Arrival_Time}");
                    Console.WriteLine($"Runs On: {ticket.Runs_On}");
                    Console.WriteLine($"Price: {ticket.Price}");
                    Console.WriteLine();
                    Console.WriteLine("---------------------------------------------");
                    Console.WriteLine();
                }
            }
            else
            {
                Console.WriteLine("No booking found for the given details.");
            }

            using (SqlConnection connection = new SqlConnection(TrainsConnectionString))
            {
                connection.Open();
                string selectQuery = "SELECT * FROM BookedTickets WHERE TrainNumber = @TrainNumber AND Class = @Class";
                SqlCommand command = new SqlCommand(selectQuery, connection);
                command.Parameters.AddWithValue("@TrainNumber", trainNumber);
                command.Parameters.AddWithValue("@Class", ticketClass);
                SqlDataReader reader = command.ExecuteReader();
                
                // Display booked ticket details in a table format
                Console.WriteLine("---------------------------------");
                Console.WriteLine("| Passenger Name | Age | Gender |");
                Console.WriteLine("---------------------------------");

                while (reader.Read())
                {
                    string passengerName = (string)reader["PassengerName"];
                    int passengerAge;
                    if (!reader.IsDBNull(reader.GetOrdinal("PassengerAge")) && int.TryParse(reader["PassengerAge"].ToString(), out passengerAge))
                    {
                        string passengerGender = (string)reader["PassengerGender"];

                        // Formatting the output in a table format
                        Console.WriteLine($"| {passengerName,-15} | {passengerAge,-3} | {passengerGender,-6} |");

                    }
                    else
                    {
                        Console.WriteLine("Error: Invalid or null value for passenger age.");
                    }

                }


            }
            Console.WriteLine();
            Console.WriteLine("------------ Happy Journey ------------");
            Console.ReadLine();

        }

        /////////////////////////////////////////////////////////////////////////////////////////////////////////
        static void AddTrain()
        {
            Console.WriteLine("Add New Train:");
            Console.Write("Enter Train Number: ");
            string trainNumber = Console.ReadLine();

            // Check if the train number already exists
            var existingTrain = db.Train_Books.FirstOrDefault(t => t.Train_No == trainNumber);
            if (existingTrain != null)
            {
                Console.WriteLine($"Train with number {trainNumber} already exists.");
                return;
            }

            // If the train number doesn't exist, proceed with adding the new train details
            Console.Write("Enter Train Name: ");
            string trainName = Console.ReadLine();
            Console.Write("Enter Arrival Time: ");
            string arrivalTime = Console.ReadLine();
            Console.Write("Enter Departure Time: ");
            string departureTime = Console.ReadLine();
            Console.Write("Enter Travel Duration: ");
            string travelDuration = Console.ReadLine();
            Console.Write("Enter Class: ");
            string ticketClass = Console.ReadLine();
            Console.Write("Enter Total Berth: ");
            int totalBerth = Convert.ToInt32(Console.ReadLine());
            Console.Write("Enter Available Berth: ");
            int availableBerth = Convert.ToInt32(Console.ReadLine());
            Console.Write("Enter From City: ");
            string fromCity = Console.ReadLine();
            Console.Write("Enter To City: ");
            string toCity = Console.ReadLine();
            Console.Write("Enter Runs On: ");
            string runsOn = Console.ReadLine();
            Console.Write("Enter Price: ");
            decimal price = Convert.ToDecimal(Console.ReadLine());

            // Add the new train to the database
            var newTrain = new Train_Book
            {
                Train_No = trainNumber,
                Train_Name = trainName,
                Arrival_Time = arrivalTime,
                Departure_Time = departureTime,
                Travel_Duration = travelDuration,
                Class = ticketClass,
                Total_Berth = totalBerth,
                Available_Berth = availableBerth,
                From = fromCity,
                To = toCity,
                Runs_On = runsOn,
                Price = price,
                IsActive = true,
            };

            db.Train_Books.InsertOnSubmit(newTrain);
            db.SubmitChanges();

            Console.WriteLine("New train added successfully!");
            Console.ReadLine();

        }

        static void UpdateTrain()
        {
            Console.WriteLine("--------------------------------------------");
            Console.WriteLine();

            Console.WriteLine("Update Train:");
            Console.Write("Enter Train Number to update: ");
            string trainNumber = Console.ReadLine();

            Console.Write("Enter Train Class Type: ");
            string trainClass = Console.ReadLine();

            Console.WriteLine();
            Console.WriteLine("--------------------------------------------");
            Console.WriteLine();

            var trainToUpdate = db.Train_Books.FirstOrDefault(t => t.Train_No == trainNumber && t.Class == trainClass);
            if (trainToUpdate == null)
            {
                Console.WriteLine($"Train with number {trainNumber} not found.");
                Console.WriteLine($"Train with class {trainClass} not found.");
                return;
            }

            Console.WriteLine("Select field to update:");
            Console.WriteLine("1. Train Name");
            Console.WriteLine("2. Arrival Time");
            Console.WriteLine("3. Departure Time");
            Console.WriteLine("4. Travel Duration");
            Console.WriteLine("5. Class");
            Console.WriteLine("6. Total Berth");
            Console.WriteLine("7. Available Berth");
            Console.WriteLine("8. From City");
            Console.WriteLine("9. To City");
            Console.WriteLine("10. Runs On");
            Console.WriteLine("11. Price");

            Console.WriteLine();
            Console.WriteLine("--------------------------------------------");
            Console.WriteLine();

            Console.Write("Enter your choice: ");
            int choice = int.Parse(Console.ReadLine());

            switch (choice)
            {
                case 1:
                    Console.Write("Enter New Train Name: ");
                    trainToUpdate.Train_Name = Console.ReadLine();
                    break;
                case 2:
                    Console.Write("Enter New Arrival Time: ");
                    trainToUpdate.Arrival_Time = Console.ReadLine();
                    break;
                case 3:
                    Console.Write("Enter New Departure Time: ");
                    trainToUpdate.Departure_Time = Console.ReadLine();
                    break;
                case 4:
                    Console.Write("Enter New Travel Duration: ");
                    trainToUpdate.Travel_Duration = Console.ReadLine();
                    break;
                case 5:
                    Console.Write("Enter New Class: ");
                    trainToUpdate.Class = Console.ReadLine();
                    break;
                case 6:
                    Console.Write("Enter New Total Berth: ");
                    trainToUpdate.Total_Berth = int.Parse(Console.ReadLine());
                    break;
                case 7:
                    Console.Write("Enter New Available Berth: ");
                    trainToUpdate.Available_Berth = int.Parse(Console.ReadLine());
                    break;
                case 8:
                    Console.Write("Enter New From City: ");
                    trainToUpdate.From = Console.ReadLine();
                    break;
                case 9:
                    Console.Write("Enter New To City: ");
                    trainToUpdate.To = Console.ReadLine();
                    break;
                case 10:
                    Console.Write("Enter New Runs On: ");
                    trainToUpdate.Runs_On = Console.ReadLine();
                    break;
                case 11:
                    Console.Write("Enter New Price: ");
                    trainToUpdate.Price = decimal.Parse(Console.ReadLine());
                    break;
                default:
                    Console.WriteLine("Invalid choice!");
                    break;
            }

            Console.WriteLine("--------------------------------------------");
            Console.WriteLine();

            db.SubmitChanges();
            Console.WriteLine("Train updated successfully!");
            Console.WriteLine();
            Console.WriteLine("--------------------------------------------");
            Console.WriteLine();
            Console.ReadLine();

        }

        static void DeleteTrain()
        {
            Console.WriteLine("Delete Train:");
            Console.Write("Enter Train Number to delete: ");
            string trainNumber = Console.ReadLine();
            Console.WriteLine();

            var trainToDelete = db.Train_Books.FirstOrDefault(t => t.Train_No == trainNumber);
            if (trainToDelete == null)
            {
                Console.WriteLine($"Train with number {trainNumber} not found.");
                return;
            }

            db.Train_Books.DeleteOnSubmit(trainToDelete);
            db.SubmitChanges();

            Console.WriteLine("Train deleted successfully!");
            Console.WriteLine("--------------------------------------------");
            Console.ReadLine();

        }

        static void ActivateTrain(string trainNumber)
        {
            var trainStatus = db.Train_Books.FirstOrDefault(t => t.Train_No == trainNumber);
            Console.WriteLine();
            if (trainStatus != null)
            {
                trainStatus.IsActive = true;
                db.SubmitChanges();
                

                Console.WriteLine($"Train {trainNumber} activated successfully.");
                
            }
            else
            {
                Console.WriteLine($"Train {trainNumber} not found.");
            }
            Console.WriteLine();
            Console.WriteLine("--------------------------------------------");
            Console.WriteLine();
        }

        static void DeactivateTrain(string trainNumber)
        {
            var trainStatus = db.Train_Books.FirstOrDefault(t => t.Train_No == trainNumber);
            Console.WriteLine();

            if (trainStatus != null)
            {
                trainStatus.IsActive = false;
                db.SubmitChanges();
                Console.WriteLine($"Train {trainNumber} deactivated successfully.");
            }
            else
            {
                Console.WriteLine($"Train {trainNumber} not found.");
            }
            Console.WriteLine();
            Console.WriteLine("--------------------------------------------");
            Console.WriteLine();
        }

        static void ViewAllTrains()
        {
            Console.WriteLine("All Trains:");
            var allTrains = db.Train_Books;

            // Display table headings
            Console.WriteLine("----------------------------------------------------------------------------------------------------------------------------------------------------------------------");
            Console.WriteLine("| Train No  |Train Name           | From              |     To             | Departure | Arrival | Duration |    Class             |   Price |    Runs On    |IsActive|");
            Console.WriteLine("----------------------------------------------------------------------------------------------------------------------------------------------------------------------");

            foreach (var train in allTrains)
            {
                // Display train details in table format
                Console.WriteLine($"| {train.Train_No,-9} | {train.Train_Name,-18} | {train.From,-18} | {train.To,-18} | {train.Departure_Time,-9} | {train.Arrival_Time,-7} | {train.Travel_Duration,-8} | {train.Class,-20} | {train.Price,-7} | {train.Runs_On,-13} | {train.IsActive,-4} |");
            }

            // End of table
            Console.WriteLine("----------------------------------------------------------------------------------------------------------------------------------------------------------------------");
            Console.ReadLine();
        }
    }
}
