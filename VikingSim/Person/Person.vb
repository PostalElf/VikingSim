Public Class Person
    Implements iModifiable

#Region "Personal Identifiers"
    Private NameFirst As String
    Private NameLast As String
    Private Epithet As String
    Public ReadOnly Property Name As String Implements iModifiable.Name
        Get
            If NameLast = "" Then
                Return NameFirst
            Else
                If Epithet = "" Then
                    Return NameFirst & " " & NameLast
                Else
                    Return NameFirst & " '" & Epithet & "' " & NameLast
                End If
            End If
        End Get
    End Property
    Public ReadOnly Property NameAndTitle As String
        Get
            Return Name & " - " & OccupationName
        End Get
    End Property

    Public Overrides Function ToString() As String
        Return NameAndTitle
    End Function

    Public Sex As Sex

    Public Inventory As New Inventory
    Public Function GetInventoryBonus(ByVal occ As String) As Integer
        Return Inventory.GetBonus(occ)
    End Function

    Private House As House
    Public Sub MoveHouse(ByVal targetHouse As House)
        If House Is Nothing = False Then House.RemoveResident(Me)
        House = targetHouse
        House.AddResident(Me)
    End Sub
    Public Sub ReturnHome()
        If House.GetResidents("name=" & Name.Replace(" ", "+")).Count > 0 Then Exit Sub
        House.AddResident(Me)
    End Sub
    Private FatherName As String
    Private MotherName As String
    Private ChildrenNames As New List(Of String)
    Public Function CheckFlags(ByVal flags As String) As Boolean
        Dim fs As String() = flags.Split(" ")
        For Each f In fs
            If f = "" Then Continue For
            Select Case f.ToLower
                Case "single", "unmarried" : If Age >= AgeMarriage AndAlso SpouseName <> "" Then Return False
                Case "married" : If SpouseName = "" Then Return False
                Case "male", "men" : If Sex <> "Male" Then Return False
                Case "female", "women" : If Sex <> "Female" Then Return False
                Case "employed" : If Occupation = Skill.Vagrant Then Return False
                Case "apprenticable" : If Age < AgeApprentice Then Return False
                Case "employable" : If Age >= AgeLabour AndAlso Occupation <> Skill.Vagrant Then Return False
                Case Else : If CheckFlagsAdvanced(f.ToLower) = False Then Return False
            End Select
        Next
        Return True
    End Function
    Private Function CheckFlagsAdvanced(ByVal flag As String) As Boolean
        'unrecognised advanced flag, let it go
        If flag.Contains("=") = False Then Return True

        'advanced flags all contain = with string split up by +
        Dim ns As String() = flag.Split("=")
        Dim header As String = ns(0)
        Dim entry As String = ns(1)

        Select Case header
            Case "occupation" : If OccupationName.ToLower = entry Then Return True
            Case "name" : If (NameFirst & "+" & NameLast).ToLower = entry Then Return True
        End Select

        Return False
    End Function
    Public Function GetRelative(ByVal relationship As String) As Person
        Select Case relationship.ToLower
            Case "mother" : Return House.GetResident(MotherName)
            Case "father" : Return House.GetResident(FatherName)
            Case "spouse" : Return House.GetResident(SpouseName)
            Case Else : Return Nothing
        End Select
    End Function
    Public Function GetRelatives(ByVal relationship As String) As List(Of Person)
        Dim total As New List(Of Person)
        Select Case relationship.ToLower
            Case "children"
                For Each childName In ChildrenNames
                    total.Add(House.GetResident(childName))
                Next
            Case "spouse", "couple"
                total.Add(Me)
                total.Add(House.GetResident(SpouseName))
        End Select
        Return total
    End Function

    Private SpouseName As String
    Public Sub Marry(ByVal spouse As Person, ByVal newHouse As House)
        If spouse.Sex = Sex Then Exit Sub

        SpouseName = spouse.Name
        MoveHouse(newHouse)

        spouse.SpouseName = Name
        spouse.MoveHouse(newHouse)
    End Sub

    Public Sub ConsoleReport()
        Console.WriteLine(Name & " - " & Age & Sex.First.ToLower)
        Console.WriteLine("└ House:      " & House.Name)
        Console.WriteLine("└ Father:     " & FatherName)
        Console.WriteLine("└ Mother:     " & MotherName)
        If SpouseName <> "" Then
            Console.WriteLine("└ Spouse:     " & SpouseName)
            Console.WriteLine("└ Children:   " & ChildrenNames.Count)
        End If
        Console.WriteLine("└ Occupation: " & Occupation.ToString)
    End Sub
