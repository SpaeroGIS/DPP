using MilSpace.Core.Actions.Base;

namespace MilSpace.Core.Actions
{
    public delegate void ActionProcessDelegate(ActionDescription actionDesctiption, ActionStatesEnum state);

    public delegate void ActionProcessCommandLineDelegate(string consoleMessage, ActironCommandLineStatesEnum state);

}
