using AutoMapper;
using DataEntity;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Linq;
using System.ServiceModel;

namespace OnlineCinemaManagement
{
    public class CinemaService : ICinemaService
    {
        private readonly DataModel _context = new DataModel();
        private readonly IMapper _mapper;

        public CinemaService()
        {
            var config = new MapperConfiguration(cfg => cfg.AddProfile<AutoMapperProfile>());
            _mapper = config.CreateMapper();
        }

        public List<ShowtimeDto> GetAllShowtimes()
        {
            var showtimes = _context.Showtimes.Include("Movies").Include("Halls").ToList();
            return _mapper.Map<List<ShowtimeDto>>(showtimes);
        }

        public ShowtimeDto GetShowtimeById(string id)
        {
            if (!int.TryParse(id, out int showTimeId))
                throw new FaultException($"Invalid ID format: '{id}'.");

            try
            {
                var showtime = _context.Showtimes
                    .Include("Movies")
                    .Include("Halls")
                    .FirstOrDefault(s => s.ShowtimeID == showTimeId);

                if (showtime == null)
                    throw new FaultException($"Showtime with ID {showTimeId} not found.");

                return _mapper.Map<ShowtimeDto>(showtime);
            }
            catch (FaultException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new FaultException($"An unexpected error occurred: {ex.Message}");
            }
        }

        public void AddShowtime(ShowtimeDto showtimeDto)
        {
            if (showtimeDto == null)
                throw new FaultException("Showtime data is required.");

            if (showtimeDto.MovieID == null || showtimeDto.MovieID <= 0)
                throw new FaultException("A valid MovieID is required.");

            if (showtimeDto.HallID == null || showtimeDto.HallID <= 0)
                throw new FaultException("A valid HallID is required.");

            if (showtimeDto.Showtime == default(DateTime))
                throw new FaultException("A valid showtime date and time is required.");

            if (showtimeDto.Showtime < DateTime.Now)
                throw new FaultException("Showtime cannot be in the past.");

            if (showtimeDto.TicketPrice <= 0)
                throw new FaultException("Ticket price must be greater than zero.");

            try
            {
                var showtime = _mapper.Map<Showtimes>(showtimeDto);
                _context.Showtimes.Add(showtime);
                _context.SaveChanges();
            }
            catch (DbUpdateException ex)
            {
                HandleDbUpdateException(ex, showtimeDto);
            }
            catch (Exception ex)
            {
                throw new FaultException($"An unexpected error occurred: {ex.Message}");
            }
        }

        public void UpdateShowtime(ShowtimeDto showtimeDto)
        {
            if (showtimeDto == null)
                throw new FaultException("Showtime data is required.");

            if (showtimeDto.ShowtimeID <= 0)
                throw new FaultException("A valid ShowtimeID is required.");

            if (showtimeDto.MovieID == null || showtimeDto.MovieID <= 0)
                throw new FaultException("A valid MovieID is required.");

            if (showtimeDto.HallID == null || showtimeDto.HallID <= 0)
                throw new FaultException("A valid HallID is required.");

            if (showtimeDto.Showtime == default(DateTime))
                throw new FaultException("A valid showtime date and time is required.");

            if (showtimeDto.Showtime < DateTime.Now)
                throw new FaultException("Showtime cannot be in the past.");

            if (showtimeDto.TicketPrice <= 0)
                throw new FaultException("Ticket price must be greater than zero.");

            try
            {
                var existingShowtime = _context.Showtimes
                    .FirstOrDefault(s => s.ShowtimeID == showtimeDto.ShowtimeID);

                if (existingShowtime == null)
                    throw new FaultException($"Showtime with ID {showtimeDto.ShowtimeID} not found.");

                _mapper.Map(showtimeDto, existingShowtime);
                _context.SaveChanges();
            }
            catch (FaultException)
            {
                throw;
            }
            catch (DbUpdateException ex)
            {
                HandleDbUpdateException(ex, showtimeDto);
            }
            catch (Exception ex)
            {
                throw new FaultException($"An unexpected error occurred: {ex.Message}");
            }
        }

        public void DeleteShowtime(string id)
        {
            if (!int.TryParse(id, out int showTimeId))
                throw new FaultException($"Invalid ID format: '{id}'.");

            try
            {
                var showtime = _context.Showtimes.FirstOrDefault(s => s.ShowtimeID == showTimeId);

                if (showtime == null)
                    throw new FaultException($"Showtime with ID {showTimeId} not found.");

                _context.Showtimes.Remove(showtime);
                _context.SaveChanges();
            }
            catch (FaultException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new FaultException($"An unexpected error occurred: {ex.Message}");
            }
        }

        public List<MovieDto> GetAllMovies()
        {
            var movies = _context.Movies.ToList();
            return _mapper.Map<List<MovieDto>>(movies);
        }

        public List<HallDto> GetAllHalls()
        {
            var halls = _context.Halls.ToList();
            return _mapper.Map<List<HallDto>>(halls);
        }

        private void HandleDbUpdateException(DbUpdateException ex, ShowtimeDto showtimeDto)
        {
            if (ex.InnerException is UpdateException updateEx &&
                updateEx.InnerException is SqlException sqlEx &&
                sqlEx.Number == 547)
            {
                if (sqlEx.Message.Contains("MovieID"))
                    throw new FaultException($"Movie with ID {showtimeDto.MovieID} does not exist.");

                if (sqlEx.Message.Contains("HallID"))
                    throw new FaultException($"Hall with ID {showtimeDto.HallID} does not exist.");

                throw new FaultException("A foreign key constraint violation occurred.");
            }

            throw new FaultException($"A database error occurred: {ex.Message}");
        }
    }
}