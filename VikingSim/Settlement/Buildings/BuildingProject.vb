Public Class BuildingProject
    Public Shared Function Import(ByVal targetName As String) As BuildingProject
        Dim rawData As List(Of String) = ImportSquareBracketSelect("data/buildings/houses.txt", targetName)
        If rawData Is Nothing Then rawData = ImportSquareBracketSelect("data/buildings/workplaces.txt", targetName)
        If rawData Is Nothing Then Throw New Exception(targetName & " not found for BuildingProject.Import")

        Dim bp As New BuildingProject
        With bp
            For Each line In rawData
                Dim ln As String() = line.Split(":")
                Dim header As String = ln(0).Trim
                Dim entry As String = ln(1).Trim
                Select Case header
                    Case "Buildcost", "Build"
                End Select
            Next
        End With
    End Function
    Public Shared Function Unpack() As Building

    End Function

    Private ConstructionCosts As New ResourceDict
End Class
