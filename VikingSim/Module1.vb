Module Module1
    Sub Main()
        Dim settlement As Settlement = BuildSettlement()

        Dim bMenu As New Dictionary(Of Integer, String)
        bMenu.Add(1, "Marry Residents")
        bMenu.Add(2, "Birth Resident")
        bMenu.Add(3, "Add Building")
        bMenu.Add(4, "Add Natural Resource")

        Dim doExit As Boolean = False
        While doExit = False
            Select Case Menu.getListChoice(bMenu, 1)
                Case -1 : doExit = False
                Case 1 : MenuMarryResidents(settlement)
                Case 2 : MenuAddBuilding(settlement)
                Case 3 : MenuAddNaturalResource(settlement)
            End Select
        End While
    End Sub
    Private Function BuildSettlement() As Settlement
        Dim site As SettlementSite = SettlementSite.Construct("Wooded")
        Dim settlement As Settlement = settlement.Construct(site)
        Dim house As New House
        settlement.AddBuilding(house)

        Dim godfather As Person = Person.Ancestor("Male", house)
        Dim godmother As Person = Person.Ancestor("Female", house)

        For n = 1 To 10
            Dim child As Person = Person.Birth(godfather, godmother)
            house.AddResident(child)
        Next
        Return settlement
    End Function

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

    End Sub
    Private Sub MenuAddBuilding(ByVal settlement As Settlement)

    End Sub
    Private Sub MenuAddNaturalResource(ByVal settlement As Settlement)

    End Sub
End Module
