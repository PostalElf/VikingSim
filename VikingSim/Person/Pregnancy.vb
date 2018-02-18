Public Class Pregnancy
    Private Weeks As Integer
    Private Father As Person
    Private Mother As Person
    Private Const TermWeeks As Integer = 32

    Public Sub New(ByVal _father As Person, ByVal _mother As Person)
        Father = _father
        Mother = _mother
    End Sub
    Public Function Tick() As Person
        Weeks += 1
        If Weeks >= TermWeeks Then
            Return Person.Birth(Father, Mother)
        End If
        Return Nothing
    End Function
    Public Function GetTickWarnings() As List(Of Alert)
        Dim total As New List(Of Alert)
        Dim dif As Integer = TermWeeks - Weeks
        Select Case dif
            Case 2 : total.Add(New Alert(Me, 3, Mother.Name & "'s birth is due next week."))
            Case 5 : total.Add(New Alert(Me, 2, Mother.Name & "'s birth is due in 4 weeks."))
        End Select
        Return total
    End Function
End Class