#End Region

#Region "Constructors"
    Public Sub New()
        For Each s As Skill In [Enum].GetValues(GetType(Skill))
            If s = Skill.Vagrant Then Continue For
            SkillRanks.Add(s, 0)
            SkillAffinities.Add(s, 0)
            SkillXP.Add(s, 0)
        Next

        For Each wt In [Enum].GetValues(GetType(WeaponTypes))
            WeaponSkillRanks.Add(wt, 0)
            WeaponSkillXP.Add(wt, 0)
        Next
    End Sub

    Public Shared Function Birth(ByVal father As Person, ByVal mother As Person, Optional ByVal setAge As Integer = 1) As Person
        Dim child As New Person
        With child
            If rng.Next(1, 3) = 1 Then .Sex = New Sex("Male") Else .Sex = New Sex("Female")
            .BirthDate = New CalendarDate(World.TimeNow)
            .Age = setAge
            .NameFirst = GrabRandomNameFirst(.Sex)
            .NameLast = GrabRandomNameLast(.Sex, father, mother)
            .FatherName = father.Name
            .MotherName = mother.Name
            father.ChildrenNames.Add(.Name)
            mother.ChildrenNames.Add(.Name)

            For Each s In [Enum].GetValues(GetType(Skill))
                'child's affinity varies by +/- 0.4
                'xp gain always rounded down from affinity, eg. 4.6 will grant 4 XP
                Dim modifier As Double = FudgeRoll() / 10

                '50/50 chance to inherit mother or father's affinity
                Dim ref As Person
                If rng.Next(1, 3) = 1 Then ref = mother Else ref = father

                'affinity capped at 0.1 to 5.0
                Dim newAffinity As Double = ref.SkillAffinities(s) + modifier
                If newAffinity <= 0 Then newAffinity = 0.1
                If newAffinity >= 5 Then newAffinity = 5
                .SkillAffinities(s) = newAffinity
            Next
        End With
        Return child
    End Function
    Public Shared Function Ancestor(ByVal childSex As String) As Person
        Dim child As New Person
        With child
            If childSex = "Male" Then
                .NameFirst = "Ask"
            ElseIf childSex = "Female" Then
                .NameFirst = "Embla"
            End If

            For Each s In [Enum].GetValues(GetType(Skill))
                .SkillAffinities(s) = 2
            Next
        End With
        Return child
    End Function

    Private Shared MaleFirstNames As New List(Of String)
    Private Shared GirlFirstNames As New List(Of String)
    Private Shared Function GrabRandomNameFirst(ByVal childSex As Sex) As String
        If childSex = "Male" Then
            If MaleFirstNames.Count = 0 Then MaleFirstNames = IO.ImportTextList(IO.tlMaleNames)
            Return GrabRandom(Of String)(MaleFirstNames)
        ElseIf childSex = "Female" Then
            If GirlFirstNames.Count = 0 Then GirlFirstNames = IO.ImportTextList(IO.tlGirlNames)
            Return GrabRandom(Of String)(GirlFirstNames)
        Else
            Throw New Exception("Sex neither male nor female")
        End If
    End Function
    Private Shared Function GrabRandomNameLast(ByVal childSex As Sex, ByVal father As Person, ByVal mother As Person)
        If childSex = "Male" Then
            Return father.NameFirst & "sson"
        ElseIf childSex = "Female" Then
            Return mother.NameFirst & "dottir"
        Else
            Throw New Exception("Sex neither male nor female")
        End If
    End Function
#End Region

#Region "Save/Load"

#End Region

