using MilSpace.Core.MilSpaceResourceManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MilSpace.Profile.Localization
{
    internal class LocalizationConstants
    {
        private static LocalizationConstants instance;
        private static MilSpaceResourceManager mngr;

        private static string RefreshButtonToolTipKey = "btnRefreshLayersToolTip";
        private static string CommonLengthTextKey = "lblCommonLengthText";
        private static string SelectedPrimitivesTextKey = "lblSelectedPrimitivesText";
        

        private LocalizationConstants()
        {
            mngr = new MilSpaceResourceManager("MilSpace.Profile.Calc");
        }


        public static string RefreshButtonToolTip => Instance.GetLocalization(RefreshButtonToolTipKey);
        public static string CommonLengthText  => Instance.GetLocalization(CommonLengthTextKey);
        public static string SelectedPrimitivesText => Instance.GetLocalization(SelectedPrimitivesTextKey);

        private string GetLocalization(string key)
        {
            return mngr.GetLocalization(key);
        }


        internal static LocalizationConstants Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new LocalizationConstants();
                }

                return instance;
            }
        }

    }
}
