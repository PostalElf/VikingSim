Public Class Settlement
    Public Sub New()
        If AllResources.Count = 0 Then AllResources = ImportSquareBracketList("data/worldgen/resources.txt")
        For Each resCategory In AllResources.Keys
            For Each r In AllResources(resCategory)
                Resources.Add(r, 0)
            Next
        Next
    End Sub
    Public Shared Function Construct(ByVal settlementSite As SettlementSite) As Settlement
        Dim settlement As New Settlement
        With settlement
            .NaturalResources = settlementSite.NaturalResourceList
            .LandTotal = 100 - (.NaturalResources.Count * 10)
        End With
        Return settlement
    End Function

    Private NaturalResources As New List(Of NaturalResources)
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

    Public Function GetResidents(ByVal name As String, Optional ByVal flags As String = "")
        Dim total As New List(Of Person)
        For Each h In Houses
            total.AddRange(h.GetResidents(name, flags))
        Next
        Return total
    End Function
    Public Function GetBestSkillUnemployed(ByVal skill As Skill) As Person
        Dim residents As List(Of Person) = GetResidents("", "unemployed")
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
        Dim residents As List(Of Person) = GetResidents("", "unemployed")
        Dim bestFit As Person = Nothing
        Dim bestAffinity As Double = -1
        For Each r In residents
            If r.skillaffinity(skill) > bestAffinity Then
                bestAffinity = r.skillaffinity(skill)
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
    Public Function GetEmployableWorkplaces() As List(Of Workplace)
        Dim total As New List(Of Workplace)
        For Each b In Buildings
            If TypeOf b Is Workplace Then
                Dim w As Workplace = CType(b, Workplace)
                If w.AddWorkerCheck(Nothing) = True Then total.Add(w)
            End If
        Next
        Return total
    End Function
    Public Function GetEmptyHouses() As List(Of House)
        Dim total As New List(Of House)
        For Each b In Buildings
            If TypeOf b Is House Then
                Dim h As House = CType(b, House)
                If h.AddResidentcheck(Nothing) = True Then total.Add(h)
            End If
        Next
        Return total
    End Function

    Private Shared AllResources As New Dictionary(Of String, List(Of String))
    Private Resources As New Dictionary(Of String, Integer)
    Public Sub AddResources(ByVal res As Dictionary(Of String, Integer), Optional ByVal remove As Boolean = False)
        For Each r In res.Keys
            If remove = True Then Resources(r) -= res(r) Else Resources(r) += res(r)
        Next
    End Sub
    Public Sub AddResources(ByVal r As String, ByVal qty As Integer)
        Resources(r) += qty
    End Sub
    Public Function CheckResources(ByVal res As Dictionary(Of String, Integer)) As Boolean
        For Each r In res.Keys
            If Resources(r) < res(r) Then Return False
        Next
        Return True
    End Function
End Class
