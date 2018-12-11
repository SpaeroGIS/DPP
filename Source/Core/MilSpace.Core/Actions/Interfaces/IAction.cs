using MilSpace.Core.Actions.Base;

namespace MilSpace.Core.Actions.Interfaces
{
    public interface IAction<out R>
    {
        R GetResult();

        void Process();

        string ActionId { get; }

        string[] UsedParameters { get; }

        IActionParam[] ParametersTemplate { get; }

        ActionDescription Description { get; }
    }
}