#Region "Skills"
    Public ReadOnly Property Occupation As Skill
        Get
            If Workplace Is Nothing Then Return Skill.Vagrant
            Return Workplace.Occupation
        End Get
    End Property
    Private ReadOnly Property OccupationName As String
        Get
            Select Case Occupation
                Case Skill.Vagrant : Return "Vagrant"
                Case Skill.Fighting : Return "Fighter"

                Case Skill.Hunting : Return "Hunter"
                Case Skill.Scouting : Return "Scout"
                Case Skill.Gathering : Return "Gatherer"

                Case Skill.Mining : Return "Miner"
                Case Skill.Quarrying : Return "Quarrier"
                Case Skill.Logging : Return "Logger"

                Case Skill.Farming : Return "Farmer"
                Case Skill.Storyteling : Return "Storyteller"
                Case Skill.Religion : Return "Priest"
                Case Skill.Trading : Return "Trader"

                Case Skill.Smithing : Return "Smith"
                Case Skill.Sculpting : Return "Sculpter"
                Case Skill.Carpentry : Return "Carpenter"

                Case Else : Throw New Exception("Invalid Occupation")
            End Select
        End Get
    End Property
    Private Workplace As Workplace
    Public Sub ChangeWorkplace(ByVal newWorkplace As Workplace)
        If newWorkplace.Settlement.Equals(House.Settlement) = False Then Exit Sub
        If newWorkplace.AddWorkerCheck(Me) = False Then Exit Sub

        If Workplace Is Nothing = False Then Workplace.RemoveWorker(Me)
        newWorkplace.AddWorker(Me)
        Workplace = newWorkplace
    End Sub
    Public Sub ChangeApprenticeship(ByVal newWorkplace As Workplace)
        If Workplace Is Nothing = False Then Workplace.RemoveApprentice(Me)
        newWorkplace.AddApprentice(Me)
        Workplace = newWorkplace
    End Sub
    Public Function PerformWork() As Integer
        'return skill rank
        PerformWork = SkillRanks(Occupation)

        'standard skill XP gain
        SkillXPGain(Occupation)

        'special weapon skill improvement from select professions
        Select Case Occupation
            Case Skill.Logging : WeaponSkillXPGain(WeaponTypes.Waraxe)
        End Select
    End Function

    Private SkillAffinities As New Dictionary(Of Skill, Double)
    Public ReadOnly Property SkillAffinity(ByVal skill As Skill) As Double
        Get
            Return SkillAffinities(skill)
        End Get
    End Property
    Private ReadOnly Property SkillAffinityDescription(ByVal skill As Skill) As String
        Get
            Select Case SkillAffinities(skill)
                Case Is <= 1 : Return "Slow"
                Case Is <= 2 : Return "Dim"
                Case Is <= 3 : Return "Average"
                Case Is <= 4 : Return "Quick"
                Case Is <= 5 : Return "Talented"
                Case Else : Throw New Exception("SkillAffinity out of bounds")
            End Select
        End Get
    End Property
    Private Shared Function XPThreshold(ByVal skillRank As Integer) As Double
        Select Case skillRank
            Case 0 : Return 100
            Case 1 : Return 300
            Case 2 : Return 600
            Case 3 : Return 1000
            Case Else : Throw New Exception("SkillRank out of bounds")
        End Select
    End Function

    Private SkillRanks As New Dictionary(Of Skill, Integer)
    Public ReadOnly Property SkillRank(ByVal skill As Skill) As Integer
        Get
            Return SkillRanks(skill)
        End Get
    End Property
    Private ReadOnly Property SkillRankDescription(ByVal skill As Skill) As String
        Get
            Select Case SkillRanks(skill)
                Case 0 : Return "Untrained"
                Case 1 : Return "Apprentice"
                Case 2 : Return "Journeyman"
                Case 3 : Return "Master"
                Case Else : Throw New Exception("SkillRank out of bounds")
            End Select
        End Get
    End Property
    Private SkillXP As New Dictionary(Of Skill, Integer)
    Private Function SkillXPGain(ByVal skill As Skill, Optional ByVal multiplier As Double = 1) As Boolean
        'increase XP
        Dim xp As Integer = Math.Floor(SkillAffinities(skill) * multiplier)
        If xp <= 0 Then xp = 1
        SkillXP(skill) += xp

        'return true if levelled up
        Dim currentSkillRank As Integer = SkillRanks(skill)
        If SkillXP(skill) >= XPThreshold(currentSkillRank) Then
            SkillRanks(skill) += 1
            Return True
        End If
        Return False
    End Function

    Private WeaponSkillRanks As New Dictionary(Of WeaponTypes, Double)
    Public ReadOnly Property WeaponRankDescription(ByVal wt As WeaponTypes) As String
        Get
            Dim rank As Integer = Math.Floor(WeaponSkillRanks(wt))
            Select Case rank
                Case 0 : Return "Untrained"
                Case 1 : Return "Apprentice"
                Case 2 : Return "Journeyman"
                Case 3 : Return "Master"
                Case Else : Throw New Exception("WeaponSkillRank out of bounds")
            End Select
        End Get
    End Property
    Private WeaponSkillXP As New Dictionary(Of WeaponTypes, Integer)
    Private Function WeaponSkillXPGain(ByVal weaponType As WeaponTypes, Optional ByVal multiplier As Double = 1) As Boolean
        'return true if levelled up

        Dim xp As Double = 0.25 * multiplier
        WeaponSkillXP(weaponType) += xp
        Dim currentSkillRank As Integer = WeaponSkillRanks(weaponType)
        If WeaponSkillXP(weaponType) >= XPThreshold(currentSkillRank) Then
            WeaponSkillRanks(weaponType) += 1
            Return True
        End If
        Return False
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

