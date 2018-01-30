Module Module1
    Public rng As New Random(3)

    Sub Main()
        Console.SetWindowSize(100, 45)
        Dim world As New World
        Dim settlement As Settlement = BuildSettlement()
        world.addsettlement(settlement)

        Dim bMenu As New List(Of String)
        bMenu.Add("Tick")
        bMenu.Add("Review Settlement")
        bMenu.Add("Review Buildings")
        bMenu.Add("Review Residents")
        bMenu.Add("Marry Residents")
        bMenu.Add("Birth Resident")
        bMenu.Add("Add Building")
        bMenu.Add("Add Location")

        Dim doExit As Boolean = False
        While doExit = False
            Console.Clear()
            Select Case Menu.getListChoice(bMenu, 0, "Select option:")
                Case 0, -1 : doExit = True
                Case "Tick" : world.Tick()
                Case "Review Settlement" : MenuReviewSettlement(settlement)
                Case "Review Buildings" : MenuReviewBuildings(settlement)
                Case "Review Residents" : MenuReviewResidents(settlement)
                Case "Marry Residents" : MenuMarryResidents(settlement)
                Case "Birth Resident" : MenuBirthResident(settlement)
                Case "Add Building" : MenuAddBuilding(settlement)
                Case "Add Location" : MenuAddLocation(settlement)
            End Select
        End While
    End Sub
    Private Function BuildSettlement() As Settlement
        Dim site As SettlementSite = SettlementSite.Construct("Hilly")
        Dim settlement As Settlement = settlement.Construct(site)

        Dim godfather As Person = Person.Ancestor("Male")
        Dim godmother As Person = Person.Ancestor("Female")
        Dim house As House = Nothing

        For n = 1 To 10
            Dim child As Person = Person.Birth(godfather, godmother)
            If n Mod 2 <> 0 Then
                house = house.Import("Hut")
                house.SetHistory("Odin", World.TimeNow)
                settlement.AddBuilding(house)
            End If
            child.MoveHouse(house)
        Next

        Dim wp = WorkplaceProjector.Import("Carpenter")
        wp.SetHistory("Odin", World.TimeNow)
        settlement.AddBuilding(wp)
        settlement.GetBestAffinityUnemployed(wp.Occupation).ChangeWorkplace(wp)
        wp.AddProject("Deepmines", settlement.GetLocations("Godbones")(0))
        For n = 1 To 200
            wp.Tick()
        Next

        Return settlement
    End Function

    Private Sub MenuReviewSettlement(ByVal settlement As Settlement)
        settlement.ConsoleReport()
        Console.ReadLine()
    End Sub
    Private Sub MenuReviewBuildings(ByVal settlement As Settlement)
        Dim choice As String = Menu.getListChoice(New List(Of String) From {"House", "Producer", "Projector"}, 1, "Select type of building:")
        Dim buildings As List(Of Building) = settlement.GetBuildings(choice)
        Console.WriteLine()
        Dim b As Building = Menu.getListChoice(buildings, 1, "Select building:")
        Console.WriteLine()
        b.ConsoleReport()
        Console.ReadLine()
    End Sub
    Private Sub MenuReviewResidents(ByVal settlement As Settlement)
        Dim selection As Person = Menu.getListChoice(Of Person)(settlement.GetResidents(""), 1, "Select resident:")
        selection.ConsoleReport()
        Console.ReadLine()
        Console.Clear()
    End Sub
    Private Sub MenuMarryResidents(ByVal settlement As Settlement)
        Dim houses As List(Of Building) = settlement.GetBuildings("house space")
        If houses.Count = 0 Then Console.WriteLine("No free house!") : Console.ReadLine() : Exit Sub

        Dim men As List(Of Person) = settlement.GetResidents("", "single male")
        Dim women As List(Of Person) = settlement.GetResidents("", "single female")
        If men.Count = 0 OrElse women.Count = 0 Then Console.WriteLine("Insufficient singles!") : Console.ReadLine() : Exit Sub
        Dim husband As Person = Menu.getListChoice(men, 1, "Select husband:")
        Console.WriteLine()
        Dim wife As Person = Menu.getListChoice(women, 1, "Select wife:")
        Console.WriteLine()
        Dim house As House = Menu.getListChoice(houses, 1, "Select house:")

        husband.Marry(wife, house)
        Console.WriteLine(husband.Name & " has married " & wife.Name & ".")
        Console.ReadLine()
        Console.Clear()
    End Sub
    Private Sub MenuBirthResident(ByVal settlement As Settlement)
        Dim women As List(Of Person) = settlement.GetResidents("", "married female")
        If women.Count = 0 Then Console.WriteLine("Insufficient married couples!") : Console.ReadLine() : Exit Sub

        Dim men As New List(Of Person)
        For Each w In women
            men.Add(w.GetRelative("spouse"))
        Next
        Dim couples As New List(Of String)
        For n = 0 To women.Count - 1
            couples.Add(women(n).Name & " & " & men(n).Name)
        Next
        Dim s As Integer = Menu.getListChoiceIndex(couples, 1, "Select couple:")

        Dim father As Person = men(s)
        Dim mother As Person = women(s)
        Dim child As Person = Person.Birth(father, mother)
        Console.WriteLine(child.Name & " has been born to " & father.Name & " and " & mother.Name & ".")
        Console.ReadLine()
        Console.Clear()
    End Sub
    Private Sub MenuAddBuilding(ByVal settlement As Settlement)
        Dim choice As String = Menu.getListChoice(New List(Of String) From {"House", "Producer", "Projector"}, 1, "Select type of building:")
        Dim p As BuildingProject = Nothing
        Dim names As List(Of String)

        Select Case choice
            Case "House" : names = IO.ImportSquareBracketHeaders(IO.sbHouses)
            Case "Producer" : names = IO.ImportSquareBracketHeaders(IO.sbProducers)
            Case "Projector" : names =  IO.ImportSquareBracketHeaders(IO.sbProjectors)
            Case Else : Throw New Exception("Invalid type of building.")
        End Select

        Dim buildingName As String = Menu.getListChoice(names, 1, "Select building:")
        If buildingName = "" Then Exit Sub
        p = BuildingProject.Import(buildingName, choice)

        If p.LocationString <> "" Then
            Dim location As SettlementLocation = Menu.getListChoice(settlement.GetLocations(p.LocationString), 1, "Select location:")
            If location Is Nothing Then Console.WriteLine("Requires location: " & p.LocationString) : Console.ReadLine() : Exit Sub
            p.Location = location
            settlement.RemoveLocation(location)
        End If

        Dim b As Building = p.Unpack
        settlement.AddBuilding(b)
        Console.WriteLine(b.Name & " added.")
        Console.ReadLine()
    End Sub
    Private Sub MenuAddLocation(ByVal settlement As Settlement)
        Dim locationStrings As List(Of String) = IO.ImportSquareBracketHeaders(IO.sbNaturalResources)
        Dim locationString As String = Menu.getListChoice(locationStrings, 1, "Select location:")
        Dim location As SettlementLocation = NaturalResources.Construct(locationString)
        settlement.AddLocation(location)
        Console.WriteLine(locationString & " added.")
        Console.ReadLine()
    End Sub
End Module
