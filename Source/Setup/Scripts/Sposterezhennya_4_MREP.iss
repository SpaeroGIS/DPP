#define AppGroupName "Sposterezhennya"
#define AppName "Sposterezhennya MRep"
#define AppVersion "1.5"
#define AppPublisher ""
#define AppURL ""

#define AppInstallTitle "Sposterezhennya.ReportApp"
#define AppInstallTitleADD ""

#define AppExeName "Sposterezhennya.ReportApp.exe"
#define ROOTSRCINSTALL "e:\Projects\2018\Sposterezhennya\App"
#define ROOTINSTALL "e:\Projects\2018\Install\Sposterezhennya\Setup"
#define ROOTWORKDIR "Sposterezhennya"

#define nProductInstall "4"
#define basenameinstall "Setup"
#define basenameUninstall "uninstall_"

[Setup]
AppId={{4E3365EE-66F7-471F-AF14-4C630AD478C5}
AppName={#AppName}
AppVersion={#AppVersion}
;AppVerName={#MyAppName} {#MyAppVersion}
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

[Files]
Source: "{#ROOTSRCINSTALL}\{#AppInstallTitle}\*"; DestDir: "{code:GetRootDir}\{#AppInstallTitle}"; Flags: ignoreversion recursesubdirs createallsubdirs
Source: "{#ROOTSRCINSTALL}\reporter\*"; DestDir: "{code:GetRootDir}\Reporter"; Flags: ignoreversion recursesubdirs createallsubdirs
Source: "{#ROOTSRCINSTALL}\Sposterezhennya.AddInReport\SposterezhennyaAddInMR.esriAddIn"; DestDir: "{code:GetRootDir}\Addins"; Flags: ignoreversion recursesubdirs createallsubdirs

[Icons]
;Name: "{group}\{#MyAppName}"; Filename: "{app}\MRep\{#MyAppExeName}"; IconFilename: "{app}\MRep\{#MyAppExeName}"
Name: "{commondesktop}\{#AppName}"; Filename: "{code:GetRootDir}\{#AppInstallTitle}\{#AppExeName}"; IconFilename: "{code:GetRootDir}\{#AppInstallTitle}\{#AppExeName}"; Tasks: desktopicon

[Run]
;Filename: "{app}\MRep\{#AppExeName}"; Flags: nowait postinstall skipifsilent runascurrentuser; Description: "{cm:LaunchProgram,{#StringChange(AppName, '&', '&&')}}"

[Registry]
Root: "HKLM"; Subkey: "SOFTWARE\WOW6432Node\{#ROOTWORKDIR}"; ValueType: string; ValueName: "ReporterPath"; ValueData: "{code:GetRootDir}\Reporter\"; Flags: uninsdeletekeyifempty uninsdeletevalue
Root: "HKLM"; Subkey: "SOFTWARE\WOW6432Node\{#ROOTWORKDIR}"; ValueType: string; ValueName: "ReporterAppPath"; ValueData: "{code:GetRootDir}\{#AppInstallTitle}"; Flags: uninsdeletekeyifempty uninsdeletevalue

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

