using MilSpace.DataAccess.DataTransfer;
using MilSpace.DataAccess.Facade;
using MilSpace.Visualization3D.Interfaces;
using MilSpace.Visualization3D.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MilSpace.Visualization3D
{
    internal class DataAccess : IDataAccess
    {
        private readonly LocalizationContext _context;

        private DataAccess() { }

        internal DataAccess(LocalizationContext context)
        {
            _context = context;
        }

        public IList<TreeViewNodeModel> ParseUserProfileSessions()
        {
            var result = new List<TreeViewNodeModel>();
            var userProfileSessions = MilSpaceProfileFacade.GetUserProfileSessions();

            foreach (var profileSession in userProfileSessions)
            {
                if (profileSession.DefinitionType == ProfileSettingsTypeEnum.Points)
                {
                    var model = new TreeViewNodeModel { Name = _context.Line };
                    AddChildCollection(model, profileSession);
                    result.Add(model);
                }

                if (profileSession.DefinitionType == ProfileSettingsTypeEnum.Fun)
                {
                    var model = new TreeViewNodeModel { Name = _context.Fun };
                    AddChildCollection(model, profileSession);
                    result.Add(model);
                }

                if (profileSession.DefinitionType == ProfileSettingsTypeEnum.Primitives)
                {
                    var model = new TreeViewNodeModel { Name = _context.Primitive };
                    AddChildCollection(model, profileSession);
                    result.Add(model);
                }
            }
            return result;
        }

        private void AddChildCollection(TreeViewNodeModel node, ProfileSession profileSession)
        {
            if (node == null || profileSession == null) return;

            node.ChildNodes = new List<TreeViewNodeModel>();

            foreach (var line in profileSession.ProfileLines)
            {
                var azimuth = line.Azimuth.ToString("F0");
                var nodeName = profileSession.DefinitionType == ProfileSettingsTypeEnum.Points
                    ? $"{azimuth}" :
                    (line.Azimuth == double.MinValue ? $"lineDefinition ({System.Array.IndexOf(profileSession.ProfileLines, line) + 1})" :
                    $"{azimuth} ({System.Array.IndexOf(profileSession.ProfileLines, line) + 1})");
                node.ChildNodes.Add(new TreeViewNodeModel { Name = nodeName, NodeProfileSession = profileSession });
            }
        }
    }
}
