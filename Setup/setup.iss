#define MyAppName "CAST Report Generator"
#define MyAppShortName "ReportGenerator"
#define MyAppVersion "_THE_VERSION_"
#define MyAppPublisher "CAST"
#define MyAppURL "http://www.castsoftware.com/"
#define MyAppExeName "CastReporting.UI.WPF.Core.exe"
#define MyAppExe "../CastReporting.UI.WPF.V2/bin/Release/netcoreapp3.0/"+MyAppExeName
#define MyAppCopyright GetFileCopyright(MyAppExe)
#define App1190Id "{{545F2836-FB30-4ECD-9E03-C7AF5AF21F50}"
#define App1191Id "{{B71BC2F3-1079-4106-827A-03A3D12D8462}"
#define App1192Id "{{6EB11AF1-AA4D-4B2F-91FF-8E3498B4CF8C}"

[Setup]
; NOTE: The value of AppId uniquely identifies this application.
; Do not use the same AppId value in installers for other applications.
; (To generate a new GUID, click Tools | Generate GUID inside the IDE.)
AppId={#App1192Id}
AppName={#MyAppName}
AppVersion={#MyAppVersion}
AppVerName={#MyAppName} {#MyAppVersion}
AppCopyright={#MyAppCopyright}
AppPublisher={#MyAppPublisher}
AppPublisherURL={#MyAppURL}
AppSupportURL={#MyAppURL}
AppUpdatesURL={#MyAppURL}
VersionInfoCompany={#MyAppPublisher}
VersionInfoVersion={#MyAppVersion}
DefaultDirName={commonpf32}\CAST\{#MyAppShortName} {#MyAppVersion}
DefaultGroupName={#MyAppName} {#MyAppVersion}
OutputBaseFilename=ReportGeneratorSetup
OutputDir=../Setup
SetupIconFile=../CastReporting.UI.WPF.V2/Resources/Images/cast.ico
Compression=lzma
SolidCompression=yes
AlwaysShowDirOnReadyPage=true
;DirExistsWarning=No
UninstallDisplayIcon={app}\{#MyAppExeName}
InfoBeforeFile=readme.txt


[Languages]
Name: "english"; MessagesFile: "compiler:Default.isl" ; LicenseFile: "../Setup/License.rtf"

[Tasks]
Name: "desktopicon"; Description: "{cm:CreateDesktopIcon}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: unchecked

[Files]
; NOTE:packages/CommonServiceLocator.1.0/lib/NET35
Source: "../packages/CommonServiceLocator.1.0/lib/NET35/Microsoft.Practices.ServiceLocation.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "../packages/CommonServiceLocator.1.0/lib/NET35/Microsoft.Practices.ServiceLocation.XML"; DestDir: "{app}"; Flags: ignoreversion
; NOTE:packages/CommonServiceLocator.1.0/lib/SL30
Source: "../packages/CommonServiceLocator.1.0/lib/SL30/Microsoft.Practices.ServiceLocation.dll"; DestDir: "{app}"; Flags: ignoreversion
Source: "../packages/CommonServiceLocator.1.0/lib/SL30/Microsoft.Practices.ServiceLocation.XML"; DestDir: "{app}"; Flags: ignoreversion
; NOTE:packages/WpfAnimatedGif-1.4.4
Source: "../packages/WpfAnimatedGif-1.4.4/WpfAnimatedGif.dll"; DestDir: "{app}"; Flags: ignoreversion
; NOTE:packages/
Source: "../packages/repositories.config"; DestDir: "{app}"; Flags: ignoreversion
; NOTE:CastReporting.UI.WPF.V2/Images
Source: "../CastReporting.UI.WPF.V2/Resources/Images/*"; DestDir: "{app}\Resources\Images"; Flags: ignoreversion
; NOTE: Value from CastReporting.UI.WPF\bin\Release
source: "../CastReporting.Console.Core/bin/Release/netcoreapp3.1/*.exe";DestDir: "{app}"; Flags: ignoreversion
source: "../CastReporting.Console.Core/bin/Release/netcoreapp3.1/*.dll";DestDir: "{app}"; Flags: ignoreversion recursesubdirs
source: "../CastReporting.Console.Core/bin/Release/netcoreapp3.1/CastReporting.Console.Core.runtimeconfig.json";DestDir: "{app}"; Flags: ignoreversion
source: "../CastReporting.Console.Core/bin/Release/netcoreapp3.1/CastReporting.Console.Core.deps.json";DestDir: "{app}"; Flags: ignoreversion
source: "../CastReporting.UI.WPF.V2/bin/Release/netcoreapp3.1/*.dll";DestDir: "{app}"; Flags: ignoreversion recursesubdirs
source: "../CastReporting.UI.WPF.V2/bin/Release/netcoreapp3.1/*.exe";DestDir: "{app}"; Flags: ignoreversion recursesubdirs
source: "../CastReporting.UI.WPF.V2/bin/Release/netcoreapp3.1/*.config";DestDir: "{app}"; Flags: ignoreversion recursesubdirs
source: "../CastReporting.UI.WPF.V2/bin/Release/netcoreapp3.1/CastReporting.UI.WPF.Core.runtimeconfig.json";DestDir: "{app}"; Flags: ignoreversion
source: "../CastReporting.UI.WPF.V2/bin/Release/netcoreapp3.1/CastReporting.UI.WPF.Core.deps.json";DestDir: "{app}"; Flags: ignoreversion
source: "../CastReporting.Console.Core/bin/Release/netcoreapp3.1/Parameters/*.xml";DestDir: "{app}"; Flags: ignoreversion
Source: "../CastReporting.Reporting.Core/Templates/*"; DestDir: "{code:GetTempPath}\Templates"; Flags: ignoreversion recursesubdirs
source: "../CastReporting.Repositories.Core/CastReportingSetting.xml"; DestDir: "{code:GetSettingsPath}"; Flags: ignoreversion; AfterInstall:SaveSettings()
; NOTE:License
Source: "../Setup/License.rtf"; DestDir: "{app}"; Flags: ignoreversion
Source: "../Setup/install_dotnet_core.bat"; DestDir: "{app}"; Flags: ignoreversion

[Icons]
Name: "{group}\{#MyAppShortName} {#MyAppVersion}"; Filename: "{app}\{#MyAppExeName}"
Name: "{group}\{cm:UninstallProgram,{#MyAppShortName} {#MyAppVersion}}"; Filename: "{uninstallexe}"
Name: "{commondesktop}\{#MyAppShortName} {#MyAppVersion}"; Filename: "{app}\{#MyAppExeName}"; Tasks: desktopicon

[Dirs]
Name: "{code:GetSettingsPath}"; Permissions: users-full

[Run]
Filename: "{app}\install_dotnet_core.bat"; Description: "Install dotnet core sdk 3.1.102 (mandatory for running CAST-ReportGenerator)"; Flags: postinstall
Filename: "{app}\{#MyAppExeName}"; Description: "{cm:LaunchProgram,{#StringChange(MyAppShortName, '&', '&&')}}"; Flags: nowait postinstall skipifsilent

[Code]
// Variables Globales
var
  PageParam: TInputDirWizardPage;

// Creer les Pages Personnalis�es
procedure CreateTheWizardPages;
begin
  PageParam := CreateInputDirPage(wpSelectDir,
		'Select Templates Destination location',
		'Where should Template documents be located?',
		'Setup will store Template documents into a sub folder "Templates" of the following folder'#13#10#13#10 +
			'To continue, click Next. If you would like to select a different folder, click Browse.',
		False, 'New Folder');

	// Ajouter un �l�ment (avec une valeur vide)
	PageParam.Add('');
	
	// Initialiser les valeurs par d�faut (optional)
	PageParam.Values[0] := ExpandConstant('{commonappdata}')+'\CAST\ReportGenerator\' + '{#MyAppVersion}';
	
end;

// Functions added to be able to save the settings in UTF-8 without bom for chinese and japanese OS
const
  CP_UTF8 = 65001;

function WideCharToMultiByte(CodePage: UINT; dwFlags: DWORD;
  lpWideCharStr: string; cchWideChar: Integer; lpMultiByteStr: AnsiString;
  cchMultiByte: Integer; lpDefaultCharFake: Integer;
  lpUsedDefaultCharFake: Integer): Integer;
  external 'WideCharToMultiByte@kernel32.dll stdcall';

function GetStringAsUtf8(S: string): AnsiString;
var
  sizeres: Integer;
  ress: AnsiString;
begin
  sizeres := WideCharToMultiByte(CP_UTF8, 0, S, Length(S), ress, 0, 0, 0);
  SetLength(ress, sizeres);
  WideCharToMultiByte(CP_UTF8, 0, S, Length(S), ress, sizeres, 0, 0);
end;

function SaveStringToUTF8FileWithoutBOM(FileName: string; S: string): Boolean;
var
  Utf8: AnsiString;
begin
  Utf8 := GetStringAsUtf8(S);
  Result := SaveStringToFile(FileName, Utf8, True);
end;

//Replace substring in a string
procedure FileReplace(SrcFile, sFrom, sTo: String);
var
        FileContent: AnsiString;
        FileContentW: String;
begin
    //Load srcfile to a string
    LoadStringFromFile(SrcFile, FileContent);
    FileContentW := String(FileContent);
    //Replace Fromstring by toString in file string content
    StringChangeEx (FileContentW, sFrom, sTo, True);
    //Replace old content srcfile by the new content
    DeleteFile(SrcFile);
    SaveStringToFile(SrcFile,AnsiString(FileContentW),True);
    //SaveStringToUTF8FileWithoutBOM(SrcFile,AnsiString(FileContentW));
end;

// Fonctions de retour
function GetTempPath(Param: String): String;
begin
    Result := PageParam.Values[0];
end;

function GetSettingsPath(Param: String): String;
begin
    //Settings dir name as used in C# report generator program
    Result := ExpandConstant('{commonappdata}') + '\CAST\ReportGenerator\' + '{#MyAppVersion}';
end;

procedure SaveSettings();
var
  S1, S2 : String;
begin
    FileReplace(GetSettingsPath(S1) + '\CastReportingSetting.xml','<TemplatePath></TemplatePath>','<TemplatePath>' + GetTempPath(S2) + '\Templates</TemplatePath>'); 
    FileReplace(GetSettingsPath(S1) + '\CastReportingSetting.xml','<PortfolioFolderNamePath></PortfolioFolderNamePath>','<PortfolioFolderNamePath>' + GetTempPath(S2) + '\Templates\Portfolio</PortfolioFolderNamePath>'); 
    FileReplace(GetSettingsPath(S1) + '\CastReportingSetting.xml','<ApplicationFolderNamePath></ApplicationFolderNamePath>','<ApplicationFolderNamePath>' + GetTempPath(S2) + '\Templates\Application</ApplicationFolderNamePath>'); 
end;

// Update ReadyMemo
function UpdateReadyMemo(Space, NewLine, MemoUserInfoInfo, MemoDirInfo, MemoTypeInfo,
  MemoComponentsInfo, MemoGroupInfo, MemoTasksInfo: String): String;
var
  S, S1: String;  
begin
  S := '';
  S := S + MemoDirInfo + NewLine;
  S := S + NewLine + MemoGroupInfo + NewLine;
  S := S + NewLine + 'Templates Destination location :' + NewLine;
  S := S + Space + GetTempPath(S1) + NewLine;
  S := S + NewLine + MemoTasksInfo + NewLine;
  Result := S;
end;

function GetUninstallString(versionid: string): string;
var
  sUnInstPath: string;
  sUnInstallString: String;
begin
  Result := '';
  sUnInstPath := ExpandConstant('Software\Microsoft\Windows\CurrentVersion\Uninstall\' + versionid + '_is1'); //Your App GUID/ID
  sUnInstallString := '';
  if not RegQueryStringValue(HKLM, sUnInstPath, 'UninstallString', sUnInstallString) then
    RegQueryStringValue(HKCU, sUnInstPath, 'UninstallString', sUnInstallString);
  Result := sUnInstallString;
end;
 
function IsUpgrade(versionid: string): Boolean;
begin
  Result := (GetUninstallString(versionid) <> '');
end;

function UninstallOldVersion(versionid: string; version: string): Boolean;
var
  iResultCode: integer;
  sUnInstallString: string;
begin
  result := true;
  if not IsUpgrade(versionid) then begin
    exit;
  end;
  
  if not (MsgBox(ExpandConstant('CAST Report Generator ' + version + ' is detected. Do you want to uninstall it?'), mbConfirmation, MB_YESNO) = IDYES) then begin
      exit;
  end;
  sUnInstallString := GetUninstallString(versionid);
  sUnInstallString :=  RemoveQuotes(sUnInstallString);
  Exec(ExpandConstant(sUnInstallString), '/VERYSILENT', '', SW_SHOW, ewWaitUntilTerminated, iResultCode);
  result := true;
end;


{*** INITIALISATIONS ***}

function InitializeSetup(): Boolean;
begin
    result := false;
    result := UninstallOldVersion('{#App1190Id}', '1.19.0');
    result := UninstallOldVersion('{#App1191Id}', '1.19.1');
    result := UninstallOldVersion('{#App1192Id}', '{#MyAppVersion}');
end;

procedure InitializeWizard;
begin
  CreateTheWizardPages;
end;

