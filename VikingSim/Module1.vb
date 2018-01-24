Module Module1
    Sub Main()
        Dim settlement As New Settlement
        Dim house As New House
        settlement.AddBuilding(house)

        Dim godfather As Person = Person.Ancestor("Male", house)
        Dim godmother As Person = Person.Ancestor("Female", house)

        For n = 1 To 10
            Dim child As Person = Person.Birth(godfather, godmother)
            house.AddResident(child)
        Next

        Dim newHouse As New House
        settlement.AddBuilding(newHouse)
        Dim couple As List(Of Person) = settlement.GetSingleCouple
        couple(0).Marry(couple(1), newHouse)

        Dim newChild As Person = Person.Birth(newHouse.GetResidents("", "married male")(0), newHouse.GetResidents("", "married female")(0))
        newHouse.AddResident(newChild)

        Dim naturalResources As NaturalResources = naturalResources.Construct("Godbones")
        Dim workplace As Workplace = workplace.Import("Deepmines", naturalResources)
        settlement.AddBuilding(workplace)
        For n = 1 To 5
            Dim worker As Person = settlement.GetBestAffinityUnemployed(workplace.Occupation)
            worker.ChangeWorkplace(workplace)
        Next

        For n = 1 To 100
            workplace.Tick()
        Next
    End Sub
End Module
