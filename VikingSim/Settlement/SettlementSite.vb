Public Class SettlementSite
    Implements iMapLocation, iTickable, iBuildable
    Public ReadOnly Property Name As String Implements iMapLocation.Name
        Get
            Return Terrain
        End Get
    End Property
    Public Function CheckFlags(ByVal flags As String) As Boolean Implements iMapLocation.CheckFlags
        For Each f In flags.Split(",")
            Select Case f.Trim
                Case "settlementsite" 'do nothing if true
                Case Else : Return False
            End Select
        Next
        Return True
    End Function
    Public Overrides Function ToString() As String
        Return Name
    End Function

    Private Property Terrain As String Implements iBuildable.Terrain
    Private Property LocationList As New List(Of String) Implements iBuildable.Locations
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

    Private Shared WeeksSinceLastVillage As Integer = 0
    Private ReadOnly Property ConversionRate(ByVal world As World) As Integer
        Get
            Dim total As Integer = Math.Ceiling(WeeksSinceLastVillage / 3)
            Dim villages As List(Of iMapLocation) = world.GetMapLocations(GetType(Village))
            Select Case villages.Count
                Case Is < 3 : total += 5
                Case Is > 5 : total -= 5
            End Select
            If total <= 0 Then total = 1
            If total > 100 Then total = 100
            Return total
        End Get
    End Property
    Public Sub Tick(ByVal parent As iTickable) Implements iTickable.Tick
        Dim world As World = TryCast(parent, World)
        If rng.Next(1, 101) <= ConversionRate(world) Then
            world.RemoveMapLocation(Me)

            Dim village As Village = village.Construct(Me)
            world.AddMapLocation(village)
            world.AddAlert(village, 2, "A new village has been founded.")
            WeeksSinceLastVillage = 0
        Else
            WeeksSinceLastVillage += 1
        End If
    End Sub
    Public Function GetTickWarnings() As List(Of Alert) Implements iTickable.GetTickWarnings
        Return Nothing
    End Function

#Region "World Map"
    Private Property X As Integer Implements iMapLocation.X, iBuildable.X
    Private Property Y As Integer Implements iMapLocation.Y, iBuildable.Y
#End Region
End Class
