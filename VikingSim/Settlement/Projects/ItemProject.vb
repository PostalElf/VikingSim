Public Class ItemProject
    Inherits Project
    Public Shared Function Import(ByVal targetName As String) As ItemProject
        Dim rawData As List(Of String) = IO.ImportSquareBracketSelect(IO.sbItems, targetName)

        Dim bp As New ItemProject
        With bp
            .Name = targetName

            For Each line In rawData
                Dim ln As String() = line.Split(":")
                Dim header As String = ln(0)
                Dim entry As String = ln(1)
                Select Case header
                    Case Else : .BaseImport(header, entry)
                End Select
            Next
        End With
        Return bp
    End Function
    Public Function Unpack() As Item
        Dim item As Item = item.import(Name)
        item.SetHistory(Creator, World.TimeNow)
        Return item
    End Function

End Class
