Public Class House
    Inherits Building

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
                Select Case header
                    Case "Capacity" : .ResidentCapacity = Convert.ToInt32(entry)
                    Case "Morale" : .HouseMorale = Convert.ToInt32(entry)
                    Case Else : .BaseImport(header, entry)
                End Select
            Next
        End With
        Return h
    End Function
#End Region

#Region "Personal Identifiers"
    Public Overrides Sub ConsoleReport()
        Console.WriteLine(Name)
        Console.WriteLine("└ Made By:   " & CreatorName & " in " & CreationDate.ToStringShort)
        Console.WriteLine("└ Residents: " & Residents.Count & "/" & ResidentCapacity)
        For Each r In Residents
            Console.WriteLine("  └ " & r.Name)
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
    Public Function GetResidents(ByVal name As String, Optional ByVal flags As String = "") As List(Of Person)
        Dim total As New List(Of Person)
        For Each r In Residents
            If name = "" OrElse r.Name = name Then
                If r.CheckFlags(flags) = True Then total.Add(r)
            End If
        Next
        Return total
    End Function
    Public Sub AddResident(ByVal p As Person)
        If Residents.Contains(p) = True Then Exit Sub

        Residents.Add(p)
    End Sub
    Public Function AddResidentcheck(ByVal p As Person) As Boolean
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
            If IsStarving = True Then Return -20


            'total morale sources
            Dim total As Integer = HouseMorale
            total += FoodMorale

            Return total
        End Get
    End Property
#End Region

#Region "Food"
    Private IsStarving As Boolean = False
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
            If Settlement.CheckResources(FoodEatenTotal) = False Then IsStarving = True Else IsStarving = False
            Settlement.AddResources(FoodEatenTotal, True)
        Else
            'no food assigned to house, auto-starvation
            IsStarving = True
        End If
        If IsStarving = True Then World.AddAlert(Me, 3, Name & " is starving!")

        For n = Residents.Count - 1 To 0 Step -1
            Dim r As Person = Residents(n)
            r.Tick()
        Next
    End Sub
    Public Overrides Function GetTickWarnings() As List(Of Alert)
        Dim total As New List(Of Alert)
        If IsStarving = True Then
            total.Add(New Alert(Me, 3, Name & " is starving!"))
        ElseIf FoodEaten.Count = 0 Then
            total.Add(New Alert(Me, 3, Name & " has no food!"))
        ElseIf Settlement.CheckResources(FoodEatenTotal) = False Then
            total.Add(New Alert(Me, 3, Name & " has insufficient food and will starve!"))
        End If
        Return total
    End Function
End Class
