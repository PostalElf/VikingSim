Public Class Settlement
    Implements iModifiable, iMapLocation, iHistorable

#Region "Constructors"
    Public Sub New()
        Dim allResources As Dictionary(Of String, List(Of String)) = IO.ImportSquareBracketList(IO.sbResources)
        For Each resCategory In allResources.Keys
            For Each r In allResources(resCategory)
                Resources.Add(r, 0)
                BaseResourceCapacity.Add(r, ResourceStartingCapacity)
            Next
        Next
    End Sub
    Public Shared Function Construct(ByVal settlementSite As SettlementSite, Optional ByVal name As String = "") As Settlement
        Dim settlement As New Settlement
        With settlement
            Dim ss As iMapLocation = CType(settlementSite, iMapLocation)
            .X = ss.X
            .Y = ss.Y

            If name <> "" Then ._Name = name
            .Locations = settlementSite.LocationList
            .LandTotal = 100 - (.Locations.Count * 10)
            .SetHistory("Odinsson", World.TimeNow)
        End With
        Return settlement
    End Function
#End Region

#Region "Personal Identifiers"
    Private _Name As String
    Public ReadOnly Property Name As String Implements iMapLocation.Name
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

    Public Function CheckFlags(ByVal flags As String) As Boolean
        Dim fs As String() = flags.Split(" ")
        For Each f In fs
            Select Case f
                Case "starving" : If HasStarvingHouse() = False Then Return False
            End Select
        Next
        Return True
    End Function
    Private Function HasStarvingHouse() As Boolean
        For Each h In Houses
            If h.IsStarving = True Then Return True
        Next
        Return False
    End Function
#End Region

#Region "World Map"
    Private Locations As New List(Of String)
    Public Sub AddLocation(ByVal l As String)
        Locations.Add(l)
    End Sub
    Public Sub RemoveLocation(ByVal l As String)
        If Locations.Contains(l) = False Then Exit Sub
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

    Private Property X As Integer Implements iMapLocation.X
    Private Property Y As Integer Implements iMapLocation.Y
#End Region

#Region "History"
    Private Property CreationDate As CalendarDate Implements iHistorable.CreationDate
    Private Property CreatorName As String Implements iHistorable.CreatorName
    Private Sub SetHistory(ByVal cr As String, ByVal cd As CalendarDate) Implements iHistorable.SetHistory
        CreatorName = cr
        CreationDate = New CalendarDate(cd)
    End Sub
    Private Function HistoryReport() As String Implements iHistorable.HistoryReport
        Return "Founded by " & CreatorName & " in " & CreationDate.ToStringShort
    End Function
#End Region

#Region "Modifier"
    Public Property ModifierList As New List(Of Modifier) Implements iModifiable.ModifierList
    Public Function GetModifier(ByVal quality As String) As Integer Implements iModifiable.GetModifier
        Return Modifier.GetModifier(quality, ModifierList)
    End Function
    Public Function GetModifiers(ByVal category As String) As List(Of Modifier) Implements iModifiable.GetModifiers
        Return Modifier.GetModifiers(category, ModifierList)
    End Function
    Private Sub AddModifier(ByVal m As Modifier) Implements iModifiable.AddModifier
        Modifier.AddModifier(m, Me)
    End Sub
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
    Public Function GetResidents(ByVal flags As String) As List(Of Person)
        Dim total As New List(Of Person)
        For Each h In Houses
            total.AddRange(h.GetResidents(flags))
        Next
        Return total
    End Function
    Public Function GetResidentBest(ByVal flags As String, ByVal comparer As String) As Person
        Dim bestFit As Person = Nothing
        Dim bestValue As Integer = -1

        Dim fs As String() = comparer.Split("=")
        Dim header As String = fs(0)
        Dim entry As String = fs(1)

        Dim residents As List(Of Person) = GetResidents(flags)
        For Each r In residents
            Select Case header
                Case "skill"
                    Dim skill As Skill = StringToEnum(Of Skill)(entry)
                    If r.SkillRank(skill) > bestValue Then
                        bestFit = r
                        bestValue = r.SkillRank(skill)
                    End If

                Case "affinity"
                    Dim skill As Skill = StringToEnum(Of Skill)(entry)
                    If r.SkillAffinity(skill) > bestValue Then
                        bestFit = r
                        bestValue = r.SkillAffinity(skill)
                    End If
            End Select
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
        Dim girls As List(Of Person) = GetResidents("single female")
        If girls.Count = 0 Then Return Nothing
        Dim boys As List(Of Person) = GetResidents("single male")
        If boys.Count = 0 Then Return Nothing

        Dim total As New List(Of List(Of Person))
        total.Add(girls)
        total.Add(boys)
        Return total
    End Function

    Public Sub RemoveResident(ByVal r As Person)
        For Each h In Houses
            If h.GetResident(r.Name) Is Nothing = False Then
                h.RemoveResident(r)
                Exit Sub
            End If
        Next
    End Sub
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
    Private Const ResourceStartingCapacity As Integer = 100
    Private BaseResourceCapacity As New ResourceDict
    Private Resources As New ResourceDict
    Public Sub AddResources(ByVal res As ResourceDict, Optional ByVal remove As Boolean = False)
        For Each r In res.Keys
            If remove = True Then AddResources(r, -res(r)) Else AddResources(r, res(r))
        Next
    End Sub
    Public Sub AddResources(ByVal r As String, ByVal qty As Integer)
        Resources.Add(r, qty)
        If Resources(r) > GetResourceCapacity(r) Then Resources(r) = GetResourceCapacity(r)
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
    Public Sub AddItems(ByVal items As List(Of Item))
        For Each i In items
            Inventory.AddItem(i)
        Next
    End Sub
    Public Sub AddItems(ByVal inventory As Inventory)
        inventory.DumpItems(Me)
    End Sub

    Private Property TradeOutpost As New TradeOutpost(Me) Implements iMapLocation.TradeOutpost
    Private Convoys As New List(Of Convoy)
    Public Sub AddConvoy(ByVal Convoy As Convoy)
        If Convoys.Contains(Convoy) Then Exit Sub
        Convoys.Add(Convoy)
    End Sub
    Public Sub RemoveConvoy(ByVal convoy As Convoy)
        If Convoys.Contains(convoy) = False Then Exit Sub
        Convoys.Remove(convoy)
    End Sub
#End Region

    Public Sub Tick()
        TickModifier()

        For n = Buildings.Count - 1 To 0 Step -1
            Dim b As Building = Buildings(n)
            b.Tick()
        Next

        For n = Convoys.Count - 1 To 0 Step -1
            Dim c As Convoy = Convoys(n)
            c.Tick()
        Next
    End Sub
    Public Function GetTickWarnings() As List(Of Alert)
        Dim total As New List(Of Alert)
        For n = Buildings.Count - 1 To 0 Step -1
            Dim b As Building = Buildings(n)
            Dim bWarnings As List(Of Alert) = b.GetTickWarnings()
            If bWarnings Is Nothing = False Then total.AddRange(bWarnings)
        Next
        For n = Convoys.Count - 1 To 0 Step -1
            Dim c As Convoy = Convoys(n)
            Dim cWarnings As List(Of Alert) = c.GetTickWarnings
            If cWarnings Is Nothing = False Then total.AddRange(cWarnings)
        Next
        Return total
    End Function

End Class
