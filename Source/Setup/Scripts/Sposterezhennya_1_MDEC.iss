#define AppGroupName "Sposterezhennya"
#define AppName "Sposterezhennya MDEC"
#define AppVersion "1.5"

#define AppPublisher ""
#define AppURL ""

#define AppInstallTitle "Sposterezhennya.Dec"
#define AppInstallTitleADD ""

#define AppExeName "Sposterezhennya.Dec.exe"
#define ROOTSRCINSTALL "e:\Projects\2018\Sposterezhennya\App"
#define ROOTSRCDATA "e:\Projects\2018\Install\Sposterezhennya\Source\DATA"
#define ROOTINSTALL "e:\Projects\2018\Install\Sposterezhennya\Setup"
#define ROOTWORKDIR "Sposterezhennya"
#define ArcMapRoot "Desktop10.6"

#define nProductInstall "1"
#define basenameinstall "Setup"
#define basenameUninstall "uninstall_"

[Setup]
AppId={{57F3A695-6CEC-4C92-820D-CFBD24076CC8}
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
;OutputDir=E:\Projects\2018\Install\Setup
OutputDir={#ROOTINSTALL}
OutputBaseFilename={#basenameinstall}_{#nProductInstall}_{#AppInstallTitle}
Compression=lzma
SetupLogging=True
DiskSpanning=True
DiskSliceSize=2000000000
DisableWelcomePage=False
UninstallDisplayName={#basenameUninstall}{#AppInstallTitle}
UsePreviousAppDir=False
AllowRootDirectory=True

[Languages]
Name: "ukrainian"; MessagesFile: "compiler:Languages\Ukrainian.isl"

[Tasks]
Name: "desktopicon"; Description: "{cm:CreateDesktopIcon}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: unchecked
Name: "quicklaunchicon"; Description: "{cm:CreateQuickLaunchIcon}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: unchecked; OnlyBelowVersion: 0,6.1

[Files]
Source: "{#ROOTSRCINSTALL}\{#AppInstallTitle}\*"; DestDir: "{app}\{#AppInstallTitle}"; Flags: ignoreversion recursesubdirs createallsubdirs
Source: "{#ROOTSRCINSTALL}\decryptor\*"; DestDir: "{app}\Decryptor"; Flags: ignoreversion recursesubdirs createallsubdirs
Source: "{#ROOTSRCINSTALL}\MilspaceHelpers\*"; DestDir: "{app}\MilspaceHelpers"; Flags: ignoreversion recursesubdirs createallsubdirs
Source: "{#ROOTSRCINSTALL}\Sposterezhennya.AddInsMain\SposterezhennyaAddInMain.esriAddIn"; DestDir: "{app}\Addins"; Flags: ignoreversion recursesubdirs createallsubdirs
Source: "{#ROOTSRCINSTALL}\Common\*"; DestDir: "{app}\Common"; Flags: ignoreversion recursesubdirs createallsubdirs

Source: "{#ROOTSRCDATA}\DB\Thesaurus.mdf"; DestDir: "{app}\DATA\DB"; Flags: onlyifdoesntexist ignoreversion
Source: "{#ROOTSRCDATA}\DB\Thesaurus_log.ldf"; DestDir: "{app}\DATA\DB"; Flags: onlyifdoesntexist ignoreversion
Source: "{#ROOTSRCDATA}\DB\SQLAttachDB.sql"; DestDir: "{app}\DATA\DB"; Flags: ignoreversion
Source: "{#ROOTSRCDATA}\DB\SQLCMD.bat"; DestDir: "{app}\DATA\DB"; Flags: ignoreversion

[Icons]
Name: "{group}\{#AppName}"; Filename: "{app}\{#AppInstallTitle}\{#AppExeName}"; IconFilename: "{app}\{#AppInstallTitle}\{#AppExeName}"
Name: "{commondesktop}\{#AppName}"; Filename: "{app}\{#AppInstallTitle}\{#AppExeName}"; IconFilename: "{app}\{#AppInstallTitle}\{#AppExeName}"; Tasks: desktopicon
;Name: "{userappdata}\Microsoft\Internet Explorer\Quick Launch\{#MyAppName}"; Filename: "{app}\Decryptor\{#AppExeName}"; Tasks: quicklaunchicon

[Run]
;Filename: "{app}\{#AppInstallTitle}\{#AppExeName}"; Flags: nowait postinstall skipifsilent runascurrentuser; Description: "{cm:LaunchProgram,{#StringChange(AppName, '&', '&&')}}"
;Filename: "{src}\Setup_2_DATADB.exe"; Flags: nowait postinstall skipifsilent runascurrentuser; Description: "{cm:StartInstallP,{#StringChange("Загальну базу даних", '&', '&&')}}"
;Filename: "{src}\Setup_3_DATAVector.exe"; Flags: nowait postinstall skipifsilent runascurrentuser; Description: "{cm:StartInstallP,{#StringChange("Векторні дані", '&', '&&')}}"
;Filename: "{src}\Setup_31_DATAGenshtab.exe"; Flags: nowait postinstall skipifsilent runascurrentuser; Description: "{cm:StartInstallP,{#StringChange("Растрові дані", '&', '&&')}}"
;Filename: "{src}\Setup_1_MDec.exe"; Flags: nowait postinstall skipifsilent runascurrentuser; Description: "{cm:StartInstallP,{#StringChange("ПЗ МДек", '&', '&&')}}"

[Registry]
Root: "HKLM"; Subkey: "SOFTWARE\WOW6432Node\{#ROOTWORKDIR}"; ValueType: string; ValueName: "RootPath"; ValueData: "{app}"; Flags: uninsdeletekeyifempty uninsdeletevalue
Root: "HKLM"; Subkey: "SOFTWARE\WOW6432Node\{#ROOTWORKDIR}"; ValueType: string; ValueName: "DecryptorAppPath"; ValueData: "{app}\{#AppInstallTitle}"; Flags: uninsdeletekeyifempty uninsdeletevalue
Root: "HKLM"; Subkey: "SOFTWARE\WOW6432Node\{#ROOTWORKDIR}"; ValueType: string; ValueName: "DecryptorPath"; ValueData: "{app}\Decryptor\"; Flags: uninsdeletekeyifempty uninsdeletevalue
Root: "HKLM"; Subkey: "SOFTWARE\WOW6432Node\{#ROOTWORKDIR}"; ValueType: string; ValueName: "CommonPath"; ValueData: "{app}\Common\"; Flags: uninsdeletekeyifempty uninsdeletevalue
Root: "HKLM"; Subkey: "SOFTWARE\WOW6432Node\{#ROOTWORKDIR}"; ValueType: string; ValueName: "DataPath"; ValueData: "{app}\DATA\"; Flags: uninsdeletekeyifempty uninsdeletevalue

[Dirs]
Name: "{app}"; Permissions: everyone-full
Name: "{app}\DATA"; Permissions: everyone-full
Name: "{app}\DATA\DB"; Permissions: everyone-full

[Code]
var
  RootFolder: string;  
  bSetup: boolean;

function _CheckPassword(Password: String): Boolean;
begin
  Result := True; 
  //if LowerCase(GetMD5OfString(Password)) = LowerCase('b6a1cfc4884d311480c3b6cd0753c1af') then Result := True;   
end;

function InitializeSetup(): Boolean;
var 
  ResultStr: string;  
  b: bool;
begin
  Log('InitializeSetup called');

//  bSetup:= false;
//  MsgBox('The installation will now start.', mbInformation, MB_OK);
//  b:= RegQueryStringValue(HKEY_LOCAL_MACHINE, 'SOFTWARE\WOW6432Node\_Sposterezhennya', 'CommonPath', ResultStr);
//  if b
//    then begin
//      RootFolder:= ResultStr;
//      MsgBox('Встановлення буду виконано у каталог'#13 + ResultStr, mbInformation, MB_OK);
//    end
//    else begin
//      MsgBox('Встановлення потребує попередньго завдання каталогу розміщення.'#13'Path: '+ResultStr+#13'Виконайте поперше Setup_1_Sposterezhennya.Dec.exe', mbInformation, MB_OK)
//    end;
//  bSetup:= b;
//  Result:= b;

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
      //MsgBox('OK. Path from Registry: '+ #13 + ResultStr, mbInformation, MB_OK);
      //Result:= 'e:\_Sposterezhennya';
      Result:= ResultStr;       
    end
    else begin
      //MsgBox('NOK. Path from Registry: '+#13+ResultStr+#13+'Stop install', mbInformation, MB_OK);
      Result:= '';;       
    end;  
end;

procedure DeinitializeSetup();
var
  fn, fnW, fnSQL, fnSQLW: String;
  ResultCode: Integer;
  b, bSQL, bCMD: boolean; 
  ss: TArrayOfString;
  i, ic: integer;
  sl: string; 
begin
  Log('DeinitializeSetup called');

  if not bSetup 
  then begin
    Log('Exit without changes in files');
    exit;
  end;
 
  //RootFolder:= GetRootDir('');
  RootFolder:= GetRootDir('{app}');

  fn:= RootFolder + '\DATA\DB\SQLCMD.bat';
  fnSQL:= RootFolder + '\DATA\DB\SQLAttachDB.sql';

  if (FileExists(fn) and FileExists(fnSQL))
  then begin
    Log('OK. Files found: ' + fn + ' ' + fnSQL); 
    //MsgBox('OK. File found: '#13 + fn +#13 + fnSQL, mbInformation, MB_OK);
    b:= LoadStringsFromFile(fn, ss);
    if b
    then begin
      Log('OK. File CMD read to list'); 
      //MsgBox('OK. File CMD read to list', mbInformation, MB_OK);
      for i:= 0 to GetArrayLength(ss) - 1 do
      begin
        ic := StringChange(ss[i], '[PATH]', RootFolder+'\DATA\DB\');
      end;
      if SaveStringsToFile(fn, ss, false)
      then begin
        Log('OK. Lines from CMD file change and safe success');
        //MsgBox('OK. Lines from CMD file change and safe success', mbInformation, MB_OK);
        bSQL:= LoadStringsFromFile(fnSQL, ss);
        if bSQL
        then begin
          Log('OK. File SQL read to list');
          //MsgBox('OK. File SQL read to list', mbInformation, MB_OK);
          for i:= 0 to GetArrayLength(ss) - 1 do
          begin
            ic:= StringChange(ss[i], '[PATH]', RootFolder+'\DATA\DB\');
          end;
          if SaveStringsToFile(fnSQL, ss, false) 
          then begin
            Log('OK. Write changes success');
            //MsgBox('OK. Lines from SQL file change and safe success', mbInformation, MB_OK);
            bCMD:= ExecAsOriginalUser(fn, '', '', SW_SHOW, ewWaitUntilTerminated, ResultCode);
            if bCMD
            then begin
              Log('OK. Bat file Execution');
              //MsgBox('OK. CMD Executed success', mbInformation, MB_OK);
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
    MsgBox('ERROR. File(s) not found: '#13 + fn, mbInformation, MB_OK);
  end;

end;










