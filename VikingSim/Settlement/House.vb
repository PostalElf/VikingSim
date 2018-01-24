Public Class House
    Inherits Building

#Region "Residents"
    Private Residents As New List(Of Person)
    Public Function GetResident(ByVal name As String) As Person
        For Each r In Residents
            If r.Name = name Then Return r
        Next
        Return Nothing
    End Function
    Public Function GetResidents(ByVal name As String, Optional ByVal flags As String = "") As List(Of Person)
        Dim total As New List(Of Person)
        For Each r In Residents
            If name = "" OrElse r.Name = name Then
                Dim yesToQuit As Boolean = False
                Dim fs As String() = flags.Split(" ")
                For Each f In fs
                    Select Case f.ToLower
                        Case "single" : If r.IsMarried <> False Then yesToQuit = True
                        Case "married" : If r.IsMarried <> True Then yesToQuit = True
                        Case "male" : If r.Sex <> "Male" Then yesToQuit = True
                        Case "female" : If r.Sex <> "Female" Then yesToQuit = True
                        Case "employed" : If r.Occupation = Skill.Vagrant Then yesToQuit = True
                        Case "unemployed" : If r.Occupation <> Skill.Vagrant Then yesToQuit = True
                    End Select
                    If yesToQuit = True Then Exit For
                Next
                If yesToQuit = True Then Continue For
                total.Add(r)
            End If
        Next
        Return total
    End Function
    Public Sub AddResident(ByVal p As Person)
        If Residents.Contains(p) = True Then Exit Sub

        Residents.Add(p)
    End Sub
    Public Sub RemoveResident(ByVal p As Person)
        If Residents.Contains(p) = False Then Exit Sub

        Residents.Remove(p)
    End Sub
#End Region

    Private Name As String

    Public Overrides Function ToString() As String
        Return Name & " - " & Residents.Count
    End Function
End Class
