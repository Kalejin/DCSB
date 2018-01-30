# define name of installer
	!system "ExtractVersionInfo.exe"
	!include "App-Version.txt"
	!include "MUI.nsh"
	!include "FileFunc.nsh"
	Name "Deathcounter and Soundboard, Version ${Version}"
	OutFile "..\Installers\DCSB_${Version}.exe"
 
# define installation directory
	InstallDir "$PROGRAMFILES\Deathcounter and Soundboard"
	InstallDirRegKey HKLM Software\DCSB InstallLocation
 
# for removing Start Menu shortcut in Windows 7
	RequestExecutionLevel admin

# define icon
	!define MUI_ICON "..\DCSB\icon.ico"

# install pages
	!insertmacro MUI_PAGE_WELCOME
	!insertmacro MUI_PAGE_DIRECTORY
	!insertmacro MUI_PAGE_INSTFILES
		# These indented statements modify settings for MUI_PAGE_FINISH
		!define MUI_FINISHPAGE_NOAUTOCLOSE
		!define MUI_FINISHPAGE_RUN "$INSTDIR\DCSB.exe"
		!define MUI_FINISHPAGE_RUN_CHECKED
		!define MUI_FINISHPAGE_RUN_TEXT "Start Deathcounter and Soundboard"
	!insertmacro MUI_PAGE_FINISH

# uninstall pages
	!insertmacro MUI_UNPAGE_WELCOME
	!insertmacro MUI_UNPAGE_CONFIRM
	!insertmacro MUI_UNPAGE_INSTFILES
	!insertmacro MUI_UNPAGE_FINISH
 
# language
	!insertmacro MUI_LANGUAGE "English"

# start default section
Section
    # creating start menu shortcuts for all users
    SetShellVarContext all

    # set the installation directory as the destination for the following actions
    SetOutPath $INSTDIR
 
    # create the uninstaller
    WriteUninstaller "uninstall.exe"

    # install app files
    File "..\DCSB\bin\Release\GalaSoft.MvvmLight.dll"
    File "..\DCSB\bin\Release\GalaSoft.MvvmLight.Extras.dll"
    File "..\DCSB\bin\Release\GalaSoft.MvvmLight.Platform.dll"
    File "..\DCSB\bin\Release\DCSB.exe"
    File "..\DCSB\bin\Release\DCSB.Business.dll"
    File "..\DCSB\bin\Release\DCSB.Controls.dll"
    File "..\DCSB\bin\Release\DCSB.Converters.dll"
    File "..\DCSB\bin\Release\DCSB.Icons.dll"
    File "..\DCSB\bin\Release\DCSB.Input.dll"
    File "..\DCSB\bin\Release\DCSB.Interactivity.dll"
    File "..\DCSB\bin\Release\DCSB.Models.dll"
    File "..\DCSB\bin\Release\DCSB.SoundPlayer.dll"
    File "..\DCSB\bin\Release\DCSB.Utils.dll"
    File "..\DCSB\bin\Release\DCSB.ViewModels.dll"
    File "..\DCSB\bin\Release\DCSB.Views.dll"
    File "..\DCSB\bin\Release\NAudio.dll"
    File "..\DCSB\bin\Release\NAudio.Vorbis.dll"
    File "..\DCSB\bin\Release\NVorbis.dll"
    File "..\DCSB\bin\Release\System.Windows.Interactivity.dll"
 
    # create a shortcut named "new shortcut" in the start menu programs directory
    # point the new shortcut at the program uninstaller
    CreateDirectory "$SMPROGRAMS\Deathcounter and Soundboard"
    CreateShortCut "$SMPROGRAMS\Deathcounter and Soundboard\uninstall.lnk" "$INSTDIR\uninstall.exe"
    CreateShortCut "$SMPROGRAMS\Deathcounter and Soundboard\DCSB.lnk" "$INSTDIR\DCSB.exe"

    # add application to Add/Remove programs
    WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\DCSB" \
                 "DisplayName" "Deathcounter and Soundboard"
    WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\DCSB" \
                 "UninstallString" "$\"$INSTDIR\uninstall.exe$\""
    WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\DCSB" \
                 "QuietUninstallString" "$\"$INSTDIR\uninstall.exe$\" /S"
	WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\DCSB" \
                 "Publisher" "Kalejin"
	WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\DCSB" \
                 "DisplayVersion" "${Version}"
	WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\DCSB" \
                 "DisplayIcon" "$INSTDIR\DCSB.exe"
    ${GetSize} "$INSTDIR" "/S=0K" $0 $1 $2
    IntFmt $0 "0x%08X" $0
	WriteRegDWORD HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\DCSB" \
                 "EstimatedSize" "$0"
    WriteRegStr HKLM "Software\DCSB" \
                 "InstallLocation" "$\"$INSTDIR$\""
SectionEnd
 
# uninstaller section start
Section "uninstall"
 
    # creating start menu shortcuts for all users
    SetShellVarContext all

    # delete the uninstaller and app files
    Delete "$INSTDIR\uninstall.exe"
	Delete "$INSTDIR\GalaSoft.MvvmLight.dll"
    Delete "$INSTDIR\GalaSoft.MvvmLight.Extras.dll"
    Delete "$INSTDIR\GalaSoft.MvvmLight.Platform.dll"
    Delete "$INSTDIR\DCSB.exe"
    Delete "$INSTDIR\DCSB.Business.dll"
    Delete "$INSTDIR\DCSB.Controls.dll"
    Delete "$INSTDIR\DCSB.Converters.dll"
    Delete "$INSTDIR\DCSB.Icons.dll"
    Delete "$INSTDIR\DCSB.Input.dll"
    Delete "$INSTDIR\DCSB.Interactivity.dll"
    Delete "$INSTDIR\DCSB.Models.dll"
    Delete "$INSTDIR\DCSB.SoundPlayer.dll"
    Delete "$INSTDIR\DCSB.Utils.dll"
    Delete "$INSTDIR\DCSB.ViewModels.dll"
    Delete "$INSTDIR\DCSB.Views.dll"
    Delete "$INSTDIR\NAudio.dll"
    Delete "$INSTDIR\NAudio.Vorbis.dll"
    Delete "$INSTDIR\NVorbis.dll"
    Delete "$INSTDIR\System.Windows.Interactivity.dll"
    StrCpy $0 "$INSTDIR"
    Call un.DeleteDirIfEmpty
 
    # remove the link from the start menu
    Delete "$SMPROGRAMS\Deathcounter and Soundboard\uninstall.lnk"
    Delete "$SMPROGRAMS\Deathcounter and Soundboard\DCSB.lnk"
    StrCpy $0 "$SMPROGRAMS\Deathcounter and Soundboard"
    Call un.DeleteDirIfEmpty

    # remove registry key
    DeleteRegKey HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\DCSB"
    DeleteRegKey HKLM "Software\DCSB"
 
# uninstaller section end
SectionEnd

Function un.DeleteDirIfEmpty
  FindFirst $R0 $R1 "$0\*.*"
  strcmp $R1 "." 0 NoDelete
   FindNext $R0 $R1
   strcmp $R1 ".." 0 NoDelete
    ClearErrors
    FindNext $R0 $R1
    IfErrors 0 NoDelete
     FindClose $R0
     Sleep 1000
     RMDir "$0"
  NoDelete:
   FindClose $R0
FunctionEnd