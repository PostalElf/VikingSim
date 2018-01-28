Public Class ItemProject
    Inherits Project

    Public Shared Function Import(ByVal targetName As String) As ItemProject
        Dim rawData As List(Of String) = IO.ImportSquareBracketSelect(IO.sbItems, targetName)
        If rawData Is Nothing Then Throw New Exception("Item name " & targetName & " not found.")

        Dim ip As New ItemProject
        With ip
            .Name = targetName

            For Each line In rawData
                Dim ln As String() = line.Split(":")
                Dim header As String = ln(0).Trim
                Dim entry As String = ln(1).Trim
                Select Case header
                    Case Else : .BaseImport(header, entry)
                End Select
            Next
        End With
        Return ip
    End Function
    Public Function Unpack() As String
        Return Name
    End Function
End Class
