using System;

namespace MilSpace.Visibility.DTO
{

    internal enum VeluableObservObjectSortFieldsEnum : byte
    {
        Title = 0,
        Type = 1,
        Affiliation = 2,
        Date = 4,
        Group = 5,
        Id

    }

    internal class ObservObjectGui
    {
        public int ObjectID { get; set; } //Nikol20191128
        public string Id { get; set; }
        public string Title { get; set; }
        public string Group { get; set; }
        public string Affiliation { get; set; }
        public DateTime Created { get; set; }

    }
}
