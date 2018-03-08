Public Class Village
    Implements iTradable, iMapLocation, iBuildable, iTickable

#Region "Constructors"
    Public Shared Function Construct(ByVal ib As iBuildable)
        Dim village As New Village
        With village
            .X = ib.X
            .Y = ib.Y
            .Terrain = ib.Terrain
            .Locations = ib.Locations

            'populate 3x worked locations, with mild effort to get different locations
            Dim WorkedLocations As New List(Of String)
            For n = 1 To 3
                Dim l As String = GetRandom(.Locations)
                If WorkedLocations.Contains(l) Then l = GetRandom(.Locations)
                WorkedLocations.Add(l)
            Next
            .SetBuySell(WorkedLocations)
        End With
        Return village
    End Function
    Private Sub SetBuySell(ByVal workedLocations As List(Of String))
        'determine production and consumption rates based on workedLocations
        For Each wl In workedLocations
            Dim possibleProduced As String()
            Dim possibleProducedRate As Integer()
            Select Case wl
                Case "Elderwoods"
                    possibleProduced = {"Hardwood", "Softwood"}
                    possibleProducedRate = {2, 2}
                Case "Godbones"
                Case "Forest"
                Case "Hills"
                Case "River"
                Case "Marsh"
                Case Else
                    Throw New Exception("Unrecognised location type: " & wl)
                    Continue For
            End Select

            'roll to determine production for this location
            Dim roll As Integer = rng.Next(possibleProduced.Count)
            Dim r As String = possibleProduced(roll)
            Dim qty As Integer = possibleProducedRate(roll)
            ProducedResourcesRate.Add(r, qty)

            'TODO: determine consumption based on production

        Next
    End Sub
#End Region

#Region "Personal Identifiers"
    Private _Name As String
    Public ReadOnly Property Name As String Implements iMapLocation.Name, iTradable.Name
        Get
            Return _Name
        End Get
    End Property
    Public Overrides Function ToString() As String
        Return "Village - " & Name
    End Function
    Public Function CheckFlags(ByVal flags As String) As Boolean Implements iMapLocation.CheckFlags
        For Each f In flags.Split(",")
            Select Case f.Trim
                Case "village" 'do nothing if true
                Case Else : Return False
            End Select
        Next
        Return True
    End Function

    Private Property X As Integer Implements iMapLocation.X, iTradable.X, iBuildable.X
    Private Property Y As Integer Implements iMapLocation.Y, iTradable.Y, iBuildable.Y

    Private Property Terrain As String Implements iBuildable.Terrain
    Private Property Locations As List(Of String) Implements iBuildable.Locations
#End Region

#Region "Trade"
    Private Property TradeOutpost As TradeOutpost Implements iTradable.TradeOutpost
    Private ProducedResourcesRate As New ResourceDict
    Private ProducedResources As New ResourceDict
    Private ConsumedResourcesRate As New ResourceDict
    Private ConsumedResources As New ResourceDict
#End Region

    Public Sub Tick(ByVal parent As iTickable) Implements iTickable.Tick
        'tick for ProducedResources and ConsumedResources
        Const ProductionThreshold As Integer = 100
        Const ConsumptionThreshold As Integer = 100
        For Each r In ProducedResourcesRate.Keys
            ProducedResources.Add(r, ProducedResourcesRate(r))
            If ProducedResources(r) >= ProductionThreshold Then TradeOutpost.AddSaleItem(r, 1)
        Next
        For Each r In ConsumedResourcesRate.Keys
            ConsumedResources.Add(r, ConsumedResourcesRate(r))
            If ConsumedResources(r) >= ConsumptionThreshold Then TradeOutpost.AddBuyItem(r, 1)
        Next
    End Sub
    Public Function GetTickWarnings() As List(Of Alert) Implements iTickable.GetTickWarnings
        Return Nothing
    End Function
End Class
