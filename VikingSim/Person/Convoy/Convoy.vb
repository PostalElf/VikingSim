Public MustInherit Class Convoy
#Region "Constructors"
    Public Sub New(ByVal _leader As Person, ByVal _people As List(Of Person), ByVal _origin As iTradable, ByVal _destination As iTradable, _
                   ByVal _foodEaten As ResourceDict, ByVal _foodSupply As ResourceDict, Optional ByVal _isRoundtrip As Boolean = False)
        Leader = _leader
        People = _people
        Origin = _origin
        Destination = _destination
        FoodEaten = _foodEaten
        IsRoundTrip = _isRoundtrip

        ResetJourney()

        If TypeOf Origin Is Settlement Then
            Dim settlement As Settlement = CType(Origin, Settlement)
            settlement.RemoveResident(Leader)
            For Each p In People
                settlement.RemoveResident(p)
            Next
        End If
    End Sub
    Public Overrides Function ToString() As String
        Return Origin.Name & " -> " & Destination.Name & " [" & People.Count & "]"
    End Function
#End Region

#Region "People"
    Protected Leader As Person
    Protected People As New List(Of Person)
    Public Sub RemovePerson(ByVal p As Person)
        If People.Contains(p) = False Then Exit Sub
        People.Remove(p)
    End Sub
    Public Function CheckPerson(ByVal p As Person) As Boolean
        Return People.Contains(p)
    End Function
#End Region

#Region "World Map"
    Protected Origin As iTradable
    Protected Destination As iTradable
    Protected IsRoundTrip As Boolean

    Private TravelSpeed As Integer
    Private Progress As Integer
    Private ProgressThreshold As Integer
    Protected MustOverride Sub ArriveDestination()
    Protected Sub ResetJourney()
        Progress = 0
        TravelSpeed = Leader.GetInventoryBonus("TravelSpeed") + 10
        ProgressThreshold = Math.Round(World.GetDistance(Origin, Destination) * 10)
    End Sub
#End Region

#Region "Food"
    Private _IsStarving As Boolean
    Private FoodSupply As ResourceDict
    Private FoodEaten As ResourceDict
    Private ReadOnly Property FoodEatenTotal As ResourceDict
        Get
            'adding 1 to people count for leader
            Return FoodEaten * (People.Count + 1)
        End Get
    End Property
    Public Sub AddFoodEaten(ByVal r As String, ByVal qty As Integer)
        If ResourceDict.GetCategory(r) <> "Food" Then Exit Sub
        FoodEaten.Add(r, qty)
    End Sub
#End Region

    Public Sub Tick()
        'eat food
        If FoodEaten.Count > 0 Then
            If FoodSupply.CheckMin(FoodEatenTotal) Then _IsStarving = False Else _IsStarving = True
            FoodSupply.Remove(FoodEatenTotal)
        Else
            _IsStarving = True
        End If
        If _IsStarving = True Then World.AddAlert(Me, 3, "Convoy lead by " & Leader.Name & " is starving!")

        'people
        Leader.Tick()
        For n = People.Count - 1 To 0 Step -1
            Dim p As Person = People(n)
            p.Tick()
        Next

        'travel
        Progress += TravelSpeed
        If Progress >= ProgressThreshold Then ArriveDestination()
    End Sub
    Public Function GetTickWarnings() As List(Of Alert)
        Dim total As New List(Of Alert)
        If Progress + TravelSpeed >= ProgressThreshold Then total.Add(New Alert(Me, 1, "Convoy from " & Origin.Name & " will arrive in " & Destination.Name & " next week."))
        Return total
    End Function
End Class
