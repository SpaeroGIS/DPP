namespace MilSpace.Core.Actions
{
    /// <summary>
    /// Defuines an Action Prosessor's state
    /// </summary>
    public enum ActionStatesEnum : byte
    {
        Idle,
        Initiating,
        Initiated,
        Processing,
        Suspended,
        Finished
    }
}
