VERSION 5.00
Begin {C62A69F0-16DC-11CE-9E98-00AA00574A4F} frmSendBulkEmails 
   Caption         =   "Send Bulk Emails"
   ClientHeight    =   12945
   ClientLeft      =   120
   ClientTop       =   465
   ClientWidth     =   14970
   OleObjectBlob   =   "frmSendBulkEmails.frx":0000
   StartUpPosition =   1  'CenterOwner
End
Attribute VB_Name = "frmSendBulkEmails"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = False
Attribute VB_PredeclaredId = True
Attribute VB_Exposed = False

Public DraftEmail As Outlook.MailItem
Private Sub CloseForm()
    tbEmailAddresses.Text = ""
    tbLog.Text = ""
    tbBatchSize.Text = "90"
    ckbShowOnly.Value = False
    ckbAutoClose.Value = False
    Set DraftEmail = Nothing
    frmSendBulkEmails.Hide
End Sub
Private Sub btnClose_Click()
    CloseForm
End Sub

Private Sub btnSendEmails_Click()
    tbLog.Text = ""
    
    Dim errorStyle As Integer
    errorStyle = vbOKOnly + vbCritical
    
    If DraftEmail Is Nothing Then
        MsgBox "For some reason a draft email was not selected.  No email was sent!", errorStyle
        frmSendBulkEmails.Hide
        Exit Sub
    End If
    
    If tbEmailAddresses.TextLength = 0 Then
        MsgBox "You must enter the recipient email address(es)!", errorStyle
        Exit Sub
    End If
    
    Dim maxBatchSize As Integer
    maxBatchSize = GetBatchSize()
    If maxBatchSize = -1 Then
        Exit Sub
    End If
    
    Dim emailAddresses
    emailAddresses = GetEmailAddresses()
    
    Dim totalEmailAddressCount As Integer
    totalEmailAddressCount = UBound(emailAddresses) + 1
    
    If (totalEmailAddressCount \ maxBatchSize) > 5 And ckbShowOnly.Value = True Then
        MsgBox "You should not check 'Show Emails Only / Manual Send' when there are too many emails to display!", errorStyle
        Exit Sub
    End If
    
    Dim batchIndex As Integer
    batchIndex = 1
    
    Dim batchSize As Integer
    batchSize = 0
    
    Dim badEmailAddressCount As Integer
    badEmailAddressCount = 0
    
    Dim bccList As String
    For Each emailAddress In emailAddresses
        If IsValidEmailAddress(emailAddress) = False Then
            badEmailAddressCount = badEmailAddressCount + 1
            
            ' log the bad email address!
            Log ("Invalid email address: " & emailAddress)
        Else
            If batchSize = 0 Then
                bccList = emailAddress
                batchSize = 1
            ElseIf batchSize < maxBatchSize Then
                bccList = bccList & "; " & emailAddress
                batchSize = batchSize + 1
            Else
                ' batch has reached max size, send it
                SendEmail bccList, batchIndex
                
                ' then start a new batch
                bccList = emailAddress
                batchSize = 1
                batchIndex = batchIndex + 1
            End If
        End If
    Next
    
    ' send the remaining batch, if any
    If batchSize <= maxBatchSize Then
        SendEmail bccList, batchIndex
    End If
    
    Log vbCrLf & "*** Summary ***"
    Log "---------------"
    Log "- Total BCC email addresses supplied: " & CStr(totalEmailAddressCount)
    Log "- Total invalid BCC email addresses : " & CStr(badEmailAddressCount)
    Log "- Total number of emails sent       : " & CStr(totalEmailAddressCount - badEmailAddressCount)
    
    If badEmailAddressCount = 0 And ckbAutoClose.Value = True Then
        CloseForm
    End If
End Sub
Private Sub SendEmail(bccList As String, batchIndex As Integer)
    Log ("Sending batch # " & CStr(batchIndex) & " ...")
    Dim email As MailItem
    Set email = CreateItem(olMailItem)
    With email
        .To = DraftEmail.To
        .BCC = bccList
        .Subject = DraftEmail.Subject
        .HTMLBody = DraftEmail.HTMLBody
    End With
    If ckbShowOnly.Value = True Then
        email.Display
    Else
        email.Send
    End If
End Sub
Private Sub Log(msg As String)
    tbLog.Text = tbLog.Text & msg & vbCrLf
End Sub
Private Function GetEmailAddresses()
    Dim emailAddresses() As String
    Dim items() As String
    items = Split(tbEmailAddresses.Text, ";")
    Dim i As Integer
    i = 0
    For Each address In items
        Dim emailAddress As String
        emailAddress = Trim(address)
        If Len(emailAddress) > 0 Then
            ReDim Preserve emailAddresses(i)
            emailAddresses(i) = emailAddress
            i = i + 1
        End If
    Next
    GetEmailAddresses = emailAddresses
End Function
Public Function IsValidEmailAddress(ByVal emailAddress As String) As Boolean
    On Error GoTo Catch
    
    Dim emailRegExp As Object
    Set emailRegExp = CreateObject("vbscript.regexp")
    emailRegExp.IgnoreCase = True
    emailRegExp.Global = True
    emailRegExp.Pattern = "^([a-zA-Z0-9_\-\.]+)@[a-z0-9-]+(\.[a-z0-9-]+)*(\.[a-z]{2,3})$"
    
    IsValidEmailAddress = emailRegExp.Test(emailAddress)
      
    Exit Function
    
Catch:
    IsValidEmailAddress = False
    MsgBox "IsValidEmailAddress function" & vbCrLf & vbCrLf _
        & "Error#:  " & Err.Number & vbCrLf & vbCrLf & Err.Description, vbOKOnly + vbCritical
End Function
Private Function GetBatchSize() As Integer
    On Error GoTo Catch
    Dim batchSize As Integer
    batchSize = CInt(tbBatchSize.Text)
    If batchSize > 90 Or batchSize <= 0 Then
        batchSize = -1
        Dim errorStyle As Integer
        errorStyle = vbOKOnly + vbCritical
        MsgBox "Batch size must be between 1 and 90!", errorStyle
    End If
    GetBatchSize = batchSize
    Exit Function
Catch:
    GetBatchSize = -1
    MsgBox "GetBatchSize function" & vbCrLf & vbCrLf _
        & "Error#:  " & Err.Number & vbCrLf & vbCrLf & Err.Description, errorStyle
End Function
