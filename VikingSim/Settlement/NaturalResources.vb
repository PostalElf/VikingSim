﻿Public Class NaturalResources
    Private Data As New Dictionary(Of String, Integer)
    Public Name As String

    Public Shared Function Construct(ByVal targetName As String) As NaturalResources
        Dim rawData As List(Of String) = ImportSquareBracketSelect("data/naturalresources.txt", targetName)

        Dim nr As New NaturalResources
        With nr
            .Name = targetName

            'populate drops
            Dim commons As New List(Of String)
            Dim rares As New List(Of String)
            For Each entry In rawData
                Dim entrySplit As String() = entry.Split(":")
                Dim header As String = entrySplit(0).Trim
                Dim data As String = entrySplit(1).Trim

                For Each s In data.Split(",")
                    If header = "Common" Then commons.Add(s.Trim)
                    If header = "Rare" Then rares.Add(s.Trim)
                Next
            Next

            'pick a common (3-6) resource first
            Dim targetResource As String = GrabRandom(Of String)(commons)
            Dim targetQty As Integer = rng.Next(3, 7)
            Dim qtyRemaining As Integer = 10 - targetQty
            .Add(targetResource, targetQty)

            'pick a rare (1-3) resource next
            If rares.Count > 0 Then
                targetResource = GrabRandom(Of String)(rares)
                targetQty = rng.Next(1, 4)
                qtyRemaining -= targetQty
                .Add(targetResource, targetQty)
            End If

            'fill up remainder with common (1-4) or rare (1-4)
            While qtyRemaining > 0
                Dim targetList As List(Of String)
                If commons.Count > 0 Then
                    targetList = commons
                ElseIf rares.Count > 0 Then
                    targetList = rares
                Else
                    Throw New Exception("Insufficient resources in " & .Name & "; should have at least 3")
                End If

                targetResource = GetRandom(Of String)(targetList)
                targetQty = rng.Next(1, 5)
                If targetQty > qtyRemaining Then targetQty = qtyRemaining
                qtyRemaining -= targetQty
                .Add(targetResource, targetQty)
            End While
        End With
        Return nr
    End Function
    Public Sub Add(ByVal key As String, ByVal value As Integer)
        If Data.ContainsKey(key) = False Then Data.Add(key, 0)
        Data.Item(key) += value
    End Sub
    Public Function Keys() As List(Of String)
        Dim total As New List(Of String)
        For Each k In Data.Keys
            total.Add(k.ToString)
        Next
        Return total
    End Function
    Default Public Property Item(ByVal key As String) As Integer
        Get
            Return Data(key)
        End Get
        Set(ByVal value As Integer)
            Data(key) = value
        End Set
    End Property
    Public Overrides Function ToString() As String
        Return Name & " (Count: " & Data.Keys.Count & ")"
    End Function
End Class
