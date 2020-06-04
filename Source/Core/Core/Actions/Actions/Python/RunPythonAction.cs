using MilSpace.Configurations;
using MilSpace.Core.Actions.ActionResults;
using MilSpace.Core.Actions.Base;
using MilSpace.Core.Actions.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace MilSpace.Core.Actions.Actions.Pythom
{
    public class RunPythonAction : MilSpace.Core.Actions.Base.Action<StringActionResult>
    {
        IEnumerable<string> pytonParameters;

        public RunPythonAction()
        {
        }

        public RunPythonAction(IActionProcessor parameters)
            : base(parameters)
        {
            logger.InfoEx("Initiating \"Launch Python script\" action.");

            pytonParameters = parameters.GetParameters<string>(ActionParamNamesCore.DataValue, string.Empty).Select(p => p.Value);

            if (pytonParameters.Count() == 1 && string.IsNullOrEmpty(pytonParameters.First()))
            {
                pytonParameters = new string[0];
            }

            this.returnResult.Result = string.Empty;
        }

        public override string ActionId => ActionsCore.RunPythot;

        public override IActionParam[] ParametersTemplate
        {
            get
            {
                return new IActionParam[]
                {
                    new ActionParam<string>() { ParamName = ActionParamNamesCore.DataValue, Value = string.Empty},
                };
            }
        }

        public override StringActionResult GetResult()
        {
            return this.returnResult;
        }

        public override void Process()
        {
        
            try
            {
                if (pytonParameters.Count() == 0)
                {
                    this.returnResult.Exception = new ArgumentException("The python script file was not pointed.");
                    return;
                }

                string scriptFileName = Path.Combine(ParthToScriptStorage, pytonParameters.First());

                if (!File.Exists(scriptFileName))
                {
                    this.returnResult.Exception = new ArgumentException("The python script file {0} was not found.".InvariantFormat(scriptFileName));
                    return;
                }

                var paramsToScript = pytonParameters.ToArray();

                paramsToScript[0] = scriptFileName;

                ProcessStartInfo info = new ProcessStartInfo();
                info.FileName = PythonExe;

                info.Arguments = string.Join(" ", paramsToScript.ToArray());//scriptFileName + string.Format(" {0} \"{1}\" \"{2}\" \"{3}\"", key, "2016-03-31", "2016-06-01", "R");//args is path to .py file and any cmd line args
                info.UseShellExecute = false;
                info.RedirectStandardOutput = true;
                info.RedirectStandardError = true;

                //if (false)
                //{
                //    info.UserName = "srvdev\administrator";

                //    SecureString ssPwd = new SecureString();
                //    string password = "LLAWERIFFF";

                //    for (int x = 0; x < password.Length; x++)
                //    {
                //        ssPwd.AppendChar(password[x]);
                //    }
                //    info.Password = ssPwd;
                //}

                using (Process process = System.Diagnostics.Process.Start(info))
                {
                    process.OutputDataReceived += Process_OutputDataReceived;
                    process.ErrorDataReceived += Process_ErrorDataReceived;

                    process.BeginOutputReadLine();
                    process.BeginErrorReadLine();

                    //using (StreamReader reader = process.StandardOutput)
                    //{
                    //    using (StreamReader srq = process.StandardError)
                    //    {
                    //        this.returnResult.Result = reader.ReadToEnd();

                    //        logger.InfoEx(this.returnResult.Result);

                    //        string srqText = srq.ReadToEnd();
                    //        if (srqText != null && srqText.Length > 0)
                    //        {
                    //            logger.ErrorEx(srqText);
                    //        }
                    //        process.WaitForExit();
                    //    }
                    //}
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
            logger.ErrorEx(e.Data);
        }

        private void Process_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            logger.InfoEx(e.Data);
        }

        public override ActionDescription Description
        {
            get
            {
                if (description.IsEmpty)
                {
                    description.Description = "Launches Pyton scrips.";
                };
                return base.Description;
            }
        }

        private static string PythonExe
        {
            get
            {
                if (!File.Exists(MilSpaceConfiguration.PythonConfigurationPoperty.PythonExecutable))
                {
                    throw new FileNotFoundException(MilSpaceConfiguration.PythonConfigurationPoperty.PythonExecutable);
                }

                return MilSpaceConfiguration.PythonConfigurationPoperty.PythonExecutable;
            }
        }

        private static string ParthToScriptStorage
        {
            get
            {
                if (!Directory.Exists(MilSpaceConfiguration.PythonConfigurationPoperty.PythonScriptsFolder))
                {
                    throw new DirectoryNotFoundException(MilSpaceConfiguration.PythonConfigurationPoperty.PythonScriptsFolder);
                }

                return MilSpaceConfiguration.PythonConfigurationPoperty.PythonScriptsFolder;
            }
        }
    }
}
