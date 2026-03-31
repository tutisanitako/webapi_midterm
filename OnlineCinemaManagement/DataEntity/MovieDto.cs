using System.Runtime.Serialization;

namespace DataEntity
{
    [DataContract]
    public class MovieDto
    {
        [DataMember] public int MovieID { get; set; }
        [DataMember] public string Title { get; set; }
    }
}