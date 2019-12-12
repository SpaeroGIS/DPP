using System;

namespace MilSpace.Visibility.DTO
{

    internal enum VeluableTaskSortFieldsEnum : byte
    {
        Title,
        State,
        Created

    }
    internal class VisibilityTasknGui
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public DateTime Created { get; set; }
        public string State {get; set;}
        
    }
}
