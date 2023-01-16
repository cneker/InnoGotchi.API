using InnoGotchi.Domain.Enums;

namespace InnoGotchi.Domain.Entities
{
    public class PetBody
    {
        public Guid Id { get; set; }
        public Body Body { get; set; }
        public Eye Eye { get; set; }
        public Nose Nose { get; set; }
        public Mouth Mouth { get; set; }
        public Guid UserId { get; set; }

        public User User { get; set; }
    }
}
