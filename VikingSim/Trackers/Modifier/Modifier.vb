Public Class Modifier
    Private Title As String
    Private Timer As Integer
    Private Quality As String
    Private Value As Integer
    Private Owner As iModifiable

    Public Sub Tick()
        If Timer = -1 Then Exit Sub

        Timer -= 1
        If Timer = 0 Then
            Owner.RemoveModifier(Me)
        End If
    End Sub
End Class
