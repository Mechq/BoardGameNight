namespace Domain;

public class EveningParticipant
{

        public int EveningId { get; set; }
        public Evening Evening { get; set; }

        public int ParticipantId { get; set; }
        public User Participant { get; set; }
    
}

