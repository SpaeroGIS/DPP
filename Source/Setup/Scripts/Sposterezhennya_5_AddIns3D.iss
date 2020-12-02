#define AppGroupName "Sposterezhennya"
#define AppName "Sposterezhennya AddIns3D"
#define AppVersion "1.5"
#define AppPublisher ""
#define AppURL ""

#define AppInstallTitle "Sposterezhennya.AddIns3D"

#define ROOTSRCINSTALL "e:\Projects\2018\Install\Sposterezhennya\Source\Addin3D"
#define ROOTADDDEMINSTALL "e:\Projects\2018\Install\Sposterezhennya\Source\AddDEM"
#define ROOTSRCDATA "e:\Projects\2018\Install\Sposterezhennya\Source\DATA"
#define ROOTINSTALL "e:\Projects\2018\Install\Sposterezhennya\Setup"
#define ROOTWORKDIR "Sposterezhennya"

#define nProductInstall "5"
#define basenameinstall "Setup"
#define basenameUninstall "uninstall_"

[Setup]
AppId={{EE5C779D-405F-4D12-B10C-7F1F84E64CED}
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
;Name: "desktopicon"; Description: "{cm:CreateDesktopIcon}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: unchecked
;Name: "quicklaunchicon"; Description: "{cm:CreateQuickLaunchIcon}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: unchecked; OnlyBelowVersion: 0,6.1

[Files]
Source: "{#ROOTSRCINSTALL}\*"; DestDir: "{code:GetRootDir}\Addins"; Flags: ignoreversion recursesubdirs createallsubdirs

Source: "{#ROOTSRCDATA}\GDB\*"; DestDir: "{code:GetRootDir}\DATA\GDB"; Flags: ignoreversion recursesubdirs createallsubdirs

Source: "{#ROOTSRCDATA}\DB\DNOEGDB.mdf"; DestDir: "{code:GetRootDir}\DATA\DB"; Flags: onlyifdoesntexist ignoreversion
Source: "{#ROOTSRCDATA}\DB\DNOEGDB_log.ldf"; DestDir: "{code:GetRootDir}\DATA\DB"; Flags: onlyifdoesntexist ignoreversion
Source: "{#ROOTSRCDATA}\DB\SQLAttachGDB.sql"; DestDir: "{code:GetRootDir}\DATA\DB"; Flags: ignoreversion
Source: "{#ROOTSRCDATA}\DB\SQLCMDGDB.bat"; DestDir: "{code:GetRootDir}\DATA\DB"; Flags: ignoreversion

Source: "{#ROOTADDDEMINSTALL}\AddDEM.ReliefProcessing\*"; DestDir: "{code:GetRootDir}\Sposterezhennya.ReliefProcessing"; Flags: ignoreversion recursesubdirs createallsubdirs
Source: "{#ROOTADDDEMINSTALL}\S1-ExternalCatalogTemplate\*"; DestDir: "{code:GetRootDir}\S1-ImageStorage"; Flags: ignoreversion recursesubdirs createallsubdirs
Source: "{#ROOTADDDEMINSTALL}\EngineSNAP\*"; DestDir: "{code:GetRootDir}\EngineSNAP"; Flags: ignoreversion recursesubdirs createallsubdirs

[Icons]
;Name: "{group}\{#AppName}"; Filename: "{app}\{#AppInstallTitle}\{#AppExeName}"; IconFilename: "{app}\{#AppInstallTitle}\{#AppExeName}"
;Name: "{commondesktop}\{#AppName}"; Filename: "{app}\{#AppInstallTitle}\{#AppExeName}"; IconFilename: "{app}\{#AppInstallTitle}\{#AppExeName}"; Tasks: desktopicon

[Run]
;Filename: "{code:GetRootDir}\Addins\gacutil\RegistryGAC.BAT"; Flags: nowait postinstall skipifsilent runascurrentuser; Description: "{cm:LaunchProgram,{#StringChange(AppName, '&', '&&')}}"

[Registry]
Root: "HKLM"; Subkey: "SOFTWARE\WOW6432Node\Milspace"; ValueType: string; ValueName: "Configuration"; ValueData: "{code:GetRootDir}\Addins"; Flags: uninsdeletekeyifempty uninsdeletevalue

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

procedure DeinitializeSetup();
var
  fn, fnSQL, fnC, fnR, fnAddDemCfg: String;
  ResultCode: Integer;
  b, bSQL, bCMD: boolean; 
  ss: TArrayOfString;
  i, ic: integer;
  sl: string; 
begin
  Log('DeinitializeSetup called');

  if not bSetup 
  then begin
    Log('Exit witout changes in files');
    exit;
  end;
 
  RootFolder:= GetRootDir('');

  fnC:= RootFolder + '\AddIns\MilSpace.Configurations.config';
  fnR:= RootFolder + '\AddIns\gacutil\RegistryGAC.BAT';

  fnAddDemCfg:=  RootFolder + '\Sposterezhennya.ReliefProcessing\Sposterezhennya.AddDem.ReliefProcessing.exe.Config';

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
    MsgBox('ERROR. File CONF not found: '#13 + fnC, mbInformation, MB_OK);
  end;

  if (FileExists(fnR))
  then begin
    Log('OK. File REGCMD found: ' + fnC); 
    b:= LoadStringsFromFile(fnR, ss);
    if b
    then begin
      Log('OK. File REGCMD read to list'); 
      for i:= 0 to GetArrayLength(ss) - 1 do
      begin
        ic := StringChange(ss[i], '[PATH]', RootFolder+'\AddIns\gacutil');
      end;
      if SaveStringsToFile(fnR, ss, false)
      then begin
        bCMD:= ExecAsOriginalUser(fnR, '', '', SW_SHOW, ewWaitUntilTerminated, ResultCode);
        if bCMD
        then begin
          Log('OK. Bat file REGCMD Execution');
        end
        else begin
          MsgBox('ERROR. REGCMD Executed NOT success', mbInformation, MB_OK);
        end;
      end
      else begin
        MsgBox('ERROR. Lines from file REGCMD NOT change success', mbInformation, MB_OK);
      end;
    end
    else begin
      MsgBox('ERROR. NOK. File REGCMD read to list', mbInformation, MB_OK);
    end
  end
  else begin
    Log('ERROR. File REGCMD not found: ' + fnC); 
    MsgBox('ERROR. File REGCMD not found: '#13 + fnC, mbInformation, MB_OK);
  end;

  if (FileExists(fnAddDemCfg))
  then begin
    Log('OK. File AddDem CONFIG found: ' + fnAddDemCfg); 
    b:= LoadStringsFromFile(fnAddDemCfg, ss);
    if b
    then begin
      Log('OK. File AddDem CONFIG read to list'); 
      for i:= 0 to GetArrayLength(ss) - 1 do
      begin
        ic := StringChange(ss[i], '[PATH]', RootFolder);
      end;
      if SaveStringsToFile(fnAddDemCfg, ss, false)
      then begin
        Log('OK. Lines from AddDem CONFIG file change and safe success');
      end
      else begin
        MsgBox('ERROR. Lines from AddDem CONFIG file NOT change success', mbInformation, MB_OK);
      end;
    end
    else begin
      MsgBox('ERROR. File AddDem CONFIG read to list', mbInformation, MB_OK);
    end
  end
  else begin
    Log('ERROR. File CONF not found: ' + fnC); 
    MsgBox('ERROR. Configuarion file Sposterezhennya.AddDem.ReliefProcessing.exe.Config was not found: '#13 + fnC, mbInformation, MB_OK);
  end;

  fn:= RootFolder + '\DATA\DB\SQLCMDGDB.bat';
  fnSQL:= RootFolder + '\DATA\DB\SQLAttachGDB.sql';

  if (FileExists(fn) and FileExists(fnSQL))
  then begin
    Log('OK. Files found: ' + fn + ' ' + fnSQL); 
    b:= LoadStringsFromFile(fn, ss);
    if b
    then begin
      Log('OK. File CMDGDB read to list'); 
      for i:= 0 to GetArrayLength(ss) - 1 do
      begin
        ic := StringChange(ss[i], '[PATH]', RootFolder+'\DATA\DB\');
      end;
      if SaveStringsToFile(fn, ss, false)
      then begin
        Log('OK. Lines from CMDGDB file change and safe success');
        bSQL:= LoadStringsFromFile(fnSQL, ss);
        if bSQL
        then begin
          Log('OK. File SQLGDB read to list');
          for i:= 0 to GetArrayLength(ss) - 1 do
          begin
            ic:= StringChange(ss[i], '[PATH]', RootFolder+'\DATA\DB\');
          end;
          if SaveStringsToFile(fnSQL, ss, false) 
          then begin
            Log('OK. Write changes SQLGDB success');
            bCMD:= ExecAsOriginalUser(fn, '', '', SW_SHOW, ewWaitUntilTerminated, ResultCode);
            if bCMD
            then begin
              Log('OK. Bat file Execution');
            end
            else begin
              MsgBox('ERROR. CMD Executed NOT success', mbInformation, MB_OK);
            end;
          end
          else begin
            MsgBox('ERROR. Lines from SQL file change and safe NOT success', mbInformation, MB_OK);
          end;
         end
        else begin
          MsgBox('ERROR. File SQL NOT read to list', mbInformation, MB_OK);
        end;
      end
      else begin
        MsgBox('ERROR. Lines from file NOT change success', mbInformation, MB_OK);
      end;
    end
    else begin
      MsgBox('ERROR. NOK. File read to list', mbInformation, MB_OK);
    end
  end
  else begin
    MsgBox('ERROR. File CMDGDB not found: '#13 + fn, mbInformation, MB_OK);
  end;
  
end;

