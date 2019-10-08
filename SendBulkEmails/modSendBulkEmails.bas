Attribute VB_Name = "modSendBulkEmails"
Option Explicit
Function GetCurrentItem() As Object
    Dim objApp As Outlook.Application
           
    Set objApp = Application
    On Error Resume Next
    Select Case TypeName(objApp.ActiveWindow)
        Case "Explorer"
            Set GetCurrentItem = objApp.ActiveExplorer.Selection.Item(1)
        Case "Inspector"
            Set GetCurrentItem = objApp.ActiveInspector.CurrentItem
    End Select
       
    Set objApp = Nothing
End Function
Function GetCurrentFolder() As Object
    Dim objApp As Outlook.Application
           
    Set objApp = Application
    On Error Resume Next
    Select Case TypeName(objApp.ActiveWindow)
        Case "Explorer"
            Set GetCurrentFolder = objApp.ActiveExplorer.currentFolder
        Case "Inspector"
            Set GetCurrentFolder = Nothing
    End Select
       
    Set objApp = Nothing
End Function
Public Sub SendBulkEmails()
    Dim oSelectedItem As Outlook.MailItem
    Dim style, response
    Dim currentFolder As Outlook.Folder
    style = vbOKOnly + vbCritical
    Set currentFolder = GetCurrentFolder()
    Set oSelectedItem = GetCurrentItem()
    If StrComp(currentFolder.Name, "Drafts", vbBinaryCompare) <> 0 Or oSelectedItem.Class <> olMail Then
        response = MsgBox("Please select the draft of the bulk email in the Drafts folder!", style)
        Exit Sub
    End If
    
    Dim validationMsg
    validationMsg = ValidateDraftEmail(oSelectedItem)
    If validationMsg <> "" Then
        response = MsgBox(validationMsg, style)
        Exit Sub
    End If
    
    Set frmSendBulkEmails.DraftEmail = oSelectedItem
    frmSendBulkEmails.Show vbModeless
End Sub
Private Function ValidateDraftEmail(draft As Outlook.MailItem)

    If StrComp(draft.To, "", vbBinaryCompare) = 0 Then
        ValidateDraftEmail = "To email address must be specified in the draft email!"
        Exit Function
    End If
    
    If frmSendBulkEmails.IsValidEmailAddress(draft.To) = False Then
        ValidateDraftEmail = "To email address is invalid!"
        Exit Function
    End If
    
    If StrComp(draft.Subject, "", vbBinaryCompare) = 0 Then
        ValidateDraftEmail = "Draft email must have a subject!"
        Exit Function
    End If
    
    If StrComp(draft.HTMLBody, "", vbBinaryCompare) = 0 Then
        ValidateDraftEmail = "Draft email must have some text in the body!"
        Exit Function
    End If
    
    ValidateDraftEmail = ""
    
End Function
