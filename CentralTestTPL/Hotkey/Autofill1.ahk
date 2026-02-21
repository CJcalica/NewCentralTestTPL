inputFile := "C:\ITfolder\TPL\AutoFillDetails.txt"
FileRead, fileContent, %inputFile%
fileContent := StrReplace(fileContent, "`r", "")
params := StrSplit(fileContent, "`n")
param1 := params[1]

title := "ASL - [" . param1 . " (Default) 7.1.2]"

SetTitleMatchMode, 2  ; partial match

WinWait, %title%, , 10
if ErrorLevel
{
    MsgBox, 16, Error, Window not found:`n%title%
    return
}

WinActivate, %title%
WinWaitActive, %title%

Sleep, 500
Send, !w
Sleep, 500
Send, {Down 3}
Sleep, 500
Send, {Enter}
Sleep, 1000
Send, !l
Sleep, 500
Send, {Enter}
Sleep, 1000
inputFile := "C:\ITfolder\TPL\AutoFillDetails.txt"
FileRead, fileContent, %inputFile%
fileContent := StrReplace(fileContent, "`r", "")
StringSplit, param, fileContent,`n
Send, %param4%
Sleep, 500
Send, {Enter}
