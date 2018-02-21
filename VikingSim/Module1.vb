Module Module1
    Public rng As New Random(3)

    Sub Main()
        Console.SetWindowSize(100, 43)
        Dim world As World = world.Construct
        Dim settlement As Settlement = BuildSettlement(world)

        Dim bMenu As New List(Of String)
        bMenu.Add("Tick")
        bMenu.Add("Review Settlement")

        While True
            Console.Clear()
            world.consoleReport()
            Select Case Menu.getListChoice(bMenu, 0, "Select option:")
                Case "Tick" : MenuTick(world)
                Case "Review Settlement" : MenuReviewSettlement(world)
                Case Else : Exit While
            End Select
        End While
    End Sub
    Private Function BuildSettlement(ByVal world As World) As Settlement
        Dim settlement As Settlement = world.GetSettlements("")(0)

        Settlement.AddResources("Hardwood", 100)
        Settlement.AddResources("Softwood", 100)
        Settlement.AddResources("Bread", 100)

        Dim campfire = WorkplaceProjector.Import("Campfire")
        campfire.SetHistory("Odin", World.TimeNow)
        Settlement.AddBuilding(campfire)
        settlement.GetResidentBest("employable", "affinity=" & campfire.Occupation.ToString).ChangeWorkplace(campfire)
        campfire.AddProjectCheck("Builder")
        campfire.AddProject("Builder")
        For n = 1 To 75
            campfire.Tick()
        Next

        Dim carpenter As WorkplaceProjector = AddProject(Settlement, "Builder", "Carpenter")
        settlement.GetResidentBest("employable", "affinity=" & carpenter.Occupation.ToString).ChangeWorkplace(carpenter)
        AddProject(Settlement, "Builder", "Cottage")

        Return Settlement
    End Function
    Private Function AddProject(ByVal settlement As Settlement, ByVal projectorName As String, ByVal projectName As String)
        projectorName = projectorName.Replace(" ", "+")
        Dim wp As WorkplaceProjector = settlement.GetBuildings("projector name=" & projectorName)(0)
        If wp Is Nothing Then Return Nothing

        If wp.GetBestWorker Is Nothing Then settlement.GetResidentBest("employable", "skill=" & wp.Occupation.ToString).ChangeWorkplace(wp)
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
            Dim warnings As List(Of Alert) = world.GetTickWarnings
            If warnings.Count > 0 Then
                Dim trueWarnings As New List(Of Alert)
                For Each w In warnings
                    If w.Priority >= 3 Then trueWarnings.Add(w)
                Next
                If trueWarnings.Count > 0 Then
                    Console.WriteLine()
                    Console.WriteLine("Tick " & n)
                    world.AlertConsoleReport(trueWarnings)
                    Console.ReadLine()
                    Exit Sub
                End If
            End If

            world.Tick()
        Next
        world.AlertConsoleReport()
        Console.ReadLine()
    End Sub
    Private Sub MenuReviewSettlement(ByVal world As World)
        Dim settlements As List(Of Settlement) = world.GetSettlements("")
        Dim settlement As Settlement = Menu.getListChoice(Of Settlement)(settlements, 1, "Select a settlement:")

        Dim bMenu As New List(Of String)
        With bMenu
            .Add("Review Building")
            .Add("Review Residents")
            .Add("Birth Child")
            .Add("Add Building")
            .Add("Add Location")
        End With

        While True
            Console.Clear()
            settlement.ConsoleReport()
            Console.WriteLine()
            Select Case Menu.getListChoice(bMenu, 0, "Select option:")
                Case "Review Building" : MenuReviewBuildings(settlement)
                Case "Review Residents" : MenuReviewResidents(settlement)
                Case "Birth Child" : MenuBirthResident(settlement)
                Case "Add Building" : MenuAddBuilding(settlement)
                Case "Add Location" : MenuAddBuilding(settlement)
                Case Else : Exit While
            End Select
        End While
    End Sub

    Private Sub MenuReviewBuildings(ByVal settlement As Settlement)
        Console.WriteLine()
        Dim choice As String = Menu.getListChoice(New List(Of String) From {"House", "Producer", "Projector"}, 1, "Select type of building:")
        If choice = "" Then Exit Sub
        Dim buildings As List(Of Building) = settlement.GetBuildings(choice)
        Console.WriteLine()
        Dim b As Building = Menu.getListChoice(buildings, 1, "Select building:")
        If b Is Nothing Then Exit Sub

        Select Case choice
            Case "House" : MenuReviewHouse(b)
            Case "Producer" : MenuReviewWorkplace(b)
            Case "Projector" : MenuReviewWorkplace(b)
        End Select
    End Sub
    Private Sub MenuReviewHouse(ByVal house As House)
        Dim bMenu As New List(Of String)
        With bMenu
            .Add("Set Food")
            .Add("Birth")
        End With

        While True
            Console.Clear()
            house.ConsoleReport()
            Console.WriteLine()

            Select Case Menu.getListChoice(bMenu, 0, "Select option:")
                Case "Set Food" : MenuSetFood(house)
                Case "Birth" : MenuBirthResident(house)
                Case Else : Exit While
            End Select
        End While
    End Sub
    Private Sub MenuReviewWorkplace(ByVal workplace As Workplace)
        Dim bMenu As New List(Of String)
        With bMenu
            .Add("Apprentice")
            .Add("Employ")
            .Add("Fill Employment (Affinity)")
            .Add("Fill Employment (Skill)")

            If TypeOf workplace Is WorkplaceProjector Then
                .Add("Add Project")
            End If
        End With

        While True
            With workplace
                Console.Clear()
                .ConsoleReport()
                Console.WriteLine()

                Select Case Menu.getListChoice(bMenu, 0, "Select option:")
                    Case "Apprentice"
                        If Menu.confirmChoice(0, "Add apprentice with best affinity? ") = True Then

                        Else
                            MenuApprenticeResident(workplace)
                        End If

                    Case "Employ"
                        MenuEmployResident(workplace)

                    Case "Fill Employment (Affinity)"
                        While .AddWorkerCheck(Nothing) = False
                            Dim worker As Person = .Settlement.GetResidentBest("employable", "affinity=" & .Occupation.ToString)
                            If worker Is Nothing Then Exit While
                            If .AddWorkerCheck(worker) = False Then Exit While
                            worker.ChangeWorkplace(workplace)
                        End While

                    Case "Fill Employment (Skill)"
                        While .AddWorkerCheck(Nothing) = False
                            Dim worker As Person = .Settlement.GetResidentBest("employable", "skill=" & .Occupation.ToString)
                            If worker Is Nothing Then Exit While
                            If .AddWorkerCheck(worker) = False Then Exit While
                            worker.ChangeWorkplace(workplace)
                        End While

                    Case "Add Project"

                    Case Else : Exit While
                End Select
            End With
        End While
    End Sub

    Private Sub MenuReviewResidents(ByVal settlement As Settlement)
        Console.Write("Enter flags: ")
        Dim flags As String = Console.ReadLine

        Dim possibleSelections As List(Of Person) = settlement.GetResidents(flags)
        Dim selection As Person = Menu.getListChoice(Of Person)(possibleSelections, 0, "Select resident:")
        If selection Is Nothing Then Exit Sub

        Dim bMenu As New List(Of String)
        With bMenu
            .Add("Marry")
            .Add("Pregnancy")
            .Add("Birth")
            .Add("Move")
            .Add("Apprentice")
            .Add("Employ")
        End With

        While True
            Console.Clear()
            selection.ConsoleReport()
            Console.WriteLine()
            Select Case Menu.getListChoice(bMenu, 0, "Select option:")
                Case "Marry" : MenuMarryResidents(settlement, selection)
                Case "Pregnancy" : menuPregnancyResident(settlement, selection)
                Case "Birth" : MenuBirthResident(selection)
                Case "Move" : MenuMoveResident(settlement, selection)
                Case "Apprentice" : menuApprenticeResident(settlement, selection)
                Case "Employ" : menuEmployResident(settlement, selection)
                Case Else : Exit While
            End Select
        End While
    End Sub
    Private Sub MenuMarryResidents(ByVal settlement As Settlement)
        Dim singleMales As List(Of Person) = settlement.GetResidents("single male")
        Dim singlePerson As Person = Menu.getListChoice(singleMales, 0, "Select husband:")
        MenuMarryResidents(settlement, singlePerson)
    End Sub
    Private Sub MenuMarryResidents(ByVal settlement As Settlement, ByVal singlePerson As Person)
        Dim houses As List(Of Building) = settlement.GetBuildings("house space")
        If houses.Count = 0 Then Console.WriteLine("No free house!") : Console.ReadLine() : Exit Sub

        Dim spouseSex As String
        If singlePerson.Sex = "Male" Then spouseSex = "female" Else spouseSex = "male"
        Dim spouses As List(Of Person) = settlement.GetResidents("single " & spouseSex)
        If spouses.Count = 0 Then Console.WriteLine("No available " & spouseSex & "s!") : Console.ReadLine() : Exit Sub
        Dim spouse As Person = Menu.getListChoice(spouses, 0, "Select spouse:")
        Console.WriteLine()
        Dim house As House = Menu.getListChoice(houses, 1, "Select house:")

        MenuMarryResidents(singlePerson, spouse, house)
    End Sub
    Private Sub MenuMarryResidents(ByVal male As Person, ByVal female As Person, ByVal house As House)
        male.Marry(female, house)
        Console.WriteLine(male.Name & " has married " & female.Name & ".")
        Console.ReadLine()
        Console.Clear()
    End Sub
    Private Sub MenuPregnancyResident(ByVal settlement As Settlement, ByVal mother As Person)
        If mother.Sex <> "Female" Then Console.WriteLine("Only women can give birth!") : Console.ReadLine() : Exit Sub
        If mother.CheckFlags("married") = False Then Console.WriteLine("Only married women can give birth!") : Console.ReadLine() : Exit Sub

        Dim weeks As Integer = Menu.getNumInput(0, 0, 32, "How many weeks pregnant? ")
        mother.BecomePregnant(weeks)
    End Sub
    Private Sub MenuBirthResident(ByVal settlement As Settlement)
        Dim women As List(Of Person) = settlement.GetResidents("married female")
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
        MenuBirthResident(mother, father)
    End Sub
    Private Sub MenuBirthResident(ByVal house As House)
        Dim mothers As List(Of Person) = house.GetResidents("married women")
        If mothers.count = 0 Then
            Console.WriteLine("No married women in this household!")
            Console.ReadLine()
            Exit Sub
        End If
        Dim mother As Person = mothers(0)
        MenuBirthResident(mother)
    End Sub
    Private Sub MenuBirthResident(ByVal mother As Person, Optional ByVal father As Person = Nothing)
        If mother.Sex <> "Female" Then Console.WriteLine("Only women can give birth!") : Console.ReadLine() : Exit Sub
        If mother.CheckFlags("married") = False Then Console.WriteLine("Only married women can give birth!") : Console.ReadLine() : Exit Sub
        If father Is Nothing Then father = mother.GetRelative("spouse")

        mother.BecomePregnant(32)
        Dim children As List(Of Person) = mother.GetRelatives("children")
        Dim child As Person = children(children.Count - 1)
        Console.WriteLine(child.Name & " has been born to " & father.Name & " and " & mother.Name & ".")
        Console.ReadLine()
        Console.Clear()
    End Sub
    Private Sub MenuMoveResident(ByVal settlement As Settlement, ByVal person As Person)
        Dim houses As List(Of Building) = settlement.GetBuildings("house space")
        Dim house As House = Menu.getListChoice(houses, 0, "Select house to move to: ")

        person.MoveHouse(house)
        Console.WriteLine(person.Name & " has moved to " & house.Name & ".")
        Dim spouse As Person = person.GetRelative("spouse")
        If spouse Is Nothing = False Then spouse.MoveHouse(house) : Console.WriteLine(spouse.Name & " has moved to " & house.Name & ".")
        Console.ReadLine()
    End Sub
    Private Sub MenuApprenticeResident(ByVal settlement As Settlement, ByVal person As Person)
        Dim workplaces As List(Of Building) = settlement.GetBuildings("workplace employable")
        Dim workplace As Workplace = Menu.getListChoice(workplaces, 0, "Select workplace:")
        MenuApprenticeResident(workplace, person)
    End Sub
    Private Sub MenuApprenticeResident(ByVal workplace As Workplace, Optional ByVal apprentice As Person = Nothing)
        With workplace
            If apprentice Is Nothing Then
                Dim apprentices As List(Of Person) = .Settlement.GetResidents("apprenticable")
                If apprentices.Count = 0 Then Console.WriteLine("No available apprentices!") : Console.ReadLine() : Exit Sub
                apprentice = Menu.getListChoice(apprentices, 0, "Select an apprentice:")
            End If
            If .AddApprenticeCheck(apprentice) = False Then Console.WriteLine(apprentice.Name & " cannot join " & .Name & "!") : Console.ReadLine() : Exit Sub

            apprentice.ChangeApprenticeship(workplace)
            Console.WriteLine(apprentice.Name & " has joined " & workplace.Name & " as an apprentice.")
            Console.ReadLine()
        End With
    End Sub
    Private Sub MenuEmployResident(ByVal settlement As Settlement, ByVal person As Person)
        Dim workplaces As List(Of Building) = settlement.GetBuildings("workplace employable")
        Dim workplace As Workplace = Menu.getListChoice(workplaces, 0, "Select workplace: ")
        MenuEmployResident(workplace, person)
    End Sub
    Private Sub MenuEmployResident(ByVal workplace As Workplace, Optional ByVal person As Person = Nothing)
        With workplace
            If person Is Nothing Then
                Dim workers As List(Of Person) = .Settlement.GetResidents("employable")
                If workers.Count = 0 Then Console.WriteLine("No available workers!") : Console.ReadLine() : Exit Sub
                person = Menu.getListChoice(workers, 0, "Select a worker:")
            End If
            If .AddWorkerCheck(person) = False Then Console.WriteLine(person.Name & " cannot join " & .Name & "!") : Console.ReadLine() : Exit Sub

            person.ChangeWorkplace(workplace)
            Console.WriteLine(person.Name & " has joined " & .Name & ".")
            Console.ReadLine()
        End With
    End Sub
    Private Sub MenuSetFood(ByVal settlement As Settlement)
        Console.WriteLine()
        Dim choice As House = Menu.getListChoice(settlement.GetBuildings("House"), 1, "Select house:")
        Console.WriteLine()
        choice.ConsoleReport()

        MenuSetFood(choice)
    End Sub
    Private Sub MenuSetFood(ByVal house As House)
        While True
            Console.Write("Enter food: ")
            Dim foodString As String = Console.ReadLine
            If ResourceDict.GetCategory(foodString) <> "Food" Then Console.WriteLine("Invalid type of food!") : Continue While
            Dim qty As Integer = Menu.getNumInput(0, 1, 100, "How much? ")

            house.AddFoodEaten(foodString, qty)
            Exit While
        End While
    End Sub
    Private Sub MenuAddBuilding(ByVal settlement As Settlement)
        Dim choice As String = Menu.getListChoice(New List(Of String) From {"House", "Producer", "Projector"}, 1, "Select type of building:")
        Dim p As BuildingProject = Nothing
        Dim names As List(Of String)

        Select Case choice
            Case "House" : names = IO.ImportSquareBracketHeaders(IO.sbHouses)
            Case "Producer" : names = IO.ImportSquareBracketHeaders(IO.sbProducers)
            Case "Projector" : names = IO.ImportSquareBracketHeaders(IO.sbProjectors)
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
