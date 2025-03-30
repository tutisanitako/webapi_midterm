using DataEntity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace OnlineCinemaManagement
{
    [ServiceContract]
    public interface ICinemaService
    {
        [OperationContract]
        [WebGet(ResponseFormat = WebMessageFormat.Json)]
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
    }
}