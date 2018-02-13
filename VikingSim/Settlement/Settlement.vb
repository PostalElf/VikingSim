﻿Public Class Settlement
    Implements iModifiable

#Region "Constructors"
    Public Sub New()
        Dim allResources As Dictionary(Of String, List(Of String)) = IO.ImportSquareBracketList(IO.sbResources)
        For Each resCategory In allResources.Keys
            For Each r In allResources(resCategory)
                Resources.Add(r, 0)
                BaseResourceCapacity.Add(r, 200)
            Next
        Next
    End Sub
    Public Shared Function Construct(ByVal settlementSite As SettlementSite, Optional ByVal name As String = "") As Settlement
        Dim settlement As New Settlement
        With settlement
            If name <> "" Then ._Name = name
            .Locations = settlementSite.LocationList
            .LandTotal = 100 - (.Locations.Count * 10)
        End With
        Return settlement
    End Function
#End Region

#Region "Personal Identifiers"
    Private _Name As String
    Public ReadOnly Property Name As String
        Get
            Return _Name
        End Get
    End Property

    Public Overrides Function ToString() As String
        Return _Name
    End Function
    Public Sub ConsoleReport()
        Console.WriteLine(Name)
        Console.WriteLine("└ Residents: " & GetResidents("").Count)
        Console.WriteLine("└ Buildings: " & Buildings.Count)
        Dim landPercent As String = ((LandFree / LandTotal) * 100).ToString("0")
        Console.WriteLine("└ Open Land: " & LandFree & "/" & LandTotal & " (" & landPercent & "%)")
        Console.WriteLine("└ Locations: ")
        For Each l In Locations
            Console.WriteLine("  └ " & l.ToString)
        Next
        Console.WriteLine("└ Resources: ")
        For Each r In Resources.Keys
            Dim value As Integer = Resources(r)
            If value > 0 Then Console.WriteLine("  └ " & r & ": " & value)
        Next
    End Sub
#End Region

#Region "Settlement Site"
    Private Locations As New List(Of String)
    Public Sub AddLocation(ByVal l As String)
        Locations.Add(l)
    End Sub
    Public Sub RemoveLocation(ByVal l As String)
        If Locations.Contains(l) = False Then Throw New Exception("Settlement.Locations does not contain " & l)
        Locations.Remove(l)
    End Sub
    Public Function GetLocations(ByVal targetName As String) As List(Of String)
        Dim total As New List(Of String)
        For Each l In Locations
            If l = targetName Then total.Add(l)
        Next
        Return total
    End Function

    Private LandTotal As Integer
    Private ReadOnly Property LandUsed As Integer
        Get
            Dim total As Integer = 0
            For Each b In Buildings
                total += b.LandUsed
            Next
            Return total
        End Get
    End Property
    Private ReadOnly Property LandFree As Integer
        Get
            Return LandTotal - LandUsed
        End Get
    End Property
#End Region

#Region "Modifier"
    Public Property ModifierList As New List(Of Modifier) Implements iModifiable.ModifierList
    Public Sub RemoveModifier(ByVal m As Modifier) Implements iModifiable.RemoveModifier
        If ModifierList.Contains(m) = False Then Exit Sub
        ModifierList.Remove(m)
    End Sub
    Public Sub TickModifier() Implements iModifiable.TickModifier
        For n = ModifierList.Count - 1 To 0 Step -1
            Dim m As Modifier = ModifierList(n)
            m.Tick()
        Next
    End Sub
#End Region

