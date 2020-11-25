#define AppGroupName "Sposterezhennya"
#define AppName "Sposterezhennya. База даних"
#define AppVersion "1.5"

#define AppPublisher ""
#define AppURL ""

#define AppInstallTitle "DATA"
#define AppInstallTitleADD "DB"
#define AppExeName ""

#define ROOTSRCINSTALL "e:\Projects\2018\Install\Sposterezhennya\Source"
#define ROOTINSTALL "e:\Projects\2018\Install\Sposterezhennya\Setup"
#define ROOTWORKDIR "_Sposterezhennya"

#define ArcMapRoot "Desktop10.6"

#define nProductInstall "2"
#define basenameinstall "Setup"
#define basenameUninstall "uninstall_"

[Setup]
AppId={{0CF94D9C-A3CB-4FE3-84EB-047980023E22}
AppName={#AppName}
AppVersion={#AppVersion}
;AppVerName={#MyAppName} {#MyAppVersion}
AppPublisher={#AppPublisher}
AppPublisherURL={#AppURL}
AppSupportURL={#AppURL}
AppUpdatesURL={#AppURL}
DefaultDirName= {code:GetRootDir}
DisableDirPage=yes
DefaultGroupName={#AppGroupName}
DisableProgramGroupPage=yes
OutputDir={#ROOTINSTALL}
OutputBaseFilename={#basenameinstall}_{#nProductInstall}_{#AppInstallTitle}{#AppInstallTitleADD}
Compression=lzma
SetupLogging=True
DiskSpanning=True
DiskSliceSize= 2100000000
DisableWelcomePage=False
;UninstallDisplayName={#basenameUninstall}{#AppInstallTitle}{#AppInstallTitleADD}

[Languages]
Name: "ukrainian"; MessagesFile: "compiler:Languages\Ukrainian.isl"

[Tasks]
;Name: "desktopicon"; Description: "{cm:CreateDesktopIcon}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: unchecked
;Name: "quicklaunchicon"; Description: "{cm:CreateQuickLaunchIcon}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: unchecked; OnlyBelowVersion: 0,6.1

[Files]
Source: "{#ROOTSRCINSTALL}\DATA\DB\Thesaurus.mdf"; DestDir: "{code:GetRootDir}\DATA\DB"; Flags: onlyifdoesntexist ignoreversion; Permissions: everyone-full
Source: "{#ROOTSRCINSTALL}\DATA\DB\Thesaurus_log.ldf"; DestDir: "{code:GetRootDir}\DATA\DB"; Flags: ignoreversion
Source: "{#ROOTSRCINSTALL}\DATA\DB\SQLAttachDB.sql"; DestDir: "{code:GetRootDir}\DATA\DB"; Flags: ignoreversion
Source: "{#ROOTSRCINSTALL}\DATA\DB\SQLCMD.bat"; DestDir: "{code:GetRootDir}\DATA\DB"; Flags: ignoreversion

[Run]
;Filename: "{code:GetRootDir}\DATA\DB\SQLCMD.bat"; WorkingDir: "{code:GetRootDir}\DATA\DB"; Flags: nowait runascurrentuser postinstall

[Registry]
;Root: "HKLM"; Subkey: "SOFTWARE\WOW6432Node\{#ROOTWORKDIR}"; ValueType: string; ValueName: "DataPath"; ValueData: "{code:GetRootDir}\DATA"; Flags: uninsdeletekeyifempty uninsdeletevalue

[Dirs]
Name: "{code:GetRootDir}\DATA"; Permissions: everyone-full

[Code]
var
  DistFolder: string;  

function InitializeSetup(): Boolean;
var 
  ResultStr: string;  
  b: bool;
begin
  Log('InitializeSetup called');
  //MsgBox('The installation will now start.', mbInformation, MB_OK);
  b:= RegQueryStringValue(HKEY_LOCAL_MACHINE, 'SOFTWARE\WOW6432Node\_Sposterezhennya', 'CommonPath', ResultStr);
  if b
    then begin
      DistFolder:= ResultStr;
      MsgBox('Встановлення буду виконано у каталог'#13 + ResultStr, mbInformation, MB_OK);
    end
    else begin
      MsgBox('Встановлення потребує попередньго завдання каталогу розміщення.'#13'Path: '+ResultStr+#13'Виконайте поперше Setup_1_Sposterezhennya.Dec.exe', mbInformation, MB_OK)
    end;
  Result:= b;
end;

function GetRootDir(s: string): string;
var 
  ResultStr: string; 
  b: bool;
begin
  b:= RegQueryStringValue(HKEY_LOCAL_MACHINE, 'SOFTWARE\WOW6432Node\_Sposterezhennya', 'CommonPath', ResultStr); 
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
  //function FileExists(const Name: String): Boolean;
  //function LoadStringFromFile(const FileName: String; var S: AnsiString): Boolean;
  //function LoadStringsFromFile(const FileName: String; var S: TArrayOfString): Boolean;
  //function SaveStringToFile(const FileName: String; const S: AnsiString; const Append: Boolean): Boolean;
  //function SaveStringsToFile(const FileName: String; const S: TArrayOfString; const Append: Boolean): Boolean;
  //function StringChange(var S: String; const FromStr, ToStr: String): Integer;

  fn:= DistFolder + '\DATA\DB\SQLCMD.bat';
  fnW:= DistFolder + '\DATA\DB\SQLCMDEX.bat';
  fnSQL:= DistFolder + '\DATA\DB\SQLAttachDB.sql';
  fnSQLW:= DistFolder + '\DATA\DB\SQLAttachDBEX.sql';

  if (FileExists(fn) and FileExists(fnSQL))
  then begin
    MsgBox('OK. File found: '#13 + fn +#13 + fnSQL, mbInformation, MB_OK);
    b:= LoadStringsFromFile(fn, ss);
    if b
    then begin
      MsgBox('OK. File CMD read to list', mbInformation, MB_OK);
      for i:= 0 to GetArrayLength(ss) - 1 do
      begin
        ic := StringChange(ss[i], '[PATH]', DistFolder+'\DATA\DB\');
      end;
      if SaveStringsToFile(fn, ss, false)
      then begin
        MsgBox('OK. Lines from CMD file change and safe success', mbInformation, MB_OK);
        bSQL:= LoadStringsFromFile(fnSQL, ss);
        if bSQL
        then begin
          MsgBox('OK. File SQL read to list', mbInformation, MB_OK);
          for i:= 0 to GetArrayLength(ss) - 1 do
          begin
            ic:= StringChange(ss[i], '[PATH]', DistFolder+'\DATA\DB\');
          end;
          if SaveStringsToFile(fnSQL, ss, false) 
          then begin
            MsgBox('OK. Lines from SQL file change and safe success', mbInformation, MB_OK);
            bCMD:= ExecAsOriginalUser(fn, '', '', SW_SHOW, ewWaitUntilTerminated, ResultCode);
            //Exec(ExpandConstant('{win}\notepad.exe'), '', '', SW_SHOW, ewWaitUntilTerminated, ResultCode)
            if bCMD
            then begin
              MsgBox('OK. CMD Executed success', mbInformation, MB_OK);
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
    MsgBox('NOK. File(s) not found: '#13 + fn, mbInformation, MB_OK);
  end;

end;