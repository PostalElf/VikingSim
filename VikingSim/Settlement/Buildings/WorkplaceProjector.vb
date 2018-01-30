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
                    Case "ProjectSubtype" : .ProjectSubtype = entry
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
    Private ProjectSubtype As String
    Public Sub AddProject(ByVal projectName As String, Optional ByVal location As SettlementLocation = Nothing)
        Select Case ProjectType
            Case "Building"
                Project = BuildingProject.Import(projectName)
                Dim p As BuildingProject = CType(Project, BuildingProject)
                If p.LocationString <> "" Then
                    p.Location = location
                    Settlement.RemoveLocation(location)
                End If
                p.PayCost(Settlement)
                p.Creator = Me

            Case "Item"
                Project = ItemProject.Import(projectName)
                Dim p As ItemProject = CType(Project, ItemProject)
                p.PayCost(Settlement)
                p.Creator = Me
        End Select
    End Sub
    Public Function AddProjectCheck(ByVal projectName As String, Optional ByVal location As SettlementLocation = Nothing)
        If Project Is Nothing = False Then Return False

        Select Case ProjectType
            Case "Building"
                If Project Is Nothing = False Then Return False
                Dim p As BuildingProject = CType(BuildingProject.Import(projectName), BuildingProject)
                If p Is Nothing Then Return False
                If p.LocationString <> "" AndAlso location.Name <> p.LocationString Then Return False
                If p.CheckType(ProjectSubtype) = False Then Return False
                If p.CheckCost(Settlement) = False Then Return False
                Return True

            Case "Gear", "Furniture"
                If Project Is Nothing = False Then Return False
                Dim p As ItemProject = CType(ItemProject.Import(projectName), ItemProject)
                If p Is Nothing Then Return False
                If p.CheckType(ProjectSubtype) = False Then Return False
                If p.CheckCost(Settlement) = False Then Return False
                Return True
            Case Else
                Throw New Exception("Unhandled ProjectType")
        End Select
    End Function

    Public Overrides Sub ConsoleReport()
        Console.WriteLine(Name)
        Console.WriteLine("└ History:   " & CreatorName & " in " & CreationDate.ToStringShort)
        Console.WriteLine("└ Works On:  " & ProjectSubtype & " " & ProjectType)
        Console.Write("└ Project:   ")
        If Project Is Nothing = False Then Console.WriteLine(Project.ToString) Else Console.WriteLine("-")
        Console.WriteLine("└ Employees: " & Workers.Count & "/" & WorkerCapacity)
        For Each r In Workers
            Console.WriteLine("  └ " & r.Name)
        Next
        Console.WriteLine()
    End Sub
    Public Overrides Sub Tick()
        If Project Is Nothing Then Exit Sub

        Dim labour As Integer = 0
        For Each p In Workers
            labour += LabourPerWorker(p.PerformWork) + p.GetInventoryBonus(Occupation)
        Next

        If Project.Tick(labour) = True Then
            Dim cp = Project.unpack()
            Select Case cp.GetType
                Case GetType(Building) : Settlement.AddBuilding(cp)
                Case GetType(Gear), GetType(Furniture) : Settlement.AddItem(cp)
            End Select

            Project = Nothing
        End If
    End Sub
End Class
