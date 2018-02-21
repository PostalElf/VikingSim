Public Class WorkplaceProjector
    Inherits Workplace
    Public Shared Function Import(ByVal workplaceName As String) As WorkplaceProjector
        Dim rawData As List(Of String) = IO.ImportSquareBracketSelect(IO.sbProjectors, workplaceName)

        Dim workplace As New WorkplaceProjector
        With workplace
            ._Name = workplaceName
            For Each line In rawData
                Dim ln As String() = line.Split(":")
                Dim header As String = ln(0).Trim
                Dim entry As String = ln(1).Trim
                Select Case header
                    Case "ProjectType" : .ProjectType = entry
                    Case "ProjectBuildtype" : .ProjectBuildtype = entry
                    Case Else : .WorkplaceImport(header, entry)
                End Select
            Next
        End With
        Return workplace
    End Function
    Public Overrides Function ToString() As String
        Return Name & " - " & Workers.Count & "/" & WorkerCapacity
    End Function

    Private Project As Project
    Private ProjectType As String
    Private ProjectBuildtype As String
    Public Sub AddProject(ByVal projectName As String)
        Select Case ProjectType
            Case "Building" : AddProject(BuildingProject.Import(projectName))
            Case "Item" : AddProject(ItemProject.Import(projectName))
        End Select
    End Sub
    Public Sub AddProject(ByVal pProject As Project)
        Project = pProject

        Select Case ProjectType
            Case "Building"
                Dim p As BuildingProject = CType(Project, BuildingProject)
                If p.Location <> "" Then Settlement.RemoveLocation(p.Location)
                p.PayCost(Settlement)
                p.Creator = Me

            Case "Item"
                Dim p As ItemProject = CType(Project, ItemProject)
                p.PayCost(Settlement)
                p.Creator = Me
        End Select

        'immediately add projects with no buildtime
        If pProject.Tick(0) = True Then Tick()
    End Sub
    Public Function AddProjectCheck(ByVal projectName As String) As String
        Select Case ProjectType
            Case "Building" : Return AddProjectCheck(BuildingProject.Import(projectName))
            Case "Gear", "Furniture" : Return AddProjectCheck(ItemProject.Import(projectName))
            Case Else : Throw New Exception("Unhandled ProjectType")
        End Select
    End Function
    Public Function AddProjectCheck(ByVal pProject As Project) As String
        If Project Is Nothing = False Then Return "Has existing project"
        If pProject Is Nothing Then Return "No project given"

        Select Case ProjectType
            Case "Building"
                Dim p As BuildingProject = TryCast(pProject, BuildingProject)
                If p Is Nothing Then Return "Invalid project type"
                If p.Location <> "" AndAlso Settlement.GetLocations(p.Location).Count = 0 Then Return "Lacks required location: " & p.Location
            Case "Gear", "Furniture"
                Dim p As ItemProject = TryCast(pProject, ItemProject)
                If p Is Nothing Then Return "Invalid project type"
            Case Else
                Throw New Exception("Unhandled ProjectType")
        End Select

        If pProject.CheckBuildType(ProjectBuildtype) = False Then Return Name & " can only work on " & ProjectBuildtype
        If pProject.CheckBuildCost(Settlement) = False Then Return Settlement.Name & " has insufficient resources"
        Return Nothing
    End Function
    Public Function GetPossibleProjects() As List(Of Project)
        Dim rawPathnames As String()
        Select Case ProjectType
            Case "Building" : rawPathnames = {IO.sbHouses, IO.sbProducers, IO.sbProjectors}
            Case "Gear" : rawPathnames = {IO.sbGear}
            Case "Furniture" : rawPathnames = {IO.sbFurniture}
            Case Else : Throw New Exception("Invalid projectType")
        End Select

        Dim total As New List(Of Project)
        For Each pathname In rawPathnames
            Dim raw As List(Of String) = IO.ImportSquareBracketHeaders(pathname)
            For Each header In raw
                Dim project As Project = Nothing
                Select Case pathname
                    Case IO.sbHouses : project = BuildingProject.Import(header, "House")
                    Case IO.sbProducers : project = BuildingProject.Import(header, "Producer")
                    Case IO.sbProjectors : project = BuildingProject.Import(header, "Projector")
                    Case IO.sbGear : project = ItemProject.Import(header, "Gear")
                    Case IO.sbFurniture : project = ItemProject.Import(header, "Furniture")
                End Select
                If project Is Nothing Then Continue For
                If AddProjectCheck(project) = "" Then total.Add(project)
            Next
        Next
        Return total
    End Function

    Public Overrides Sub ConsoleReport()
        MyBase.ConsoleReport()

        Console.WriteLine("└ Works On:    " & ProjectBuildtype & " " & ProjectType)
        Console.Write("└ Project:     ")
        If Project Is Nothing = False Then Console.WriteLine(Project.ToString) Else Console.WriteLine("-")
        Console.WriteLine()
    End Sub
    Public Overrides Sub Tick()
        MyBase.Tick()

        If Project Is Nothing Then Exit Sub

        Dim labour As Integer = 0
        For Each p In Workers
            labour += LabourPerWorker(p.PerformWork) + p.GetInventoryBonus(Occupation)
        Next
        For Each a In Apprentices
            labour += Math.Max(LabourPerWorker(a.PerformWork) - 1, 1)
        Next

        If Project.Tick(labour) = True Then
            Dim cp = Project.unpack()
            Select Case cp.GetType
                Case GetType(House), GetType(WorkplaceProducer), GetType(WorkplaceProjector), GetType(Storage) : Settlement.AddBuilding(cp)
                Case GetType(Gear), GetType(Furniture) : Settlement.AddItem(cp)
            End Select
            World.AddAlert(Me, 2, Name & " has finished production: " & Project.ToString)

            Project = Nothing
        End If
    End Sub
    Public Overrides Function GetTickWarnings() As System.Collections.Generic.List(Of Alert)
        Dim total As New List(Of Alert)
        If Project Is Nothing Then total.Add(New Alert(Me, 1, Name & " is idle."))
        Return total
    End Function
End Class
