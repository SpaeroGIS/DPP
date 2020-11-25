#define AppGroupName "Sposterezhennya"
#define AppName "Sposterezhennya Картографічні дані"
#define AppVersion "1.5"
#define AppPublisher ""
#define AppURL ""

#define AppInstallTitle "Sposterezhennya.Vector DATA"
#define AppInstallTitleADD "Vector"
#define MyAppExeName ""

#define ROOTSRCINSTALL "e:\Projects\2018\Install\Sposterezhennya\Source"
#define ROOTSRCDATA "e:\Projects\2018\Install\Sposterezhennya\Source\DATA"
#define ROOTINSTALL "e:\Projects\2018\Install\Sposterezhennya\Setup"
#define ROOTWORKDIR "Sposterezhennya"

#define nProductInstall "2"
#define basenameinstall "Setup"
#define basenameUninstall "uninstall_"

[Setup]
AppId={{11B2A73A-F87C-457C-9374-74F0E1B3FCCD}
AppName={#AppName}
AppVersion={#AppVersion}
;AppVerName={#AppName} {#MyAppVersion}
AppPublisher={#AppPublisher}
AppPublisherURL={#AppURL}
AppSupportURL={#AppURL}
AppUpdatesURL={#AppURL}
;DefaultDirName={pf}\{#ROOTWORKDIR}
DefaultDirName= {code:GetRootDir}
DisableDirPage=yes
DefaultGroupName={#AppGroupName}
DisableProgramGroupPage=yes
OutputDir={#ROOTINSTALL}
OutputBaseFilename={#basenameinstall}_{#nProductInstall}_{#AppInstallTitle}.{#AppInstallTitleADD}
Compression=lzma
SetupLogging=True
DiskSpanning=True
DiskSliceSize= 2100000000
DisableWelcomePage=False
UninstallDisplayName={#basenameUninstall}{#AppInstallTitle}
UninstallFilesDir={code:GetRootDir}

[Languages]
Name: "ukrainian"; MessagesFile: "compiler:Languages\Ukrainian.isl"

[Tasks]
;Name: "desktopicon"; Description: "{cm:CreateDesktopIcon}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: unchecked
;Name: "quicklaunchicon"; Description: "{cm:CreateQuickLaunchIcon}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: unchecked; OnlyBelowVersion: 0,6.1

[Files]
Source: "{#ROOTSRCDATA}\Vector\*"; DestDir: "{code:GetRootDir}\DATA\Vector"; Flags: ignoreversion recursesubdirs createallsubdirs
Source: "{#ROOTSRCDATA}\Styles\*"; DestDir: "{code:GetRootDir}\DATA\Styles"; Flags: ignoreversion recursesubdirs createallsubdirs
Source: "{#ROOTSRCDATA}\MapTemplate\*"; DestDir: "{code:GetRootDir}\DATA\MapTemplate"; Flags: ignoreversion recursesubdirs createallsubdirs

[Icons]
;Name: "{group}\{#MyAppName}"; Filename: "{app}\Decryptor\{#MyAppExeName}"; IconFilename: "{app}\Decryptor\{#MyAppExeName}"
;Name: "{commondesktop}\{#MyAppName}"; Filename: "{app}\Decryptor\{#MyAppExeName}"; IconFilename: "{app}\Decryptor\{#MyAppExeName}"; Tasks: desktopicon
;Name: "{userappdata}\Microsoft\Internet Explorer\Quick Launch\{#MyAppName}"; Filename: "{app}\Decryptor\{#MyAppExeName}"; Tasks: quicklaunchicon

[Run]
;Filename: "{app}\Decryptor\{#MyAppExeName}"; Flags: nowait postinstall skipifsilent runascurrentuser; Description: "{cm:LaunchProgram,{#StringChange(MyAppName, '&', '&&')}}"

[Registry]
;Root: "HKLM"; Subkey: "SOFTWARE\WOW6432Node\{#ROOTWORKDIR}"; ValueType: string; ValueName: "{#AppInstallTitle}Path"; ValueData: "{app}\{#AppInstallTitle}\"; Flags: createvalueifdoesntexist

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