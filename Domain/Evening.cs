using System.Text.Json.Serialization;

namespace Domain
{
    public class Evening
    {
        public Evening()
        {
            Games = new List<EveningGame>();
            Participants = new List<EveningParticipant>();
        }
        
        public Evening(int id, string hostId, int maxUsers, DateOnly hostDate, string? allergy, Address address)
        {
            Id = id;
            HostId = hostId; 
            MaxUsers = maxUsers;
            HostDate = hostDate;
            Allergy = allergy;
            Address = address;
        }

        public int Id { get; set; }
        public string? HostId { get; set; }
        public int MaxUsers { get; set; }
        public DateOnly HostDate { get; set; }
        public string? Allergy { get; set; }
        public Address Address { get; set; }
        public int AddressId { get; set; }  
        
        [JsonIgnore] 
        public List<EveningGame> Games { get; set; }
        
        [JsonIgnore] 
        public List<EveningParticipant> Participants { get; set; } 
    }
}