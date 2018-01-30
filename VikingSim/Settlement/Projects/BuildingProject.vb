Public Class BuildingProject
    Inherits Project
    Public Shared Function Import(ByVal targetName As String, Optional ByVal pBuildingType As String = "") As BuildingProject
        Dim rawData As List(Of String) = Nothing
        If pBuildingType = "" Then
            pBuildingType = "House"
            rawData = IO.ImportSquareBracketSelect(IO.sbHouses, targetName)
            If rawData Is Nothing Then rawData = IO.ImportSquareBracketSelect(IO.sbProducers, targetName) : pBuildingType = "Producer"
            If rawData Is Nothing Then rawData = IO.ImportSquareBracketSelect(IO.sbProjectors, targetName) : pBuildingType = "Projector"
            If rawData Is Nothing Then Throw New Exception(targetName & " not found for BuildingProject.Import")
        Else
            Dim pathname As String = ""
            Select Case pBuildingType
                Case "House" : pathname = IO.sbHouses
                Case "Producer" : pathname = IO.sbProducers
                Case "Projector" : pathname = IO.sbProjectors
            End Select
            rawData = IO.ImportSquareBracketSelect(pathname, targetName)
        End If

        Dim bp As New BuildingProject
        With bp
            .BuildingType = pBuildingType
            .Name = targetName

            For Each line In rawData
                Dim ln As String() = line.Split(":")
                Dim header As String = ln(0).Trim
                Dim entry As String = ln(1).Trim
                Select Case header
                    Case "Resource" : .LocationString = entry
                    Case "Buildtype" : .BuildType = entry
                    Case Else : .BaseImport(header, entry)
                End Select
            Next
        End With
        Return bp
    End Function
    Public Overrides Function Unpack()
        Dim b As Building
        Select Case BuildingType
            Case "House" : b = House.Import(Name)
            Case "Producer" : b = WorkplaceProducer.Import(Name, Location)
            Case "Projector" : b = WorkplaceProjector.Import(Name)
            Case Else : Throw New Exception("Unrecgonised BuildingType in BuildingProject")
        End Select
        b.SetHistory(Creator, World.TimeNow)
        Return b
    End Function

    Private BuildingType As String
    Public LocationString As String
    Public Location As NaturalResources
End Class
