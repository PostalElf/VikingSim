Public Class BuildingProject
    Public Shared Function Import(ByVal targetName As String) As BuildingProject
        Dim BuildingType As String = "House"
        Dim rawData As List(Of String) = ImportSquareBracketSelect("data/buildings/houses.txt", targetName)
        If rawData Is Nothing Then rawData = ImportSquareBracketSelect("data/buildings/workplaces.txt", targetName) : BuildingType = "Workplace"
        If rawData Is Nothing Then Throw New Exception(targetName & " not found for BuildingProject.Import")

        Dim bp As New BuildingProject
        With bp
            .BuildingType = BuildingType
            .BuildingName = targetName
            For Each line In rawData
                Dim ln As String() = line.Split(":")
                Dim header As String = ln(0).Trim
                Dim entry As String = ln(1).Trim
                Select Case header
                    Case "Buildtime", "Time" : .BuildTime = Convert.ToInt32(entry)
                    Case "Buildcost", "Build" : .ConstructionCosts.ParsedAdd(entry)
                End Select
            Next
        End With
        Return bp
    End Function
    Public Function Tick(ByVal labour As Integer) As Boolean
        'return true when completed
        BuildTimeProgress += labour
        If BuildTimeProgress >= BuildTime Then Return True

        Return False
    End Function
    Public Function Unpack() As Building
        Select Case BuildingType
            Case "House" : Return House.Import(BuildingName)
            Case "Workplace" : Return Workplace.Import(BuildingName)
            Case Else : Throw New Exception("Unrecgonised BuildingType in BuildingProject")
        End Select
    End Function

    Private BuildingType As String
    Private BuildingName As String
    Private BuildTime As Integer
    Private BuildTimeProgress As Integer
    Private ConstructionCosts As New ResourceDict
End Class
