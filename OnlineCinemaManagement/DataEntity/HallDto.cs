using System.Runtime.Serialization;

namespace DataEntity
{
    [DataContract]
    public class HallDto
    {
        [DataMember] public int HallID { get; set; }
        [DataMember] public string HallName { get; set; }
    }
}