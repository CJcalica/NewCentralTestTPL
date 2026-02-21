;param1 := (A_Args.MaxIndex() >= 1) ? A_Args[1] : ""
;param2 := (A_Args.MaxIndex() >= 2) ? A_Args[2] : ""
;param3 := (A_Args.MaxIndex() >= 3) ? A_Args[3] : ""
;param4 := (A_Args.MaxIndex() >= 4) ? A_Args[4] : ""
WinWait, Program Selection, , 10         ; Waits up to 10 seconds for the window
if ErrorLevel
{
    MsgBox, 16, Error, Window not found.
}
else
{
    WinActivate, Program Selection
    WinWaitActive, Program Selection
    Sleep, 1000
    ControlFocus, Button3, Program Selection
    Sleep, 500
    Send, {Space}
    Sleep, 500
    Send, {Space}
    Sleep, 500
    ControlFocus, Button4, Program Selection
    Sleep, 500
    Send, {Space}
    Sleep, 500
    WinActivate, Select program file to insert.
    WinWaitActive, Select program file to insert.
    Sleep, 500
    inputFile := "C:\TPL\AutoFillDetails.txt"
    FileRead, fileContent, %inputFile%
    fileContent := StrReplace(fileContent, "`r", "")
    StringSplit, param, fileContent,`n
    Sleep, 500
    Send, Testprogram
    Sleep, 500
    Send, {Enter}
    Sleep, 500
    Send, %param1%
    Sleep, 500
    Send, {Enter}
    Sleep, 500
    Send, Programs
    Sleep, 500
    Send, {Enter}
    Sleep, 500
    Send, %param2%
    Sleep, 500
    Send, {Enter}
    Sleep, 500
    WinActivate, Program Selection
    WinWaitActive, Program Selection
    Sleep, 500
    ControlFocus, Button1, Program Selection
    Sleep, 500
    Send, {Space}
    Sleep, 500
}