#Region "Pregnancy"
    Private Pregnancy As Pregnancy
    Private FertilityWeek As Integer = 1
    Private ReadOnly Property HasPeriod As Boolean
        Get
            If FertilityWeek = 1 Then Return True Else Return False
        End Get
    End Property
    Private ReadOnly Property PregnancyChance As Integer
        Get
            If Age < AgeMarriage Then Return 0
            If Age > AgeMenopause Then Return 0
            If House.AddResidentCheck(Nothing) = False Then Return 0

            Dim total As Integer = 0
            Select Case Age
                Case 16 To 20 : total = 20
                Case 21 To 24 : total = 15
                Case 25 To 29 : total = 12
                Case 30 To 34 : total = 10
                Case 35 To 40 : total = 5
                Case 41 To 45 : total = 3
                Case 46 To 49 : total = 1
                Case Else : total = 0
            End Select

            Return total
        End Get
    End Property

    Private Sub FemaleTick()
        If Pregnancy Is Nothing = False Then
            Dim child As Person = Pregnancy.Tick()
            If child Is Nothing = False Then GiveBirth(child)
        Else
            'not pregnant; check if married and husband is around
            If GetRelative("spouse") Is Nothing = False Then
                FertilityWeek += 1
                If FertilityWeek >= 4 Then
                    'chance for pregnancy
                    If rng.Next(1, 101) <= PregnancyChance Then BecomePregnant() Else FertilityWeek = 1
                End If
            End If
        End If
    End Sub
    Private Function GetFemaleTickWarnings() As List(Of Alert)
        Dim total As New List(Of Alert)
        If Pregnancy Is Nothing = False Then
            total.AddRange(Pregnancy.GetTickWarnings)
        End If
        Return total
    End Function
    Public Sub BecomePregnant(Optional ByVal weeks As Integer = 0)
        Pregnancy = New Pregnancy(House.GetResident(SpouseName), Me)
        World.AddAlert(Me, 2, Name & " has become pregnant!")
        For n = 1 To weeks
            Dim child As Person = Pregnancy.Tick
            If child Is Nothing = False Then GiveBirth(child)
        Next
        FertilityWeek = 0
    End Sub
    Private Sub GiveBirth(ByVal child As Person)
        If child Is Nothing Then Exit Sub

        World.AddAlert(child, 2, child.Name & " has been born to " & child.MotherName & " & " & child.FatherName & ".")
        If House.AddResidentcheck(child) = False Then
            'if there's no space left in this house, everyone moves to a hut
            Dim father As Person = GetRelative("spouse")
            Dim settlement As Settlement = House.Settlement
            Dim hut As House = House.Import("Hut")

            settlement.AddBuilding(hut)
            father.MoveHouse(hut)
            Me.MoveHouse(hut)
        End If
        child.MoveHouse(House)

        Pregnancy = Nothing
        FertilityWeek = 0
    End Sub
