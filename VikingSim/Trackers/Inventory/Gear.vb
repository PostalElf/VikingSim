Public Class Gear
    Inherits Item

    Public Shared Function Import(ByVal targetName As String) As Gear
        Dim rawData As List(Of String) = IO.ImportSquareBracketSelect(IO.sbGear, targetName)
        Dim gear As New Gear
        With gear
            .Name = targetName

            For Each line In rawData
                Dim split As String() = line.Split(":")
                Dim header As String = split(0).Trim
                Dim entry As String = split(1).Trim

                Select Case header

                    Case Else : .BaseImport(header, entry)
                End Select
            Next
        End With
        Return gear
    End Function
End Class
