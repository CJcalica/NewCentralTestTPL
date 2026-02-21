Run, "C:\Documents and Settings\All Users\Desktop\VisATE Launcher.exe"

WinWait, VisualATE Launcher, , 10         ; Waits up to 10 seconds for the window

if ErrorLevel
{
    MsgBox, 16, Error, Window not found.
}
else
{
    WinActivate, VisualATE Launcher
    WinWaitActive, VisualATE Launcher
    Sleep, 1000
    ControlClick, Button7, VisualATE Launcher
    Sleep, 1000
    Send, {Left}
    Sleep, 500
    Send, {Enter}
    Sleep, 500
    Run, "C:\ITfolder\TPL\Login.exe"
}


