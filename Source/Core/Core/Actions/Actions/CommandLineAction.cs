using MilSpace.Core.Actions.ActionResults;
using MilSpace.Core.Actions.Base;
using MilSpace.Core.Actions.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace MilSpace.Core.Actions.Actions
{
    public class CommandLineAction : MilSpace.Core.Actions.Base.Action<StringActionResult>
    {

        private string executableFile;
        private string workingDirectory;
        IEnumerable<string> commandArgs;
        ActionProcessCommandLineDelegate onOutput = null;
        ActionProcessCommandLineDelegate onError = null;

        bool gotError = false;

        public CommandLineAction()
        { }

        public CommandLineAction(IActionProcessor parameters)
                   : base(parameters)
        {
            executableFile = parameters.GetParameter(ActionParamNamesCore.PathToFile, string.Empty).Value;
            workingDirectory = parameters.GetParameter(ActionParamNamesCore.WorkingDirectory, string.Empty).Value;
            commandArgs = parameters.GetParameters<string>(ActionParamNamesCore.DataValue, string.Empty).Select(p => p.Value);
            onOutput = parameters.GetParameter<ActionProcessCommandLineDelegate>(ActionParamNamesCore.OutputDataReceivedDelegate, null).Value;
            onError = parameters.GetParameter<ActionProcessCommandLineDelegate>(ActionParamNamesCore.ErrorDataReceivedDelegate, null).Value;
        }

        public override string ActionId => ActionsCore.RunCommandLine;

        public override IActionParam[] ParametersTemplate
        {
            get
            {
                return new IActionParam[]
                {
                    new ActionParam<string>() { ParamName = ActionParamNamesCore.PathToFile, Value = string.Empty},
                    new ActionParam<string>() { ParamName = ActionParamNamesCore.DataValue, Value = string.Empty},
                    new ActionParam<string>() { ParamName = ActionParamNamesCore.WorkingDirectory, Value = string.Empty},
                    new ActionParam<ActionProcessCommandLineDelegate>() { ParamName = ActionParamNamesCore.OutputDataReceivedDelegate, Value = null},
                    new ActionParam<ActionProcessCommandLineDelegate>() { ParamName = ActionParamNamesCore.ErrorDataReceivedDelegate, Value = null}
                };
            }
        }


        public override StringActionResult GetResult()
        {
            return returnResult;
        }

        public override void Process()
        {

            try
            {
                if (string.IsNullOrWhiteSpace(executableFile))
                {
                    this.returnResult.Exception = new ArgumentException($"File name cannpt be empty");
                    return;
                }

                if (!File.Exists(executableFile))
                {
                    this.returnResult.Exception = new ArgumentException("The python script file {0} was not found.".InvariantFormat(executableFile));
                    return;
                }


                ProcessStartInfo info = new ProcessStartInfo();
                if (!string.IsNullOrEmpty(workingDirectory))
                {
                    if (Directory.Exists(workingDirectory))
                    {
                        info.WorkingDirectory = workingDirectory;
                    }
                    else
                    {
                        logger.WarnEx($"Directory {workingDirectory} does not exist.");
                    }
                }

                info.FileName = executableFile;

                info.Arguments = string.Join(" ", commandArgs.ToArray());//scriptFileName + string.Format(" {0} \"{1}\" \"{2}\" \"{3}\"", key, "2016-03-31", "2016-06-01", "R");//args is path to .py file and any cmd line args
                info.UseShellExecute = false;
                info.RedirectStandardOutput = true;
                info.RedirectStandardError = true;

                using (Process process = System.Diagnostics.Process.Start(info))
                {
                    process.OutputDataReceived += Process_OutputDataReceived;
                    process.ErrorDataReceived += Process_ErrorDataReceived;

                    process.BeginOutputReadLine();
                    process.BeginErrorReadLine();

                    process.WaitForExit();
                }
            }
            catch (Exception ex)
            {
                this.returnResult.Exception = ex;
                return;
            }
        }

        private void Process_ErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            onError?.Invoke(e.Data, ActironCommandLineStatesEnum.Error);
            logger.ErrorEx(e.Data);
        }

        private void Process_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            onOutput?.Invoke(e.Data, ActironCommandLineStatesEnum.Output);
            logger.InfoEx(e.Data);
        }


        private const string commandLineDescription = "Run command line";
        public override ActionDescription Description
        {
            get
            {
                if (string.IsNullOrWhiteSpace(description.Description))
                {
                    description.Description = commandLineDescription;
                }

                return description;
            }
        }

    }
}

