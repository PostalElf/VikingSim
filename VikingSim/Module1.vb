Module Module1
    Sub Main()
        Dim settlement As Settlement = BuildSettlement()

        Dim bMenu As New Dictionary(Of Integer, String)
        bMenu.Add(1, "Review Residents")
        bMenu.Add(11, "Marry Residents")
        bMenu.Add(12, "Birth Resident")
        bMenu.Add(13, "Add Building")
        bMenu.Add(14, "Add Natural Resource")

        Dim doExit As Boolean = False
        While doExit = False
            Select Case Menu.getListChoice(bMenu, 0, "Select option:")
                Case 0, -1 : doExit = True
                Case 1 : MenuReviewResidents(settlement)
                Case 11 : MenuMarryResidents(settlement)
                Case 12 : MenuBirthResident(settlement)
                Case 13 : MenuAddBuilding(settlement)
                Case 14 : MenuAddNaturalResource(settlement)
            End Select
        End While
    End Sub
    Private Function BuildSettlement() As Settlement
        Dim site As SettlementSite = SettlementSite.Construct("Wooded")
        Dim settlement As Settlement = settlement.Construct(site)

        Dim godfather As Person = Person.Ancestor("Male")
        Dim godmother As Person = Person.Ancestor("Female")
        Dim house As House = Nothing

        For n = 1 To 10
            Dim child As Person = Person.Birth(godfather, godmother)
            If n Mod 2 <> 0 Then
                house = house.Import("Hut")
                settlement.AddBuilding(house)
            End If
            child.MoveHouse(house)
        Next

        Dim nr As NaturalResources = NaturalResources.Construct("Godbones")
        Dim wp = WorkplaceProjector.Import("Carpenter")
        settlement.AddBuilding(wp)
        settlement.GetBestAffinityUnemployed(wp.Occupation).ChangeWorkplace(wp)
        wp.AddProject("Deepmines", nr)
        For n = 1 To 100
            wp.Tick()
        Next

        Return settlement
    End Function

    Private Sub MenuReviewResidents(ByVal settlement As Settlement)
        Dim selection As Person = Menu.getListChoice(Of Person)(settlement.GetResidents(""), 1, "Select resident:")
        selection.ConsoleReport()
        Console.ReadLine()
        Console.Clear()
    End Sub
    Private Sub MenuMarryResidents(ByVal settlement As Settlement)
        Dim men As List(Of Person) = settlement.GetResidents("", "single male")
        Dim women As List(Of Person) = settlement.GetResidents("", "single female")
        If men.Count = 0 OrElse women.Count = 0 Then Console.WriteLine("Insufficient singles!") : Console.ReadLine() : Exit Sub
        Dim houses As List(Of House) = settlement.GetEmptyHouses()

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
        Dim selection As Integer = Menu.getListChoice(couples, 1, "Select couple:")

        Dim father As Person = men(selection)
        Dim mother As Person = women(selection)
        Dim child As Person = Person.Birth(father, mother)
        Console.WriteLine(child.Name & " has been born to " & father.Name & " and " & mother.Name & ".")
        Console.ReadLine()
        Console.Clear()
    End Sub
    Private Sub MenuAddBuilding(ByVal settlement As Settlement)

    End Sub
    Private Sub MenuAddNaturalResource(ByVal settlement As Settlement)

    End Sub
End Module
