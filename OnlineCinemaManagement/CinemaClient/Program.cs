using CinemaClient.CinemaServiceReference;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CinemaClient
{
    class Program
    {
        static CinemaServiceClient _service = new CinemaServiceClient();

        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            bool exit = false;

            while (!exit)
            {
                try
                {
                    Console.Clear();
                    Console.WriteLine("==== Online Cinema Management System =====");
                    Console.WriteLine("1. View All Showtimes");
                    Console.WriteLine("2. View Showtime By ID");
                    Console.WriteLine("3. Add New Showtime");
                    Console.WriteLine("4. Update Existing Showtime");
                    Console.WriteLine("5. Delete Showtime");
                    Console.WriteLine("0. Exit");
                    Console.WriteLine("============================================");
                    Console.Write("\nEnter your choice: ");

                    if (int.TryParse(Console.ReadLine(), out int choice))
                    {
                        switch (choice)
                        {
                            case 1: ViewAllShowtimes(); break;
                            case 2: ViewShowtimeById(); break;
                            case 3: AddShowtime(); break;
                            case 4: UpdateShowtime(); break;
                            case 5: DeleteShowtime(); break;
                            case 0: exit = true; break;
                            default:
                                Console.WriteLine("Invalid choice. Press any key to continue...");
                                Console.ReadKey();
                                break;
                        }
                    }
                    else
                    {
                        Console.WriteLine("Invalid input. Press any key to continue...");
                        Console.ReadKey();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"An error occurred: {ex.Message}");
                    Console.WriteLine("Press any key to continue...");
                    Console.ReadKey();
                }
            }
        }

        static void ViewAllShowtimes()
        {
            Console.Clear();
            Console.WriteLine("=== All Showtimes ===\n");

            List<ShowtimeDto> showtimes = _service.GetAllShowtimes().ToList();

            if (showtimes.Count == 0)
            {
                Console.WriteLine("No showtimes found.");
            }
            else
            {
                Console.WriteLine($"{"ID",-5} {"Movie",-30} {"Hall",-15} {"Date & Time",-20} {"Price",-10}");
                Console.WriteLine(new string('-', 80));

                foreach (var showtime in showtimes)
                {
                    Console.WriteLine($"{showtime.ShowtimeID,-5} {showtime.MovieTitle,-30} {showtime.HallName,-15} {showtime.Showtime,-20:g} {showtime.TicketPrice,-10:C}");
                }
            }

            Console.WriteLine("\nPress any key to return to the menu...");
            Console.ReadKey();
        }

        static void ViewShowtimeById()
        {
            Console.Clear();
            Console.WriteLine("=== View Showtime By ID ===\n");

            Console.Write("Enter Showtime ID: ");
            if (int.TryParse(Console.ReadLine(), out int id))
            {
                try
                {
                    ShowtimeDto showtime = _service.GetShowtimeById(id.ToString());

                    Console.WriteLine("\nShowtime Details:");
                    Console.WriteLine($"ID: {showtime.ShowtimeID}");
                    Console.WriteLine($"Movie: {showtime.MovieTitle} (ID: {showtime.MovieID})");
                    Console.WriteLine($"Hall: {showtime.HallName} (ID: {showtime.HallID})");
                    Console.WriteLine($"Date & Time: {showtime.Showtime:g}");
                    Console.WriteLine($"Ticket Price: {showtime.TicketPrice:C}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                }
            }
            else
            {
                Console.WriteLine("Invalid ID format.");
            }

            Console.WriteLine("\nPress any key to return to the menu...");
            Console.ReadKey();
        }

        static void AddShowtime()
        {
            Console.Clear();
            Console.WriteLine("=== Add New Showtime ===\n");

            // Show available movies
            List<MovieDto> movies = new List<MovieDto>();
            try
            {
                movies = _service.GetAllMovies().ToList();
                Console.WriteLine("Available Movies:");
                foreach (var m in movies)
                    Console.WriteLine($"  {m.MovieID}: {m.Title}");
            }
            catch
            {
                Console.WriteLine("(Could not load movie list.)");
            }

            ShowtimeDto newShowtime = new ShowtimeDto();

            Console.Write("\nEnter Movie ID: ");
            if (!int.TryParse(Console.ReadLine(), out int movieId))
            {
                Console.WriteLine("Invalid Movie ID. Operation canceled.");
                Console.WriteLine("\nPress any key to return to the menu...");
                Console.ReadKey();
                return;
            }

            if (movies.Any() && !movies.Any(m => m.MovieID == movieId))
            {
                Console.WriteLine($"Movie with ID {movieId} does not exist. Operation canceled.");
                Console.WriteLine("\nPress any key to return to the menu...");
                Console.ReadKey();
                return;
            }

            newShowtime.MovieID = movieId;

            // Show available halls
            List<HallDto> halls = new List<HallDto>();
            try
            {
                halls = _service.GetAllHalls().ToList();
                Console.WriteLine("\nAvailable Halls:");
                foreach (var h in halls)
                    Console.WriteLine($"  {h.HallID}: {h.HallName}");
            }
            catch
            {
                Console.WriteLine("(Could not load hall list.)");
            }

            Console.Write("\nEnter Hall ID: ");
            if (!int.TryParse(Console.ReadLine(), out int hallId))
            {
                Console.WriteLine("Invalid Hall ID. Operation canceled.");
                Console.WriteLine("\nPress any key to return to the menu...");
                Console.ReadKey();
                return;
            }

            if (halls.Any() && !halls.Any(h => h.HallID == hallId))
            {
                Console.WriteLine($"Hall with ID {hallId} does not exist. Operation canceled.");
                Console.WriteLine("\nPress any key to return to the menu...");
                Console.ReadKey();
                return;
            }

            newShowtime.HallID = hallId;

            Console.Write("Enter Showtime (yyyy-MM-dd HH:mm): ");
            if (!DateTime.TryParse(Console.ReadLine(), out DateTime showtime))
            {
                Console.WriteLine("Invalid date format. Operation canceled.");
                Console.WriteLine("\nPress any key to return to the menu...");
                Console.ReadKey();
                return;
            }
            newShowtime.Showtime = showtime;

            Console.Write("Enter Ticket Price: ");
            if (!decimal.TryParse(Console.ReadLine(), out decimal price) || price <= 0)
            {
                Console.WriteLine("Invalid price. Must be a number greater than zero. Operation canceled.");
                Console.WriteLine("\nPress any key to return to the menu...");
                Console.ReadKey();
                return;
            }
            newShowtime.TicketPrice = price;

            Console.WriteLine("\nReview:");
            Console.WriteLine($"Movie ID: {newShowtime.MovieID}");
            Console.WriteLine($"Hall ID: {newShowtime.HallID}");
            Console.WriteLine($"Showtime: {newShowtime.Showtime:g}");
            Console.WriteLine($"Ticket Price: {newShowtime.TicketPrice:C}");

            Console.Write("\nDo you want to add this showtime? (Y/N): ");
            if (Console.ReadLine().Trim().ToUpper() == "Y")
            {
                try
                {
                    _service.AddShowtime(newShowtime);
                    Console.WriteLine("Showtime added successfully!");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                }
            }
            else
            {
                Console.WriteLine("Operation canceled.");
            }

            Console.WriteLine("\nPress any key to return to the menu...");
            Console.ReadKey();
        }

        static void UpdateShowtime()
        {
            Console.Clear();
            Console.WriteLine("=== Update Existing Showtime ===\n");

            Console.Write("Enter Showtime ID to update: ");
            if (!int.TryParse(Console.ReadLine(), out int id))
            {
                Console.WriteLine("Invalid ID format. Operation canceled.");
                Console.WriteLine("\nPress any key to return to the menu...");
                Console.ReadKey();
                return;
            }

            try
            {
                ShowtimeDto showtime = _service.GetShowtimeById(id.ToString());
                ShowtimeDto original = new ShowtimeDto
                {
                    ShowtimeID = showtime.ShowtimeID,
                    MovieID = showtime.MovieID,
                    MovieTitle = showtime.MovieTitle,
                    HallID = showtime.HallID,
                    HallName = showtime.HallName,
                    Showtime = showtime.Showtime,
                    TicketPrice = showtime.TicketPrice
                };

                Console.WriteLine("\nCurrent Showtime Details:");
                Console.WriteLine($"ID: {showtime.ShowtimeID}");
                Console.WriteLine($"Movie: {showtime.MovieTitle} (ID: {showtime.MovieID})");
                Console.WriteLine($"Hall: {showtime.HallName} (ID: {showtime.HallID})");
                Console.WriteLine($"Date & Time: {showtime.Showtime:g}");
                Console.WriteLine($"Ticket Price: {showtime.TicketPrice:C}");

                Console.WriteLine("\nEnter new details (leave blank to keep current value):");

                Console.Write($"Enter new Movie ID [{showtime.MovieID}]: ");
                string movieIdInput = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(movieIdInput))
                {
                    if (int.TryParse(movieIdInput, out int newMovieId))
                        showtime.MovieID = newMovieId;
                    else
                    {
                        Console.WriteLine("Invalid Movie ID, keeping current value.");
                    }
                }

                Console.Write($"Enter new Hall ID [{showtime.HallID}]: ");
                string hallIdInput = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(hallIdInput))
                {
                    if (int.TryParse(hallIdInput, out int newHallId))
                        showtime.HallID = newHallId;
                    else
                        Console.WriteLine("Invalid Hall ID, keeping current value.");
                }

                Console.Write($"Enter new Showtime [{showtime.Showtime:g}] (yyyy-MM-dd HH:mm): ");
                string showtimeInput = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(showtimeInput))
                {
                    if (DateTime.TryParse(showtimeInput, out DateTime newDate))
                        showtime.Showtime = newDate;
                    else
                        Console.WriteLine("Invalid date format, keeping current value.");
                }

                Console.Write($"Enter new Ticket Price [{showtime.TicketPrice:C}]: ");
                string priceInput = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(priceInput))
                {
                    if (decimal.TryParse(priceInput, out decimal newPrice) && newPrice > 0)
                        showtime.TicketPrice = newPrice;
                    else
                        Console.WriteLine("Invalid price, keeping current value.");
                }

                bool hasChanges =
                    showtime.MovieID != original.MovieID ||
                    showtime.HallID != original.HallID ||
                    showtime.Showtime != original.Showtime ||
                    showtime.TicketPrice != original.TicketPrice;

                if (!hasChanges)
                {
                    Console.WriteLine("\nNo changes were made. Operation canceled.");
                    Console.WriteLine("\nPress any key to return to the menu...");
                    Console.ReadKey();
                    return;
                }

                Console.WriteLine("\nChanges to be made:");
                if (showtime.MovieID != original.MovieID) Console.WriteLine($"Movie ID: {original.MovieID} → {showtime.MovieID}");
                if (showtime.HallID != original.HallID) Console.WriteLine($"Hall ID: {original.HallID} → {showtime.HallID}");
                if (showtime.Showtime != original.Showtime) Console.WriteLine($"Showtime: {original.Showtime:g} → {showtime.Showtime:g}");
                if (showtime.TicketPrice != original.TicketPrice) Console.WriteLine($"Ticket Price: {original.TicketPrice:C} → {showtime.TicketPrice:C}");

                Console.Write("\nDo you want to update this showtime? (Y/N): ");
                if (Console.ReadLine().Trim().ToUpper() == "Y")
                {
                    _service.UpdateShowtime(showtime);
                    Console.WriteLine("Showtime updated successfully!");
                }
                else
                {
                    Console.WriteLine("Operation canceled.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }

            Console.WriteLine("\nPress any key to return to the menu...");
            Console.ReadKey();
        }

        static void DeleteShowtime()
        {
            Console.Clear();
            Console.WriteLine("=== Delete Showtime ===\n");

            Console.Write("Enter Showtime ID to delete: ");
            if (!int.TryParse(Console.ReadLine(), out int id))
            {
                Console.WriteLine("Invalid ID format. Operation canceled.");
                Console.WriteLine("\nPress any key to return to the menu...");
                Console.ReadKey();
                return;
            }

            try
            {
                ShowtimeDto showtime = _service.GetShowtimeById(id.ToString());

                Console.WriteLine("\nShowtime to delete:");
                Console.WriteLine($"ID: {showtime.ShowtimeID}");
                Console.WriteLine($"Movie: {showtime.MovieTitle} (ID: {showtime.MovieID})");
                Console.WriteLine($"Hall: {showtime.HallName} (ID: {showtime.HallID})");
                Console.WriteLine($"Date & Time: {showtime.Showtime:g}");
                Console.WriteLine($"Ticket Price: {showtime.TicketPrice:C}");

                Console.Write("\nAre you sure you want to delete this showtime? (Y/N): ");
                if (Console.ReadLine().Trim().ToUpper() == "Y")
                {
                    _service.DeleteShowtime(id.ToString());
                    Console.WriteLine("Showtime deleted successfully!");
                }
                else
                {
                    Console.WriteLine("Operation canceled.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }

            Console.WriteLine("\nPress any key to return to the menu...");
            Console.ReadKey();
        }
    }
}