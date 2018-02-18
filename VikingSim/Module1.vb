Module Module1
    Public rng As New Random(3)

    Sub Main()
        Console.SetWindowSize(100, 43)
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
                Case 0, -1 : world.Tick()
                Case "Tick" : MenuTick(world)
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
        Dim site As SettlementSite = SettlementSite.Construct("Tundra")
        Dim settlement As Settlement = settlement.Construct(site, "Askton")

        Dim godfather As Person = Person.Ancestor("Male")
        Dim godmother As Person = Person.Ancestor("Female")
        Dim house As House = Nothing

        For n = 1 To 10
            Dim child As Person = Person.Birth(godfather, godmother)
            If n Mod 2 <> 0 Then
                house = house.Import("Hut")
                house.SetHistory("Odin", World.TimeNow)
                settlement.AddBuilding(house)
                house.AddFoodEaten("Bread", 1)
            End If
            child.MoveHouse(house)
        Next


        settlement.AddResources("Hardwood", 100)
        settlement.AddResources("Softwood", 100)
        settlement.AddResources("Bread", 100)

        Dim campfire = WorkplaceProjector.Import("Campfire")
        campfire.SetHistory("Odin", World.TimeNow)
        settlement.AddBuilding(campfire)
        campfire.AddWorkerBestAffinity()
        campfire.AddProjectCheck("Builder")
        campfire.AddProject("Builder")
        For n = 1 To 75
            campfire.Tick()
        Next

        Dim carpenter As WorkplaceProjector = AddProject(settlement, "Builder", "Carpenter")
        carpenter.AddWorkerBestAffinity()

        Return settlement
    End Function
    Private Function AddProject(ByVal settlement As Settlement, ByVal projectorName As String, ByVal projectName As String)
        projectorName = projectorName.Replace(" ", "+")
        Dim wp As WorkplaceProjector = settlement.GetBuildings("projector name=" & projectorName)(0)
        If wp Is Nothing Then Return Nothing

        If wp.GetBestWorker Is Nothing Then wp.AddWorkerBestAffinity()
        If wp.AddProjectCheck(projectName) = False Then Return Nothing
        wp.AddProject(projectName)
        For n = 1 To 75
            wp.Tick()
        Next
        Return settlement.GetBuildings("name=" & projectName)(0)
    End Function

    Private Sub MenuTick(ByVal world As World)
        Dim num As Integer = Menu.getNumInput(0, 1, 100, "Select number of ticks: ")
        For n = 1 To num
            World.Tick()
        Next
        world.AlertConsoleReport()
        Console.ReadLine()
    End Sub
    Private Sub MenuReviewSettlement(ByVal settlement As Settlement)
        settlement.ConsoleReport()
        Console.ReadLine()
    End Sub
    Private Sub MenuReviewBuildings(ByVal settlement As Settlement)
        Console.WriteLine()
        Dim choice As String = Menu.getListChoice(New List(Of String) From {"House", "Producer", "Projector"}, 1, "Select type of building:")
        If choice = "" Then Exit Sub
        Dim buildings As List(Of Building) = settlement.GetBuildings(choice)
        Console.WriteLine()
        Dim b As Building = Menu.getListChoice(buildings, 1, "Select building:")
        If b Is Nothing Then Exit Sub
        Console.WriteLine()
        b.ConsoleReport()
        Console.ReadLine()
    End Sub
    Private Sub MenuReviewResidents(ByVal settlement As Settlement)
        Dim selection As Person = Menu.getListChoice(Of Person)(settlement.GetResidents(""), 1, "Select resident:")
        If selection Is Nothing Then Exit Sub
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

        If p.Location <> "" Then
            Dim location As String = Menu.getListChoice(settlement.GetLocations(p.Location), 1, "Select location:")
            If location Is Nothing Then Console.WriteLine("Requires location: " & p.Location) : Console.ReadLine() : Exit Sub
            p.Location = location
        End If

        Dim projectors As List(Of Building) = settlement.GetBuildings("projector")
        For n = projectors.Count - 1 To 0 Step -1
            Dim proj As WorkplaceProjector = projectors(n)
            If proj.AddProjectCheck(p) = False Then projectors.RemoveAt(n)
        Next
        If projectors.Count = 0 Then Console.WriteLine("No projector can take on this project.") : Console.ReadLine() : Exit Sub

        Dim projector As WorkplaceProjector = Menu.getListChoice(projectors, 1, "Select projector:")
        projector.AddProject(p, p.Location)
        Console.WriteLine(buildingName & " project added to " & projector.Name & ".")
        Console.ReadLine()
    End Sub
    Private Sub MenuAddLocation(ByVal settlement As Settlement)
        Dim locationStrings As List(Of String) = IO.ImportSquareBracketHeaders(IO.sbTerrain)
        Dim locationString As String = Menu.getListChoice(locationStrings, 1, "Select location:")
        settlement.AddLocation(locationString)
        Console.WriteLine(locationString & " added.")
        Console.ReadLine()
    End Sub
End Module
