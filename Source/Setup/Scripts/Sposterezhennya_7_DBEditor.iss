#define AppGroupName "СПП MDEC"
#define AppName "СПП MDEC. Редактор довідника"
#define AppVersion "1.5"
#define AppPublisher "ТОВ НВП Агроресурссистеми"
#define AppURL "http:/www.agroresurs.org"

#define AppInstallTitle "DBEditor"
#define AppInstallTitleADD ""

#define AppExeName "DBEditor.exe"
#define ROOTSRCINSTALL "e:\Projects\2018\Install\Source"
#define ROOTINSTALL "e:\Projects\2018\Install\Setup\8-install"
#define ROOTWORKDIR "MilSpace"
#define ArcMapRoot "Desktop10.6"

#define nProductInstall "6"
#define basenameinstall "Setup"
#define basenameUninstall "uninstall_"

[Setup]
AppId={{9FECEFB8-3604-4228-8C6B-D80B31855020}
AppName={#AppName}
AppVersion={#AppVersion}
AppPublisher={#AppPublisher}
AppPublisherURL={#AppURL}
AppSupportURL={#AppURL}
AppUpdatesURL={#AppURL}
DefaultDirName={pf}\{#ROOTWORKDIR}
DisableDirPage=yes
DefaultGroupName={#AppGroupName}
DisableProgramGroupPage=yes
OutputDir={#ROOTINSTALL}
OutputBaseFilename={#basenameinstall}_{#nProductInstall}_{#AppInstallTitle}
Compression=lzma
SetupLogging=True
DiskSpanning=True
DiskSliceSize=2000000000
DisableWelcomePage=False
UninstallDisplayName={#basenameUninstall}{#AppInstallTitle}

[Languages]
Name: "ukrainian"; MessagesFile: "compiler:Languages\Ukrainian.isl"

[Tasks]
Name: "desktopicon"; Description: "{cm:CreateDesktopIcon}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: unchecked
;Name: "quicklaunchicon"; Description: "{cm:CreateQuickLaunchIcon}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: unchecked; OnlyBelowVersion: 0,6.1

[Files]
Source: "{#ROOTSRCINSTALL}\{#AppInstallTitle}\*"; DestDir: "{app}\{#AppInstallTitle}"; Flags: ignoreversion recursesubdirs createallsubdirs

[Icons]
Name: "{group}\{#AppName}"; Filename: "{app}\{#AppInstallTitle}\{#AppExeName}"; IconFilename: "{app}\{#AppInstallTitle}\{#AppExeName}"
Name: "{commondesktop}\{#AppName}"; Filename: "{app}\{#AppInstallTitle}\{#AppExeName}"; IconFilename: "{app}\{#AppInstallTitle}\{#AppExeName}"; Tasks: desktopicon

[Run]
Filename: "{app}\{#AppInstallTitle}\{#AppExeName}"; Flags: nowait postinstall skipifsilent runascurrentuser; Description: "{cm:LaunchProgram,{#StringChange(AppName, '&', '&&')}}"

[Registry]
Root: "HKLM"; Subkey: "SOFTWARE\WOW6432Node\{#ROOTWORKDIR}"; ValueType: string; ValueName: "{#AppInstallTitle}Path"; ValueData: "{app}\{#AppInstallTitle}\"; Flags: uninsdeletekeyifempty uninsdeletevalue

;[Code]
;function CheckPassword(Password: String): Boolean;
;begin
;  if LowerCase(GetMD5OfString(Password)) = LowerCase('d7b3269d1e49a34cc9995e6b56d40dd0') then Result := True;
;end;
