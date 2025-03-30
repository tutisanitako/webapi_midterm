using CinemaClient.CinemaServiceReference;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CinemaClient
{
    class Program
    {
        static CinemaServiceClient _service = new CinemaServiceClient();

        static void Main(string[] args)
        {
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
                            case 1:
                                ViewAllShowtimes();
                                break;
                            case 2:
                                ViewShowtimeById();
                                break;
                            case 3:
                                AddShowtime();
                                break;
                            case 4:
                                UpdateShowtime();
                                break;
                            case 5:
                                DeleteShowtime();
                                break;
                            case 0:
                                exit = true;
                                break;
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
                    ShowtimeDto showtime = _service.GetShowtimeById(id);

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

            ShowtimeDto newShowtime = new ShowtimeDto();

            Console.Write("Enter Movie ID: ");
            if (int.TryParse(Console.ReadLine(), out int movieId))
            {
                newShowtime.MovieID = movieId;

                try
                {
                    var movieTitle = FindMovieTitleById(movieId);
                    if (!string.IsNullOrEmpty(movieTitle))
                    {
                        Console.WriteLine($"Selected Movie: {movieTitle}");
                        newShowtime.MovieTitle = movieTitle;
                    }
                    else
                    {
                        Console.WriteLine("Warning: Could not verify movie. The operation might fail.");
                    }
                }
                catch
                {
                    Console.WriteLine("Warning: Could not verify movie. The operation might fail.");
                }
            }
            else
            {
                Console.WriteLine("Invalid Movie ID. Operation canceled.");
                Console.WriteLine("\nPress any key to return to the menu...");
                Console.ReadKey();
                return;
            }

            Console.Write("Enter Hall ID: ");
            if (int.TryParse(Console.ReadLine(), out int hallId))
            {
                newShowtime.HallID = hallId;

                try
                {
                    var hallName = FindHallNameById(hallId);
                    if (!string.IsNullOrEmpty(hallName))
                    {
                        Console.WriteLine($"Selected Hall: {hallName}");
                        newShowtime.HallName = hallName;
                    }
                    else
                    {
                        Console.WriteLine("Warning: Could not verify hall. The operation might fail.");
                    }
                }
                catch
                {
                    Console.WriteLine("Warning: Could not verify hall. The operation might fail.");
                }
            }
            else
            {
                Console.WriteLine("Invalid Hall ID. Operation canceled.");
                Console.WriteLine("\nPress any key to return to the menu...");
                Console.ReadKey();
                return;
            }

            Console.Write("Enter Showtime (yyyy-MM-dd HH:mm): ");
            if (DateTime.TryParse(Console.ReadLine(), out DateTime showtime))
            {
                newShowtime.Showtime = showtime;
            }
            else
            {
                Console.WriteLine("Invalid date format. Operation canceled.");
                Console.WriteLine("\nPress any key to return to the menu...");
                Console.ReadKey();
                return;
            }

            Console.Write("Enter Ticket Price: ");
            if (decimal.TryParse(Console.ReadLine(), out decimal price))
            {
                newShowtime.TicketPrice = price;
            }
            else
            {
                Console.WriteLine("Invalid price format. Operation canceled.");
                Console.WriteLine("\nPress any key to return to the menu...");
                Console.ReadKey();
                return;
            }

            Console.WriteLine("\nReview your showtime information:");
            Console.WriteLine($"Movie: {newShowtime.MovieTitle ?? "Unknown"} (ID: {newShowtime.MovieID})");
            Console.WriteLine($"Hall: {newShowtime.HallName ?? "Unknown"} (ID: {newShowtime.HallID})");
            Console.WriteLine($"Showtime: {newShowtime.Showtime:g}");
            Console.WriteLine($"Ticket Price: {newShowtime.TicketPrice:C}");

            Console.Write("\nDo you want to add this showtime? (Y/N): ");
            string confirmation = Console.ReadLine().Trim().ToUpper();

            if (confirmation == "Y")
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
                ShowtimeDto showtime = _service.GetShowtimeById(id);
                ShowtimeDto originalShowtime = new ShowtimeDto
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
                if (!string.IsNullOrWhiteSpace(movieIdInput) && int.TryParse(movieIdInput, out int movieId))
                {
                    showtime.MovieID = movieId;

                    try
                    {
                        var movieTitle = FindMovieTitleById(movieId);
                        if (!string.IsNullOrEmpty(movieTitle))
                        {
                            Console.WriteLine($"Selected Movie: {movieTitle}");
                            showtime.MovieTitle = movieTitle;
                        }
                        else
                        {
                            Console.WriteLine("Warning: Could not verify movie. The operation might fail.");
                        }
                    }
                    catch
                    {
                        Console.WriteLine("Warning: Could not verify movie. The operation might fail.");
                    }
                }

                Console.Write($"Enter new Hall ID [{showtime.HallID}]: ");
                string hallIdInput = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(hallIdInput) && int.TryParse(hallIdInput, out int hallId))
                {
                    showtime.HallID = hallId;

                    try
                    {
                        var hallName = FindHallNameById(hallId);
                        if (!string.IsNullOrEmpty(hallName))
                        {
                            Console.WriteLine($"Selected Hall: {hallName}");
                            showtime.HallName = hallName;
                        }
                        else
                        {
                            Console.WriteLine("Warning: Could not verify hall. The operation might fail.");
                        }
                    }
                    catch
                    {
                        Console.WriteLine("Warning: Could not verify hall. The operation might fail.");
                    }
                }

                Console.Write($"Enter new Showtime [{showtime.Showtime:g}] (yyyy-MM-dd HH:mm): ");
                string showtimeInput = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(showtimeInput) && DateTime.TryParse(showtimeInput, out DateTime showtimeDate))
                {
                    showtime.Showtime = showtimeDate;
                }

                Console.Write($"Enter new Ticket Price [{showtime.TicketPrice:C}]: ");
                string priceInput = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(priceInput) && decimal.TryParse(priceInput, out decimal price))
                {
                    showtime.TicketPrice = price;
                }

                bool hasChanges =
                    showtime.MovieID != originalShowtime.MovieID ||
                    showtime.HallID != originalShowtime.HallID ||
                    showtime.Showtime != originalShowtime.Showtime ||
                    showtime.TicketPrice != originalShowtime.TicketPrice;

                if (!hasChanges)
                {
                    Console.WriteLine("\nNo changes were made. Operation canceled.");
                    Console.WriteLine("\nPress any key to return to the menu...");
                    Console.ReadKey();
                    return;
                }

                Console.WriteLine("\nChanges to be made:");

                if (showtime.MovieID != originalShowtime.MovieID)
                {
                    Console.WriteLine($"Movie: {originalShowtime.MovieTitle} (ID: {originalShowtime.MovieID}) → {showtime.MovieTitle ?? "Unknown"} (ID: {showtime.MovieID})");
                }

                if (showtime.HallID != originalShowtime.HallID)
                {
                    Console.WriteLine($"Hall: {originalShowtime.HallName} (ID: {originalShowtime.HallID}) → {showtime.HallName ?? "Unknown"} (ID: {showtime.HallID})");
                }

                if (showtime.Showtime != originalShowtime.Showtime)
                {
                    Console.WriteLine($"Showtime: {originalShowtime.Showtime:g} → {showtime.Showtime:g}");
                }

                if (showtime.TicketPrice != originalShowtime.TicketPrice)
                {
                    Console.WriteLine($"Ticket Price: {originalShowtime.TicketPrice:C} → {showtime.TicketPrice:C}");
                }

                Console.Write("\nDo you want to update this showtime? (Y/N): ");
                string confirmation = Console.ReadLine().Trim().ToUpper();

                if (confirmation == "Y")
                {
                    try
                    {
                        _service.UpdateShowtime(showtime);
                        Console.WriteLine("Showtime updated successfully!");
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
                ShowtimeDto showtime = _service.GetShowtimeById(id);

                Console.WriteLine("\nShowtime to delete:");
                Console.WriteLine($"ID: {showtime.ShowtimeID}");
                Console.WriteLine($"Movie: {showtime.MovieTitle} (ID: {showtime.MovieID})");
                Console.WriteLine($"Hall: {showtime.HallName} (ID: {showtime.HallID})");
                Console.WriteLine($"Date & Time: {showtime.Showtime:g}");
                Console.WriteLine($"Ticket Price: {showtime.TicketPrice:C}");

                Console.Write("\nAre you sure you want to delete this showtime? (Y/N): ");
                string confirmation = Console.ReadLine().Trim().ToUpper();

                if (confirmation == "Y")
                {
                    try
                    {
                        _service.DeleteShowtime(id);
                        Console.WriteLine("Showtime deleted successfully!");
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
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }

            Console.WriteLine("\nPress any key to return to the menu...");
            Console.ReadKey();
        }

        private static string FindMovieTitleById(int movieId)
        {
            var showtimes = _service.GetAllShowtimes();
            var showtime = showtimes.FirstOrDefault(s => s.MovieID == movieId);
            return showtime?.MovieTitle;
        }

        private static string FindHallNameById(int hallId)
        {
            var showtimes = _service.GetAllShowtimes();
            var showtime = showtimes.FirstOrDefault(s => s.HallID == hallId);
            return showtime?.HallName;
        }
    }
}