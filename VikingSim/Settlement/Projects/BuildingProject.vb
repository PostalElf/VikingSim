Public Class BuildingProject
    Inherits Project
    Public Shared Function Import(ByVal targetName As String) As BuildingProject
        Dim BuildingType As String = "House"
        Dim rawData As List(Of String) = IO.ImportSquareBracketSelect(IO.sbHouses, targetName)
        If rawData Is Nothing Then rawData = IO.ImportSquareBracketSelect(IO.sbProducers, targetName) : BuildingType = "Producer"
        If rawData Is Nothing Then rawData = IO.ImportSquareBracketSelect(IO.sbProjectors, targetName) : BuildingType = "Projector"
        If rawData Is Nothing Then Throw New Exception(targetName & " not found for BuildingProject.Import")

        Dim bp As New BuildingProject
        With bp
            .BuildingType = BuildingType
            .Name = targetName

            For Each line In rawData
                Dim ln As String() = line.Split(":")
                Dim header As String = ln(0).Trim
                Dim entry As String = ln(1).Trim
                Select Case header
                    Case "Resource" : .LocationString = entry
                    Case "Buildtype" : .BuildType = entry
                    Case Else : .baseImport(header, entry)
                End Select
            Next
        End With
        Return bp
    End Function
    Public Function Unpack() As Building
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
