using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace MilSpace.Core.ModulesInteraction
{
    public class ModuleInteraction
    {
        private static Logger logger = Logger.GetLoggerEx("ModuleInteraction");

        private static Dictionary<ModulesEnum, Type> moduleInteractions = new Dictionary<ModulesEnum, Type>()
        {
            {ModulesEnum.GeoCalculator, typeof(IGeocalculatorInteraction) },
            {ModulesEnum.Profile, typeof(IProfileInteraction) },
            {ModulesEnum.Visibility, typeof(IVisibilityInteraction) },
            {ModulesEnum.Visualization3D, typeof(I3DVisualizationInteraction) }
        };


        private static Hashtable moduleregistration = new Hashtable()
        {
            { typeof(IGeocalculatorInteraction), null },
            { typeof(IProfileInteraction), null },
            { typeof(IVisibilityInteraction), null },
            { typeof(I3DVisualizationInteraction), null }
        };

        private static readonly ModuleInteraction instance = new ModuleInteraction();

        private ModuleInteraction()
        {
            logger.InfoEx("Initiate Module interaction");
        }

        public static ModuleInteraction Instance => instance;

        public void RegisterModuleInteraction<T>(T interaction)
        {

            if (!moduleregistration.ContainsKey(typeof(T)))
            {
                logger.InfoEx("Type {0} cannot be used as interaction".InvariantFormat(typeof(T)));
                throw new NotFiniteNumberException("Type {0} cannot be used as interaction".InvariantFormat(typeof(T)));
            }

            var moduleName = moduleInteractions.First(t => t.Value == typeof(T)).Key;
            if (moduleregistration[typeof(T)] != null)
            {
                logger.InfoEx("Module {0} was already registered. Update interaction instance".InvariantFormat(moduleName));
            }
            else
            {
                logger.InfoEx("Register module {0}".InvariantFormat(moduleName));
            }
            moduleregistration[typeof(T)] = interaction;

            return;
        }

        public T GetModilInteraction<T>(out bool change )
        {
            change = false;
            if (moduleregistration.ContainsKey(typeof(T)) && moduleregistration[typeof(T)] != null)
            {
                change = true;
                return (T) Convert.ChangeType(moduleregistration[typeof(T)], typeof(T));
            }

            throw new NotFiniteNumberException($"Type {typeof(T)} is not registered as  module interaction."); 
        }
    }
}
