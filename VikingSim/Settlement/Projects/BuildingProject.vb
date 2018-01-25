Public Class BuildingProject
    Inherits Project
    Public Shared Function Import(ByVal targetName As String) As BuildingProject
        Dim BuildingType As String = "House"
        Dim rawData As List(Of String) = ImportSquareBracketSelect("data/buildings/houses.txt", targetName)
        If rawData Is Nothing Then rawData = ImportSquareBracketSelect("data/buildings/producers.txt", targetName) : BuildingType = "Producer"
        If rawData Is Nothing Then rawData = ImportSquareBracketSelect("data/buildings/projectors.txt", targetName) : BuildingType = "Projector"
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
                    Case Else : .baseImport(header, entry)
                End Select
            Next
        End With
        Return bp
    End Function
    Public Function Unpack() As Building
        Select Case BuildingType
            Case "House" : Return House.Import(Name)
            Case "Producer" : Return WorkplaceProducer.Import(Name, Location)
            Case "Projector" : Return WorkplaceProjector.Import(Name)
            Case Else : Throw New Exception("Unrecgonised BuildingType in BuildingProject")
        End Select
    End Function

    Private BuildingType As String
    Public LocationString As String
    Public Location As NaturalResources
End Class
