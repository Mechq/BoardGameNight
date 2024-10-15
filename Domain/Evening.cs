namespace Domain
{
    public class Evening
    {
        public Evening()
        {
            BoardGames = new List<Game>();
            Participants = new List<EveningParticipant>();
        }
        
        
        public Evening(int id, int hostId, int maxUsers, DateOnly hostDate, string? allergy, Address address)
        {
            Id = id;
            HostId = hostId;
            MaxUsers = maxUsers;
            HostDate = hostDate;
            Allergy = allergy;
            Address = address;
        }

        public int Id { get; set; }
        public int HostId { get; set; }
        public User Host { get; set; }
        public int MaxUsers { get; set; }
        public DateOnly HostDate { get; set; }
        public string? Allergy { get; set; }
        public Address Address { get; set; }
        public List<Game> BoardGames { get; set; }
        public List<EveningParticipant> Participants { get; set; } 
    }

}