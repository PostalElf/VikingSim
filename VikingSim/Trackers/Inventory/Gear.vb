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
                    Case "Efficiency"
                        Dim s As String() = entry.Split(",")
                        Dim occ As String = split(0).Trim
                        Dim value As Integer = Convert.ToInt32(split(1).Trim)
                        If .Bonuses.ContainsKey(occ) = False Then .Bonuses.Add(occ, 0)
                        .Bonuses(occ) += value

                    Case Else : .BaseImport(header, entry)
                End Select
            Next
        End With
        Return gear
    End Function

    Private Bonuses As New Dictionary(Of String, Integer)
    Public Overrides Function GetBonus(ByVal occupation As String) As Integer
        If Bonuses.ContainsKey(occupation) = False Then Return 0
        Return Bonuses(occupation)
    End Function
End Class
