Public Class SettlementSite
    Implements iMapLocation, iTickable
    Public ReadOnly Property Name As String Implements iMapLocation.Name
        Get
            Return Terrain
        End Get
    End Property
    Public Overrides Function ToString() As String
        Return Name
    End Function

    Private Terrain As String
    Public LocationList As New List(Of String)
    Public Shared Function Construct(ByVal px As Integer, ByVal py As Integer, Optional ByVal terrain As String = Nothing) As SettlementSite
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
            .X = px
            .Y = py

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

    Public Function CheckFlags(ByVal flags As String) As Boolean Implements iMapLocation.CheckFlags
        For Each f In flags.Split(",")
            Select Case f.Trim
                Case "settlementsite" 'do nothing if true
                Case Else : Return False
            End Select
        Next
        Return True
    End Function
    Public Sub Tick(ByVal parent As iTickable) Implements iTickable.Tick
        Const ConversionRate As Integer = 5
        If rng.Next(1, 101) <= ConversionRate Then
            Dim world As World = TryCast(parent, World)
            If world Is Nothing Then Exit Sub
            world.RemoveMapLocation(Me)

            Dim village As Village = village.construct(Me)
            world.AddMapLocation(village)
            world.AddAlert(village, 2, "A new village has been founded.")
        End If
    End Sub
    Public Function GetTickWarnings() As List(Of Alert) Implements iTickable.GetTickWarnings
        Return Nothing
    End Function

#Region "World Map"
    Private Property X As Integer Implements iMapLocation.X
    Private Property Y As Integer Implements iMapLocation.Y
#End Region
End Class
