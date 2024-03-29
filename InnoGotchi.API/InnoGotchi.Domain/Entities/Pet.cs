﻿using InnoGotchi.Domain.Enums;

namespace InnoGotchi.Domain.Entities
{
    public class Pet
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public DateTime Birthday { get; set; }
        public DateTime? DeathDay { get; set; }
        public double HappinessDayCount { get; set; }
        public HungerLevel HungerLevel { get; set; }
        public ThirstyLevel ThirstyLevel { get; set; }
        public Body Body { get; set; }
        public Eye Eye { get; set; }
        public Nose Nose { get; set; }
        public Mouth Mouth { get; set; }
        public Guid FarmId { get; set; }

        public Farm Farm { get; set; }
        public ICollection<HungryStateChanges> HungryStateChangesHistory { get; set; }
        public ICollection<ThirstyStateChanges> ThirstyStateChangesHistory { get; set; }
    }
}
