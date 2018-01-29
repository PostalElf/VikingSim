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
End Class
