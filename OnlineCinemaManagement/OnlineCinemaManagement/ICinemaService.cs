using DataEntity;
using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Web;

namespace OnlineCinemaManagement
{
    [ServiceContract]
    public interface ICinemaService
    {
        [OperationContract]
        [WebGet(ResponseFormat = WebMessageFormat.Json, UriTemplate = "GetShowtimes")]
        List<ShowtimeDto> GetAllShowtimes();

        [OperationContract]
        [WebGet(UriTemplate = "GetShowtime/{id}", ResponseFormat = WebMessageFormat.Json)]
        ShowtimeDto GetShowtimeById(string id);

        [OperationContract]
        [WebInvoke(Method = "POST", RequestFormat = WebMessageFormat.Json,
                   ResponseFormat = WebMessageFormat.Json, UriTemplate = "AddShowtime")]
        void AddShowtime(ShowtimeDto showtimeDto);

        [OperationContract]
        [WebInvoke(Method = "PUT", RequestFormat = WebMessageFormat.Json,
                   ResponseFormat = WebMessageFormat.Json, UriTemplate = "UpdateShowtime")]
        void UpdateShowtime(ShowtimeDto showtimeDto);

        [OperationContract]
        [WebInvoke(Method = "DELETE", ResponseFormat = WebMessageFormat.Json,
                   UriTemplate = "DeleteShowtime/{id}")]
        void DeleteShowtime(string id);

        [OperationContract]
        [WebGet(ResponseFormat = WebMessageFormat.Json, UriTemplate = "GetMovies")]
        List<MovieDto> GetAllMovies();

        [OperationContract]
        [WebGet(ResponseFormat = WebMessageFormat.Json, UriTemplate = "GetHalls")]
        List<HallDto> GetAllHalls();
    }
}