#End Region

#Region "Aging and Disease"
    Private BirthDate As CalendarDate
    Private Age As Integer = 1
    Private Const AgeMarriage As Integer = 16
    Private Const AgeLabour As Integer = 12
    Private Const AgeApprentice As Integer = 6
    Private Const AgeMenopause As Integer = 50

    Private ReadOnly Property DiseaseChance As Integer
        Get
            Const dc As String = "DiseaseChance"
            Dim total As Integer = House.Settlement.GetModifier(dc)
            total += House.GetModifier(dc)
            If Workplace Is Nothing = False Then total += Workplace.GetModifier(dc)

            Select Case Age
                Case 1 To 2 : total += 10
                Case 3 To 16 : total -= 10
                Case 17 To 35 : total += 0
                Case 36 To 50 : total += 5
                Case 50 To 55 : total += 10
                Case Else : total += (Age - 50) * 2
            End Select

            Return total
        End Get
    End Property
    Private Function GetRandomDisease() As Modifier
        Dim allDiseaseList As New List(Of String)
        With allDiseaseList
            .AddRange({"Ague", "Flux", "Plague", "Pox"})          'pollinate with standard diseases

            'occupation-specific diseases
            Select Case Occupation
                Case Skill.Mining : .Add("Blacklung")
            End Select
        End With

        Dim roll As Integer = rng.Next(allDiseaseList.Count)
        Dim disease As Modifier = Modifier.Import(allDiseaseList(roll))
        Return disease
    End Function
    Private Sub AgeTick()
        If World.TimeNow.Month = BirthDate.Month AndAlso World.TimeNow.Week = BirthDate.Week Then
            Age += 1
            Select Case Age
                Case AgeApprentice : World.AddAlert(Me, 1, Name & " is now " & Age & " and may now become an apprentice.")
                Case AgeLabour : World.AddAlert(Me, 1, Name & " is now " & Age & " and may now work.")
                Case AgeMarriage : World.AddAlert(Me, 1, Name & " is now " & Age & " and may now marry.")
                Case Else : World.AddAlert(Me, 1, Name & " is now " & Age & ".")
            End Select
        End If

        'check deathchance for current diseases
        Dim deathChance As Integer = GetModifier("DeathChance")
        If deathChance > 0 Then
            If rng.Next(1, 101) <= deathChance Then
                Die()
                World.AddAlert(House, 3, Name & " has died in " & Sex.PronounPossessive.ToLower & " sleep.")
                Exit Sub
            End If
        End If

        'check chance for new disease
        If rng.Next(1, 101) <= DiseaseChance Then
            Dim disease As Modifier = GetRandomDisease()
            AddModifier(disease)
            World.AddAlert(Me, 2, Name & " has caught the " & disease.Title & ".")
        End If
    End Sub
    Private Sub Die()
        Dim settlement As Settlement = House.Settlement
        Dim convoy As Convoy = settlement.getConvoy(Me)
        If convoy Is Nothing = False Then
            convoy.removePerson(Me)
        End If

        House.RemoveResident(Me)
        Workplace.RemoveApprentice(Me)
        Workplace.RemoveWorker(Me)
    End Sub
#End Region

    Public Sub Tick()
        TickModifier()
        AgeTick()
        If Sex = "Female" Then FemaleTick()
    End Sub
    Public Function GetTickWarnings() As List(Of Alert)
        Dim total As New List(Of Alert)

        If Sex = "Female" Then total.AddRange(GetFemaleTickWarnings)

        Return total
    End Function
End Class
