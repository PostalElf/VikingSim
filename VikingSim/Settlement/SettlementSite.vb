Public Class SettlementSite
    Implements iMapLocation
    Public Terrain As String
    Public LocationList As New List(Of String)
    Public Shared Function Construct(Optional ByVal terrain As String = Nothing) As SettlementSite
        'allLocation holds a list of all the different types of locations
        Dim terrainDict As Dictionary(Of String, List(Of String)) = IO.ImportSquareBracketList(IO.sbTerrain)
        Dim allLocation As New List(Of String)
        For Each k In terrainDict.Keys
            For Each l In terrainDict(k)
                If allLocation.Contains(l) = False Then allLocation.Add(l)
            Next
        Next

        Dim site As New SettlementSite
        With site
            If terrain = "" Then
                Dim terrainRoll As Integer = rng.Next(terrainDict.Keys.Count)
                .Terrain = terrainDict.Keys(terrainRoll)
            Else
                .Terrain = terrain
            End If
            Dim terrainList As List(Of String) = terrainDict(.Terrain)

            'roll number of locations (2-7)
            Dim numLocations As Integer = rng.Next(2, 8)
            For n = 1 To numLocations
                If n - 1 <= terrainList.Count - 1 Then
                    'get guaranteed location from terrainList
                    .LocationList.Add(terrainList(n - 1))
                Else
                    'roll random location
                    .LocationList.Add(GetRandom(allLocation))
                End If
            Next
        End With
        Return site
    End Function

#Region "World Map"
    Private Property X As Integer Implements iMapLocation.X
    Private Property Y As Integer Implements iMapLocation.Y
#End Region
End Class
