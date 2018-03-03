Public Class House
    Inherits Building
    Implements iSaveable


#Region "Constructors"
    Private Shared HouseNumber As Integer = 1
    Public Shared Function Import(ByVal targetName As String) As House
        Dim data As List(Of String) = IO.ImportSquareBracketSelect(IO.sbHouses, targetName)
        If data Is Nothing Then Throw New Exception("House type not found")

        Dim h As New House
        With h
            ._Name = targetName & " #" & HouseNumber
            HouseNumber += 1

            For Each line In data
                Dim ln As String() = line.Split(":")
                Dim header As String = ln(0).Trim
                Dim entry As String = ln(1).Trim
                .ParsedLoad(header, entry)
            Next
        End With
        Return h
    End Function
    Private Sub ParsedLoad(ByVal header As String, ByVal entry As String)
        Select Case header
            Case "Capacity" : ResidentCapacity = Convert.ToInt32(entry)
            Case "Morale" : HouseMorale = Convert.ToInt32(entry)

            Case "Resident"
                Dim possiblePeople As List(Of Person) = Settlement.GetResidents("name=" & entry.Replace(" ", "+"))
                If possiblePeople.Count > 0 Then Throw New Exception("More than one person with the same name.")
                AddResident(possiblePeople(0))

            Case Else : BaseParsedLoad(header, entry)
        End Select
    End Sub
#End Region

#Region "Save/Load"
    Private Function GetSaveListHeader() As String Implements iSaveable.GetSaveListHeader
        Return Name
    End Function
    Private Overloads Function GetSaveList() As System.Collections.Generic.List(Of String) Implements iSaveable.GetSaveList
        Dim total As New List(Of String)
        With total
            .AddRange(MyBase.BaseGetSaveList)
            .Add("Capacity:" & ResidentCapacity)
            .Add("Morale:" & HouseMorale)
            For Each r In Residents
                .Add("Resident:" & r.Name)
            Next
        End With
        Return total
    End Function
    Public Overloads Shared Function Load(ByVal raw As List(Of String)) As House
        Dim house As New House
        With house
            For Each ln In raw
                Dim fs As String() = ln.Split(":")
                Dim header As String = fs(0)
                Dim entry As String = fs(1)
                .ParsedLoad(header, entry)
            Next
        End With
        Return house
    End Function
#End Region

#Region "Personal Identifiers"
    Public Overrides Sub ConsoleReport()
        Console.WriteLine(Name)
        Console.WriteLine("└ Made By:   " & CreatorName & " in " & CreationDate.ToStringShort)
        Console.WriteLine("└ Residents: " & Residents.Count & "/" & ResidentCapacity)
        For Each r In Residents
            Console.WriteLine("  └ " & r.NameAndTitle)
        Next
        Console.WriteLine("└ Food:      " & FoodEaten.ToString)
        Console.WriteLine()
    End Sub
    Public Overrides Function ToString() As String
        Return _Name & " - " & Residents.Count & "/" & ResidentCapacity
    End Function
#End Region

#Region "Residents"
    Private ResidentCapacity As Integer = 5
    Private Residents As New List(Of Person)
    Public Function GetResident(ByVal name As String) As Person
        For Each r In Residents
            If r.Name = name Then Return r
        Next
        Return Nothing
    End Function
    Public Function GetResidents(ByVal flags As String) As List(Of Person)
        Dim total As New List(Of Person)
        For Each r In Residents
            If r.CheckFlags(flags) = True Then total.Add(r)
        Next
        Return total
    End Function
    Public Sub AddResident(ByVal p As Person)
        If Residents.Contains(p) = True Then Exit Sub

        Residents.Add(p)
    End Sub
    Public Function AddResidentCheck(ByVal p As Person) As Boolean
        If Residents.Count + 1 > ResidentCapacity Then Return False
        If p Is Nothing = False Then
            'person specific checks
        End If

        Return True
    End Function
    Public Sub RemoveResident(ByVal p As Person)
        If Residents.Contains(p) = False Then Exit Sub

        Residents.Remove(p)
    End Sub

    Private HouseMorale As Integer
    Private ReadOnly Property Morale As Integer
        Get
            'shortcircuit for starvation at flat -10
            If _IsStarving = True Then Return -20


            'total morale sources
            Dim total As Integer = HouseMorale
            total += FoodMorale

            Return total
        End Get
    End Property
#End Region

#Region "Food"
    Private _IsStarving As Boolean = False
    Public ReadOnly Property IsStarving As Boolean
        Get
            Return _IsStarving
        End Get
    End Property
    Private FoodEaten As New ResourceDict
    Public Sub AddFoodEaten(ByVal r As String, ByVal qty As Integer)
        If ResourceDict.GetCategory(r) <> "Food" Then Exit Sub
        FoodEaten.Add(r, qty)
    End Sub
    Public Sub RemoveFoodEaten(ByVal r As String, ByVal qty As Integer)
        If FoodEaten.ContainsKey(r) = False Then Exit Sub

        FoodEaten(r) -= qty
        If FoodEaten(r) <= 0 Then FoodEaten.RemoveKey(r)
    End Sub
    Public Sub RemoveFoodEaten()
        For n = 0 To FoodEaten.Keys.Count - 1 Step -1
            Dim k As String = FoodEaten.Keys(n)
            FoodEaten.RemoveKey(k)
        Next
    End Sub
    Private ReadOnly Property FoodEatenTotal As ResourceDict
        Get
            'actual amount of food eaten
            Return FoodEaten * Residents.Count
        End Get
    End Property
    Private ReadOnly Property FoodMorale As Integer
        Get
            Dim total As Integer = -10
            For Each f In FoodEaten.Keys
                total += FoodEaten(f) * 5
            Next
            Return total
        End Get
    End Property
#End Region

    Public Overrides Sub Tick()
        MyBase.Tick()

        'check for starvation
        If FoodEaten.Keys.Count > 0 Then
            'check if foodeaten can be met, then remove resources regardless (<0 handled in resourcedict.remove)
            If Settlement.CheckResources(FoodEatenTotal) = False Then _IsStarving = True Else _IsStarving = False
            Settlement.AddResources(FoodEatenTotal, True)
        Else
            'no food assigned to house, auto-starvation
            _IsStarving = True
        End If
        If _IsStarving = True Then World.AddAlert(Me, 3, Name & " is starving!")

        For n = Residents.Count - 1 To 0 Step -1
            Dim r As Person = Residents(n)
            r.Tick()
        Next
    End Sub
    Public Overrides Function GetTickWarnings() As List(Of Alert)
        Dim total As New List(Of Alert)
        If _IsStarving = True Then
            total.Add(New Alert(Me, 3, Name & " is starving!"))
        ElseIf FoodEaten.Count = 0 Then
            total.Add(New Alert(Me, 3, Name & " has no food!"))
        ElseIf Settlement.CheckResources(FoodEatenTotal) = False Then
            total.Add(New Alert(Me, 3, Name & " has insufficient food and will starve!"))
        End If
        Return total
    End Function
End Class
