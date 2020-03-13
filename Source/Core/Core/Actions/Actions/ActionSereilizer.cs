using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using MilSpace.Core.Actions.Interfaces;
using System.Xml.Serialization;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace MilSpace.Core.Actions.Actions
{
    public class ActionSereilizer
    {
        private Logger _logger = Logger.GetLoggerEx("MilSpace.Core.Actions.Actions.ActionSereilizer");
        IActionParam[] actionParameters = null;
        private string actionId;

        public string ActionId
        {
            get { return actionId; }
            set { actionId = value; }
        }

        public ActionSereilizer(string actionId)
        {
            actionParameters = LoadedActions.GetActionParams(actionId);
        }

        public ActionSereilizer()
        {
        }

        public byte[] Serialize(IActionParam[] paramsToserialise)
        {
            //TODO:: Check params to match to 

            byte[] ba = null;
            try
            {


                using (MemoryStream stream = new MemoryStream())
                {
                    var formatter = new BinaryFormatter();
                    formatter.AssemblyFormat = System.Runtime.Serialization.Formatters.FormatterAssemblyStyle.Simple;
                    formatter.Serialize(stream, paramsToserialise);
                    stream.Seek(0, SeekOrigin.Begin);
                    ba = new byte[stream.Length];
                    stream.Read(ba, 0, (int)stream.Length);
                }
            }
            catch (Exception ex)
            {
                _logger.WarnEx($"> Serialize. Exception: {ex.Message}");
            }


            return ba;
        }

        public IActionParam[] Deserialize(byte[] toDeserialize)
        {
            IActionParam[] ap;

            if (toDeserialize.Length == 0 )
            {
                return null;
            }
            using (MemoryStream stream = new MemoryStream())
            {
                stream.Write(toDeserialize, 0, toDeserialize.Length);
                stream.Seek(0, SeekOrigin.Begin);
                var formatter = new BinaryFormatter();
                formatter.AssemblyFormat = System.Runtime.Serialization.Formatters.FormatterAssemblyStyle.Simple;
                ap = (IActionParam[])formatter.Deserialize(stream);
            }

            return ap;
        }
    }
}
