﻿Public Class WorkplaceProjector
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
    Public Sub AddProject(ByVal projectName As String, Optional ByVal location As SettlementLocation = Nothing)
        Select Case ProjectType
            Case "Building"
                Project = BuildingProject.Import(projectName)
                Dim p As BuildingProject = CType(Project, BuildingProject)
                If p.LocationString <> "" Then
                    p.Location = location
                    Settlement.RemoveLocation(location)
                End If
                p.creator = Me
        End Select
    End Sub

    Public Overrides Sub ConsoleReport()
        Console.WriteLine(Name)
        Console.WriteLine("└ History:   " & Creator & " in " & CreationDate.ToStringShort)
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
            labour += LabourPerWorker(p.PerformWork)
        Next

        If Project.Tick(labour) = True Then
            Select Case Project.GetType
                Case GetType(BuildingProject)
                    Dim p As BuildingProject = CType(Project, BuildingProject)
                    Settlement.AddBuilding(p.Unpack())
            End Select

            Project = Nothing
        End If
    End Sub
End Class