#Region "Residents"
    Public Function GetResidents(ByVal name As String, Optional ByVal flags As String = "") As List(Of Person)
        Dim total As New List(Of Person)
        For Each h In Houses
            total.AddRange(h.GetResidents(name, flags))
        Next
        Return total
    End Function
    Public Function GetBestSkillUnemployed(ByVal skill As Skill) As Person
        Dim residents As List(Of Person) = GetResidents("", "employable")
        Dim bestFit As Person = Nothing
        Dim bestSkill As Integer = -1
        For Each r In residents
            If r.SkillRank(skill) > bestSkill Then
                bestSkill = r.SkillRank(skill)
                bestFit = r
            End If
        Next
        Return bestFit
    End Function
    Public Function GetBestAffinityUnemployed(ByVal skill As Skill) As Person
        Dim residents As List(Of Person) = GetResidents("", "employable")
        Dim bestFit As Person = Nothing
        Dim bestAffinity As Double = -1
        For Each r In residents
            If r.SkillAffinity(skill) > bestAffinity Then
                bestAffinity = r.SkillAffinity(skill)
                bestFit = r
            End If
        Next
        Return bestFit
    End Function
    Public Function GetSingleCouple() As List(Of Person)
        Dim singles As List(Of List(Of Person)) = GetSingleCouples()
        Dim girls As List(Of Person) = singles(0)
        Dim boys As List(Of Person) = singles(1)

        Dim pair As New List(Of Person)
        pair.Add(GetRandom(Of Person)(girls))
        pair.Add(GetRandom(Of Person)(boys))
        Return pair
    End Function
    Public Function GetSingleCouples() As List(Of List(Of Person))
        Dim girls As List(Of Person) = GetResidents("", "single female")
        If girls.Count = 0 Then Return Nothing
        Dim boys As List(Of Person) = GetResidents("", "single male")
        If boys.Count = 0 Then Return Nothing

        Dim total As New List(Of List(Of Person))
        total.Add(girls)
        total.Add(boys)
        Return total
    End Function
#End Region

#Region "Buildings"
    Private Buildings As New List(Of Building)
    Private ReadOnly Property Houses As List(Of House)
        Get
            Dim total As New List(Of House)
            For Each b In Buildings
                If TypeOf b Is House Then total.Add(CType(b, House))
            Next
            Return total
        End Get
    End Property
    Public Sub AddBuilding(ByVal b As Building)
        Buildings.Add(b)
        b.Settlement = Me
    End Sub
    Public Function AddBuildingCheck(ByVal b As Building) As Boolean
        If LandUsed + b.LandUsed > LandTotal Then Return False

        Return True
    End Function
    Public Sub RemoveBuilding(ByVal b As Building)
        If Buildings.Contains(b) = False Then Exit Sub
        b.Inventory.DumpItems(Me)
        b.Settlement = Nothing
        Buildings.Remove(b)
    End Sub
    Public Function GetBuildings(Optional ByVal flags As String = "") As List(Of Building)
        Dim total As New List(Of Building)
        For Each b In Buildings
            If b.CheckFlags(flags) = True Then total.Add(b)
        Next
        Return total
    End Function
#End Region

#Region "Resources"
    Private BaseResourceCapacity As New ResourceDict
    Private Resources As New ResourceDict
    Public Sub AddResources(ByVal res As ResourceDict, Optional ByVal remove As Boolean = False)
        For Each r In res.Keys
            If remove = True Then AddResources(r, -res(r)) Else AddResources(r, res(r))
        Next
    End Sub
    Public Sub AddResources(ByVal r As String, ByVal qty As Integer)
        Resources(r) += qty
        If Resources(r) >= GetResourceCapacity(r) Then Resources(r) = GetResourceCapacity(r)
        If Resources(r) < 0 Then Resources(r) = 0
    End Sub
    Private Function GetResourceCapacity(ByVal r As String) As Integer
        Dim total As Integer = BaseResourceCapacity(r)
        Dim storages As List(Of Building) = GetBuildings("storage")
        For Each storage In storages
            total += CType(storage, Storage).GetCapacity(r)
        Next
        Return total
    End Function
    Public Function CheckResources(ByVal res As ResourceDict) As Boolean
        For Each r In res.Keys
            If Resources(r) < res(r) Then Return False
        Next
        Return True
    End Function
    Public Function CheckResources(ByVal r As String, ByVal qty As Integer) As Boolean
        If Resources.ContainsKey(r) = False Then Return False
        If Resources(r) <= qty Then Return False

        Return True
    End Function

    Private Inventory As New Inventory
    Public Sub AddItem(ByVal item As Item)
        Inventory.AddItem(item)
    End Sub
#End Region

    Public Sub Tick()
        TickModifier()

        For n = Buildings.Count - 1 To 0 Step -1
            Dim b As Building = Buildings(n)
            b.Tick()
        Next
    End Sub

End Class
