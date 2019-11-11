using System;

namespace MilSpace.DataAccess.DataTransfer
{
    public class ObservationObject
    {
        public int ObjectId;
        public string Id;
        public string Group;
        public string Title;
        public bool Shared;
        public DateTime DTO;
        public ObservationObjectTypesEnum ObjectType;
        public string Creator;
    }
}
