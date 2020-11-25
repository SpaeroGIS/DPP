#define AppGroupName "Sposterezhennya"
#define AppName "Sposterezhennya Довідник"
#define AppVersion "1.5"
#define AppPublisher ""
#define AppURL ""

#define AppInstallTitle "Sposterezhennya.Thesaurus"
#define AppExeName "Sposterezhennya.Thesaurus.exe"
#define AppAlias "Thesaurus"

#define AppInstallTitleAdd "Sposterezhennya.DBeditor"
#define AppExeNameAdd "Sposterezhennya.DbEditor.exe"
#define AppAliasAdd "DbEditor"

#define ROOTSRCINSTALL "e:\Projects\2018\Install\Sposterezhennya\Source"
#define ROOTINSTALL "e:\Projects\2018\Install\Sposterezhennya\Setup"
#define ROOTWORKDIR "Sposterezhennya"

#define nProductInstall "6"
#define basenameinstall "Setup"
#define basenameUninstall "uninstall_"

[Setup]
AppId={{A8B429D4-5F95-4D19-8D95-D756E6D22203}
AppName={#AppName}
AppVersion={#AppVersion}
;AppVerName={#MyAppName} {#MyAppVersion}
AppPublisher={#AppPublisher}
AppPublisherURL={#AppURL}
AppSupportURL={#AppURL}
AppUpdatesURL={#AppURL}
DefaultDirName= {code:GetRootDir}
;DefaultDirName={pf}\{#ROOTWORKDIR}
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

;Sposterezhennya.DBeditor
;Sposterezhennya.Thesaurus

[Languages]
Name: "ukrainian"; MessagesFile: "compiler:Languages\Ukrainian.isl"

[Tasks]
Name: "desktopicon"; Description: "{cm:CreateDesktopIcon}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: unchecked
;Name: "quicklaunchicon"; Description: "{cm:CreateQuickLaunchIcon}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: unchecked; OnlyBelowVersion: 0,6.1

[Files]
Source: "{#ROOTSRCINSTALL}\{#AppInstallTitle}\*"; DestDir: "{code:GetRootDir}\{#AppInstallTitle}"; Flags: ignoreversion recursesubdirs createallsubdirs
Source: "{#ROOTSRCINSTALL}\{#AppInstallTitleAdd}\*"; DestDir: "{code:GetRootDir}\{#AppInstallTitleAdd}"; Flags: ignoreversion recursesubdirs createallsubdirs

[Icons]
;Name: "{group}\{#AppName}"; Filename: "{app}\{#AppInstallTitle}\{#AppExeName}"; IconFilename: "{app}\{#AppInstallTitle}\{#AppExeName}"
Name: "{commondesktop}\{#AppAlias}"; Filename: "{code:GetRootDir}\{#AppInstallTitle}\{#AppExeName}"; IconFilename: "{code:GetRootDir}\{#AppInstallTitle}\{#AppExeName}"; Tasks: desktopicon
Name: "{commondesktop}\{#AppAliasAdd}"; Filename: "{code:GetRootDir}\{#AppInstallTitleAdd}\{#AppExeNameAdd}"; IconFilename: "{code:GetRootDir}\{#AppInstallTitleAdd}\{#AppExeNameAdd}"; Tasks: desktopicon

[Run]
;Filename: "{app}\{#AppInstallTitle}\{#AppExeName}"; Flags: nowait postinstall skipifsilent runascurrentuser; Description: "{cm:LaunchProgram,{#StringChange(AppName, '&', '&&')}}"

[Registry]
Root: "HKLM"; Subkey: "SOFTWARE\WOW6432Node\{#ROOTWORKDIR}"; ValueType: string; ValueName: "{#AppAlias}Path"; ValueData: "{code:GetRootDir}\{#AppInstallTitle}\"; Flags: uninsdeletekeyifempty uninsdeletevalue
Root: "HKLM"; Subkey: "SOFTWARE\WOW6432Node\{#ROOTWORKDIR}"; ValueType: string; ValueName: "{#AppAliasAdd}Path"; ValueData: "{code:GetRootDir}\{#AppInstallTitleAdd}\"; Flags: uninsdeletekeyifempty uninsdeletevalue

[Code]
var
  RootFolder: string;  
  bSetup: boolean;

//function CheckPassword(Password: String): Boolean;
//begin
//  Result := True; 
//end;

function InitializeSetup(): Boolean;
var 
  ResultStr: string;  
  b: bool;
begin
  Log('InitializeSetup called');
  //MsgBox('The installation will now start.', mbInformation, MB_OK);
  b:= RegQueryStringValue(HKEY_LOCAL_MACHINE, 'SOFTWARE\WOW6432Node\Sposterezhennya', 'RootPath', ResultStr);
  if b
    then begin
      RootFolder:= ResultStr;
      Log('OK. Встановлення буду виконано у каталог '+ ResultStr);
      MsgBox('Встановлення буду виконано у каталог'#13 + ResultStr, mbInformation, MB_OK);
    end
    else begin
      RootFolder:= '';
      Log('ERROR. RootRegistry key missing');
      MsgBox('Встановлення потребує попередньго завдання каталогу розміщення.'#13'Path: '+ResultStr+#13'Виконайте поперше Setup_1_Sposterezhennya.Dec.exe', mbInformation, MB_OK)
    end;
  Log('Root folder: ' + RootFolder); 
  bSetup:= b;
  Result:= b;
end;

function GetRootDir(s: string): string;
var 
  ResultStr: string; 
  b: bool;
begin
  Log('GetRootDir called');
  b:= RegQueryStringValue(HKEY_LOCAL_MACHINE, 'SOFTWARE\WOW6432Node\Sposterezhennya', 'RootPath', ResultStr); 
  if b
    then begin
      Log('OK. RegQueryStringValue exists'); 
      //MsgBox('OK. Path from Registry: '+ #13 + ResultStr, mbInformation, MB_OK);
      //Result:= 'e:\_Sposterezhennya';
      Result:= ResultStr;       
    end
    else begin
      Log('ERROR. RegQueryStringValue missing'); 
      //MsgBox('NOK. Path from Registry: '+#13+ResultStr+#13+'Stop install', mbInformation, MB_OK);
      Result:= '';       
    end;  
end;

