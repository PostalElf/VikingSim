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
    Public Sub AddProject(ByVal projectName As String, Optional ByVal location As String = Nothing)
        Select Case ProjectType
            Case "Building" : AddProject(BuildingProject.Import(projectName), location)
            Case "Item" : AddProject(ItemProject.Import(projectName), location)
        End Select
    End Sub
    Public Sub AddProject(ByVal pProject As Project, Optional ByVal location As String = Nothing)
        Project = pProject

        Select Case ProjectType
            Case "Building"
                Dim p As BuildingProject = CType(Project, BuildingProject)
                If p.Location <> "" Then
                    p.Location = location
                    Settlement.RemoveLocation(location)
                End If
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
    Public Function AddProjectCheck(ByVal projectName As String)
        Select Case ProjectType
            Case "Building" : Return AddProjectCheck(BuildingProject.Import(projectName))
            Case "Gear", "Furniture" : Return AddProjectCheck(ItemProject.Import(projectName))
            Case Else : Throw New Exception("Unhandled ProjectType")
        End Select
    End Function
    Public Function AddProjectCheck(ByVal pProject As Project)
        If Project Is Nothing = False Then Return False

        Select Case ProjectType
            Case "Building"
                If Project Is Nothing = False Then Return False
                If pProject Is Nothing Then Return False
                Dim p As BuildingProject = TryCast(pProject, BuildingProject)
                If p Is Nothing Then Return False
                If p.Location <> "" AndAlso Settlement.GetLocations(p.Location).Count = 0 Then Return False
                If p.CheckBuildType(ProjectBuildtype) = False Then Return False
                If p.CheckBuildCost(Settlement) = False Then Return False
                Return True

            Case "Gear", "Furniture"
                If Project Is Nothing = False Then Return False
                If pProject Is Nothing Then Return False
                Dim p As ItemProject = TryCast(pProject, ItemProject)
                If p Is Nothing Then Return False
                If p.CheckBuildType(ProjectBuildtype) = False Then Return False
                If p.CheckBuildCost(Settlement) = False Then Return False
                Return True
            Case Else
                Throw New Exception("Unhandled ProjectType")
        End Select
    End Function

    Public Overrides Sub ConsoleReport()
        Console.WriteLine(Name)
        Console.WriteLine("└ Made By:   " & CreatorName & " in " & CreationDate.ToStringShort)
        Console.WriteLine("└ Works On:  " & ProjectBuildtype & " " & ProjectType)
        Console.Write("└ Project:   ")
        If Project Is Nothing = False Then Console.WriteLine(Project.ToString) Else Console.WriteLine("-")
        Console.WriteLine("└ Employees: " & Workers.Count & "/" & WorkerCapacity)
        For Each r In Workers
            Console.WriteLine("  └ " & r.Name)
        Next
        Console.WriteLine()
    End Sub
    Public Overrides Sub Tick()
        MyBase.Tick()

        If Project Is Nothing Then Exit Sub

        Dim labour As Integer = 0
        For Each p In Workers
            labour += LabourPerWorker(p.PerformWork) + p.GetInventoryBonus(Occupation)
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
End Class
