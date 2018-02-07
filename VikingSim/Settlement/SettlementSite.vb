Public Class SettlementSite
    Public Terrain As String
    Public LocationList As New List(Of String)
    Public Shared Function Construct(Optional ByVal locations As List(Of String) = Nothing) As SettlementSite
        Dim site As New SettlementSite
        With site
            If locations Is Nothing Then
                Dim locationDict As Dictionary(Of String, List(Of String)) = IO.ImportSquareBracketList(IO.sbTerrain)
                Dim locationKeys As New List(Of String)
                For Each k In locationDict.Keys
                    locationKeys.Add(k)
                Next

                'roll 2-5 locations
                Dim numLocations As Integer = rng.Next(2, 6)
                For n = 1 To numLocations
                    Dim location As String = GetRandom(Of String)(locationKeys)
                    .LocationList.Add(location)
                Next
            Else
                .LocationList.AddRange(locations)
            End If
        End With
        Return site
    End Function
End Class
