Public Class WorkplacePost
    Inherits Workplace

    Public Shared Function Import(ByVal targetName As String) As WorkplacePost
        Dim rawData As List(Of String) = IO.ImportSquareBracketSelect(IO.sbPosts, targetName)

        Dim workplace As New WorkplacePost
        With workplace
            ._Name = targetName
            .ApprenticeCapacity = 0

            For Each line In rawData
                Dim ln As String() = line.Split(":")
                Dim header As String = ln(0).Trim
                Dim entry As String = ln(1).Trim
                Select Case header
                    Case Else : .WorkplaceImport(header, entry)
                End Select
            Next
        End With
        Return workplace
    End Function
    Public Overrides Sub Tick()
        MyBase.Tick()

        'posts should not have to do anything
    End Sub
    Public Overrides Function GetTickWarnings() As System.Collections.Generic.List(Of Alert)
        'posts should have no alerts
        Return New List(Of Alert)
    End Function
End Class
