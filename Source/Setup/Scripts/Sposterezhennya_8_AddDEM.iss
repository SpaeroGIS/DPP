#define AppGroupName "Sposterezhennya"
#define AppName "Sposterezhennya AddDem ReliefProcessing"
#define AppVersion "1.5"

#define AppPublisher ""
#define AppURL ""

#define AppInstallTitleADD "Sposterezhennya.AddDem.ReliefProcessing"
#define AppInstallTitle "Sposterezhennya.ReliefProcessing"

#define AppExeName "Sposterezhennya.AddDem.ReliefProcessing.exe"
#define ROOTSRCINSTALL "e:\Projects\2018\Install\Sposterezhennya\Source\AddDEM"
#define ROOTINSTALL "e:\Projects\2018\Install\Sposterezhennya\Setup"
#define ROOTWORKDIR "Sposterezhennya"

#define nProductInstall "7"
#define basenameinstall "Setup"
#define basenameUninstall "uninstall_"

[Setup]
AppId={{9BBC0DF8-849C-402E-B9DD-8E6DC1EB15FF}
AppName={#AppName}
AppVersion={#AppVersion}
;AppVerName={#MyAppName} {#MyAppVersion}
AppPublisher={#AppPublisher}
AppPublisherURL={#AppURL}
AppSupportURL={#AppURL}
AppUpdatesURL={#AppURL}
DefaultDirName={pf}\{#ROOTWORKDIR}
DisableDirPage=no
DefaultGroupName={#AppGroupName}
DisableProgramGroupPage=yes
OutputDir={#ROOTINSTALL}
OutputBaseFilename={#basenameinstall}_{#nProductInstall}_{#AppInstallTitleADD}
Compression=lzma
SetupLogging=True
DiskSpanning=True
DiskSliceSize=2000000000
DisableWelcomePage=False
UninstallDisplayName={#basenameUninstall}{#AppInstallTitleADD}

[Languages]
Name: "ukrainian"; MessagesFile: "compiler:Languages\Ukrainian.isl"

[Tasks]
Name: "desktopicon"; Description: "{cm:CreateDesktopIcon}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: unchecked
Name: "quicklaunchicon"; Description: "{cm:CreateQuickLaunchIcon}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: unchecked; OnlyBelowVersion: 0,6.1

[Files]

Source: "{#ROOTSRCINSTALL}\AddDEM.ReliefProcessing\*"; DestDir: "{app}\{#AppInstallTitle}"; Flags: ignoreversion recursesubdirs createallsubdirs
Source: "{#ROOTSRCINSTALL}\S1-ExternalCatalogTemplate\*"; DestDir: "{app}\S1-ImageStorage"; Flags: ignoreversion recursesubdirs createallsubdirs
Source: "{#ROOTSRCINSTALL}\EngineSNAP\*"; DestDir: "{app}\EngineSNAP"; Flags: ignoreversion recursesubdirs createallsubdirs

[Icons]
Name: "{group}\{#AppName}"; Filename: "{app}\{#AppInstallTitle}\{#AppExeName}"; IconFilename: "{app}\{#AppInstallTitle}\{#AppExeName}"
Name: "{commondesktop}\{#AppName}"; Filename: "{app}\{#AppInstallTitle}\{#AppExeName}"; IconFilename: "{app}\{#AppInstallTitle}\{#AppExeName}"; Tasks: desktopicon

[Run]
;Filename: "{code:GetRootDir}\Addins\gacutil\RegistryGAC.BAT"; Flags: nowait postinstall skipifsilent runascurrentuser; Description: "{cm:LaunchProgram,{#StringChange(AppName, '&', '&&')}}"

[Registry]
Root: "HKLM"; Subkey: "SOFTWARE\WOW6432Node\Sposterezhennya"; ValueType: string; ValueName: "AddDemConfiguration"; ValueData: "{app}\{#AppInstallTitle}"; Flags: uninsdeletekeyifempty uninsdeletevalue

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
  //b:= RegQueryStringValue(HKEY_LOCAL_MACHINE, 'SOFTWARE\WOW6432Node\Sposterezhennya', 'RootPath', ResultStr);
  //if b
//    then begin
      //RootFolder:= ResultStr;
      //Log('OK. Встановлення буду виконано у каталог '+ ResultStr);
      //MsgBox('Встановлення буду виконано у каталог'#13 + ResultStr, mbInformation, MB_OK);
    //end
    //else begin
//      RootFolder:= '';
      //Log('ERROR. RootRegistry key missing');
      //MsgBox('Встановлення потребує попередньго завдання каталогу розміщення.'#13'Path: '+ResultStr+#13'Виконайте поперше Setup_1_Sposterezhennya.Dec.exe', mbInformation, MB_OK)
    //end;
  //Log('Root folder: ' + RootFolder); 
  bSetup:= true;
  Result:= true;
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

procedure DeinitializeSetup();
var
  fn, fnSQL, fnC, fnR: String;
  ResultCode: Integer;
  b, bSQL, bCMD: boolean; 
  ss: TArrayOfString;
  i, ic: integer;
  sl: string; 
  RootFolder: string;  
begin
  Log('DeinitializeSetup called');

  if not bSetup 
  then begin
    Log('Exit witout changes in files');
    exit;
  end;
  RootFolder:= ExpandConstant('{app}');


  fnC:=  RootFolder + '\Sposterezhennya.ReliefProcessing\Sposterezhennya.AddDem.ReliefProcessing.exe.Config';

    if (FileExists(fnC))
  then begin
    Log('OK. File CONFIG found: ' + fnC); 
    b:= LoadStringsFromFile(fnC, ss);
    if b
    then begin
      Log('OK. File CONFIG read to list'); 
      for i:= 0 to GetArrayLength(ss) - 1 do
      begin
        ic := StringChange(ss[i], '[PATH]', RootFolder);
      end;
      if SaveStringsToFile(fnC, ss, false)
      then begin
        Log('OK. Lines from CONFIG file change and safe success');
      end
      else begin
        MsgBox('ERROR. Lines from CONFIG file NOT change success', mbInformation, MB_OK);
      end;
    end
    else begin
      MsgBox('ERROR. File CONF read to list', mbInformation, MB_OK);
    end
  end
  else begin
    Log('ERROR. File CONF not found: ' + fnC); 
    MsgBox('ERROR. Configuarion file Sposterezhennya.AddDem.ReliefProcessing.exe.Config was not found: '#13 + fnC, mbInformation, MB_OK);
  end;
  
  
end;

