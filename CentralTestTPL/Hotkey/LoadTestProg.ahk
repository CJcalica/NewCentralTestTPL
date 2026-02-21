WinWait, Task List, , 10         ; Waits up to 10 seconds for the window

if ErrorLevel
{
    MsgBox, 16, Error, Window not found.
}
else
{
    WinActivate, Task List
    WinWaitActive, Task List
    Sleep, 1000
    ControlFocus, Button5, Task List
    Sleep, 500
    Send, {Space}
    Sleep, 500
    Run, "C:\ITfolder\TPL\LoadTestProg2.exe"
}

