using MilSpace.Profile.DTO;
using System;


namespace MilSpace.Profile
{
    public class ProfileSettingsEventArgs : EventArgs
    {
        public ProfileSettingsEventArgs(ProfileSettings profileSetting, ProfileSettingsTypeEnum settingsType)
        {
            ProfileSetting = profileSetting;
            ProfileSettingsType = settingsType;
        }
        public ProfileSettings ProfileSetting;

        public ProfileSettingsTypeEnum ProfileSettingsType;
    }
}
