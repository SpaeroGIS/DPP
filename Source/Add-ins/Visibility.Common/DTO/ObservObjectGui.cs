using MilSpace.DataAccess.DataTransfer;
using System;

namespace MilSpace.Visibility.DTO
{
    internal class ObservObjectGui
    {
        public int ObjectID { get; set; } //Nikol20191128
        public string Id { get; set; }
        public string Title { get; set; }
        public string Group { get; set; }
        public DateTime Created { get; set; }
        public string Affiliation { get; set; }
    }
}
