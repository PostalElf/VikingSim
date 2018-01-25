Public Class SettlementSite
    Public Terrain As String
    Public NaturalResourceList As New List(Of NaturalResources)
    Public Shared Function Construct(Optional ByVal targetTerrain As String = "") As SettlementSite
        Dim site As New SettlementSite
        With site
            'roll terrain
            Dim terrainDict As Dictionary(Of String, List(Of String)) = ImportSquareBracketList("data/worldgen/terrain.txt")
            If targetTerrain = "" Then
                Dim terrainList As New List(Of String)
                For Each t In terrainDict.Keys
                    terrainList.Add(t)
                Next
                .Terrain = GetRandom(Of String)(terrainList)
            Else
                .Terrain = targetTerrain
            End If

            'add 2-6 resources
            Dim count As Integer = rng.Next(2, 7)
            Dim possibleResources As New List(Of String)
            For n = 0 To count
                If possibleResources.Count = 0 Then possibleResources.AddRange(terrainDict(.Terrain))
                Dim resource As String = GrabRandom(Of String)(possibleResources)

                .NaturalResourceList.Add(NaturalResources.Construct(resource))
            Next
        End With
        Return site
    End Function
End Class
