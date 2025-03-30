using AutoMapper;
using DataEntity;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace OnlineCinemaManagement
{
    [JsonBehavior]
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
            var showtimes = _context.Showtimes.ToList();
            return _mapper.Map<List<ShowtimeDto>>(showtimes);
        }

        public ShowtimeDto GetShowtimeById(string id)
        {
            int showTimeId = int.Parse(id);

            try
            {
                var showtime = _context.Showtimes.FirstOrDefault(s => s.ShowtimeID == showTimeId);
                if (showtime == null)
                {
                    throw new FaultException($"Showtime with ID {showTimeId} not found.");
                }
                return _mapper.Map<ShowtimeDto>(showtime);
            }
            catch (FaultException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw new FaultException($"An unexpected error occurred: {ex.Message}");
            }
        }

        public void AddShowtime(ShowtimeDto showtimeDto)
        {
            try
            {
                var showtime = _mapper.Map<Showtimes>(showtimeDto);
                _context.Showtimes.Add(showtime);
                _context.SaveChanges();
            }
            catch (DbUpdateException ex)
            {
                if (ex.InnerException is UpdateException updateEx && updateEx.InnerException is SqlException sqlEx)
                {
                    if (sqlEx.Number == 547)
                    {
                        if (sqlEx.Message.Contains("MovieID"))
                        {
                            throw new FaultException($"Movie with ID {showtimeDto.MovieID} does not exist.");
                        }
                        else if (sqlEx.Message.Contains("HallID"))
                        {
                            throw new FaultException($"Hall with ID {showtimeDto.HallID} does not exist.");
                        }
                        else
                        {
                            throw new FaultException("Foreign key constraint violation occurred.");
                        }
                    }
                }
                throw new FaultException($"An error occurred while adding the showtime: {ex.Message}");
            }
            catch (Exception ex)
            {
                throw new FaultException($"An unexpected error occurred: {ex.Message}");
            }
        }

        public void UpdateShowtime(ShowtimeDto showtimeDto)
        {
            try
            {
                var existingShowtime = _context.Showtimes.FirstOrDefault(s => s.ShowtimeID == showtimeDto.ShowtimeID);
                if (existingShowtime == null)
                {
                    throw new FaultException($"Showtime with ID {showtimeDto.ShowtimeID} not found.");
                }
                _mapper.Map(showtimeDto, existingShowtime);
                _context.SaveChanges();
            }
            catch (FaultException ex)
            {
                throw ex;
            }
            catch (DbUpdateException ex)
            {
                if (ex.InnerException is UpdateException updateEx && updateEx.InnerException is SqlException sqlEx)
                {
                    if (sqlEx.Number == 547)
                    {
                        if (sqlEx.Message.Contains("MovieID"))
                        {
                            throw new FaultException($"Movie with ID {showtimeDto.MovieID} does not exist.");
                        }
                        else if (sqlEx.Message.Contains("HallID"))
                        {
                            throw new FaultException($"Hall with ID {showtimeDto.HallID} does not exist.");
                        }
                        else
                        {
                            throw new FaultException("Foreign key constraint violation occurred.");
                        }
                    }
                }
                throw new FaultException($"An error occurred while updating the showtime: {ex.Message}");
            }
            catch (Exception ex)
            {
                throw new FaultException($"An unexpected error occurred: {ex.Message}");
            }
        }

        public void DeleteShowtime(string id)
        {
            // Convert string id from URL to integer
            int showTimeId = int.Parse(id);

            try
            {
                var showtime = _context.Showtimes.FirstOrDefault(s => s.ShowtimeID == showTimeId);
                if (showtime == null)
                {
                    throw new FaultException($"Showtime with ID {showTimeId} not found.");
                }
                _context.Showtimes.Remove(showtime);
                _context.SaveChanges();
            }
            catch (FaultException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw new FaultException($"An unexpected error occurred: {ex.Message}");
            }
        }
    }
}