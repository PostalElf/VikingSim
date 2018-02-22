﻿Public MustInherit Class Convoy
    Protected Leader As Person
    Protected People As New List(Of Person)
    Protected Origin As iMapLocation
    Protected Destination As iMapLocation
    Protected IsRoundTrip As Boolean

    Private TravelSpeed As Integer
    Private Progress As Integer
    Private ProgressThreshold As Integer

    Public Sub New(ByVal _leader As Person, ByVal _people As List(Of Person), ByVal _origin As iMapLocation, ByVal _destination As iMapLocation, Optional ByVal _isRoundtrip As Boolean = False)
        Leader = _leader
        People = _people
        Origin = _origin
        Destination = _destination
        IsRoundTrip = _isRoundtrip

        TravelSpeed = Leader.GetInventoryBonus("TravelSpeed") + 10
        ProgressThreshold = Math.Round(World.GetDistance(_origin, _destination) * 10)
    End Sub

    Public Overrides Function ToString() As String
        Return Origin.Name & " -> " & Destination.Name & " [" & People.Count & "]"
    End Function

    Public Sub Tick()
        Progress += TravelSpeed
        If Progress >= ProgressThreshold Then ArriveDestination()
    End Sub
    Protected MustOverride Sub ArriveDestination()
    Public Function GetTickWarnings() As List(Of Alert)
        Dim total As New List(Of Alert)
        If Progress + TravelSpeed >= ProgressThreshold Then total.Add(New Alert(Me, 1, "Convoy from " & Origin.Name & " will arrive in " & Destination.Name & " next week."))
        Return total
    End Function
End Class