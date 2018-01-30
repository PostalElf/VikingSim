Public Class Person

#Region "Personal Identifiers"
    Private NameFirst As String
    Private NameLast As String
    Private Epithet As String
    Public ReadOnly Property Name As String
        Get
            If Epithet = "" Then
                Return NameFirst & " " & NameLast
            Else
                Return NameFirst & " '" & Epithet & "' " & NameLast
            End If
        End Get
    End Property
    Public ReadOnly Property NameAndTitle As String
        Get
            Return Name & " (" & OccupationName & ")"
        End Get
    End Property

    Public Overrides Function ToString() As String
        Return Name
    End Function

    Private _Sex As String
    Public ReadOnly Property Sex As String
        Get
            Return _Sex
        End Get
    End Property
    Private Pregnancy As pregnancy
    Private BirthDate As CalendarDate
    Private ReadOnly Property Age As Integer
        Get
            Return (BirthDate - World.TimeNow).Year
        End Get
    End Property
    Public Sub Tick()
        If Sex = "Female" Then
            If Pregnancy Is Nothing = False Then
                Dim child As Person = Pregnancy.Tick()
                If child Is Nothing = False Then child.MoveHouse(House) : Pregnancy = Nothing
            ElseIf SpouseName <> "" Then
                'TODO: add sexy times
            End If
        End If

        'TODO: add death related stuff
    End Sub

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
    Private FatherName As String
    Private MotherName As String
    Private ChildrenNames As New List(Of String)
    Public Function CheckFlags(ByVal flags As String) As Boolean
        Dim fs As String() = flags.Split(" ")
        For Each f In fs
            Select Case f.ToLower
                Case "single", "unmarried" : If SpouseName <> "" Then Return False
                Case "married" : If SpouseName = "" Then Return False
                Case "male", "men" : If Sex <> "Male" Then Return False
                Case "female", "women" : If Sex <> "Female" Then Return False
                Case "child" : If Age >= 12 Then Return False
                Case "adult" : If Age < 12 Then Return False
                Case "employed" : If Occupation = Skill.Vagrant Then Return False
                Case "unemployed" : If Occupation <> Skill.Vagrant Then Return False
            End Select
        Next
        Return True
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
        Console.WriteLine(Name & " - " & Age & Sex.ToLower.First)
        Console.WriteLine("├ House:      " & House.Name)
        Console.WriteLine("├ Father:     " & FatherName)
        Console.WriteLine("├ Mother:     " & MotherName)
        If SpouseName <> "" Then
            Console.WriteLine("├ Spouse:     " & SpouseName)
            Console.WriteLine("├ Children:   " & ChildrenNames.Count)
        End If
        Console.WriteLine("├ Occupation: " & Occupation.ToString)
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

    Public Shared Function Birth(ByVal father As Person, ByVal mother As Person) As Person
        Dim child As New Person
        With child
            If rng.Next(1, 3) = 1 Then ._Sex = "Male" Else ._Sex = "Female"
            .BirthDate = World.TimeNow
            .NameFirst = GrabRandomNameFirst(.Sex)
            .NameLast = GrabRandomNameLast(.Sex, father, mother)
            .FatherName = father.Name
            .MotherName = mother.Name
            father.ChildrenNames.Add(.Name)
            mother.ChildrenNames.Add(.Name)

            For Each s In [Enum].GetValues(GetType(Skill))
                'child's affinity varies by +/- 0.4
                'xp gain always rounded down from affinity
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
    Private Shared Function GrabRandomNameFirst(ByVal childSex As String) As String
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
    Private Shared Function GrabRandomNameLast(ByVal childSex As String, ByVal father As Person, ByVal mother As Person)
        If childSex = "Male" Then
            Return father.NameFirst & "sson"
        ElseIf childSex = "Female" Then
            Return mother.NameFirst & "dottir"
        Else
            Throw New Exception("Sex neither male nor female")
        End If
    End Function
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

                Case Skill.Smelting : Return "Smelter"
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

        newWorkplace.AddWorker(Me)
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
End Class
