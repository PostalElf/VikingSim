Public Class Furniture
    Inherits Item
    Public Shared Function Import(ByVal targetName As String) As Furniture
        Dim rawData As List(Of String) = IO.ImportSquareBracketSelect(IO.sbFurniture, targetName)
        Dim furniture As New Furniture
        With furniture
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
        Return furniture
    End Function
End Class
