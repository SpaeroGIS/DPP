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
        private const string Degree = "°";
        private readonly LocalizationContext _context;

        private DataAccess() { }

        internal DataAccess(LocalizationContext context)
        {
            _context = context;
        }

        internal TreeViewModel ParseUserProfileSessions()
        {
            var result = new TreeViewModel();
            var userProfileSessions = MilSpaceProfileFacade.GetUserProfileSessions();

            foreach (var profileSession in userProfileSessions)
            {
                var model = new TreeViewNodeModel { Name = profileSession.SessionName, Guid = Guid.NewGuid(), NodeProfileSession = profileSession };
                AddChildCollection(model, profileSession);

                if (profileSession.DefinitionType == ProfileSettingsTypeEnum.Points)
                {
                    result.Lines.Add(model);
                }

                if (profileSession.DefinitionType == ProfileSettingsTypeEnum.Fun)
                {
                    result.Funs.Add(model);
                }

                if (profileSession.DefinitionType == ProfileSettingsTypeEnum.Primitives)
                {
                    result.Primitives.Add(model);
                }
            }
            return result;
        }

        internal IEnumerable<VisibilitySession> GetVisibilitySessions()
        {
            return VisibilityZonesFacade.GetAllVisibilitySessions();
        }

        private void AddChildCollection(TreeViewNodeModel node, ProfileSession profileSession)
        {
            if (node == null || profileSession == null) return;

            node.ChildNodes = new List<TreeViewNodeModel>();

            foreach (var line in profileSession.ProfileLines)
            {
                var azimuth = line.Azimuth.ToString("F0");
                var nodeName = profileSession.DefinitionType == ProfileSettingsTypeEnum.Points
                    ? $"{azimuth}{Degree}" :
                    (line.Azimuth == double.MinValue ? $"{_context.Profile}: ({Array.IndexOf(profileSession.ProfileLines, line) + 1})" :
                    $"{azimuth}{Degree} ({System.Array.IndexOf(profileSession.ProfileLines, line) + 1})");
                node.ChildNodes.Add(new TreeViewNodeModel { Name = nodeName, NodeProfileSession = profileSession });
            }
        }
    }
}
