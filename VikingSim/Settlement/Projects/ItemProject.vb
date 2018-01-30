Public Class ItemProject
    Inherits Project
    Public Shared Function Import(ByVal targetName As String, Optional ByVal pBuildType As String = "") As ItemProject
        Dim rawData As List(Of String) = Nothing
        If pBuildType = "" Then
            pBuildType = "Gear"
            rawData = IO.ImportSquareBracketSelect(IO.sbGear, targetName)
            If rawData Is Nothing Then rawData = IO.ImportSquareBracketSelect(IO.sbFurniture, targetName) : pBuildType = "Furniture"
            If rawData Is Nothing Then Throw New Exception("TargetName not found for Item.Import : " & targetName)
        Else
            Dim pathname As String = ""
            Select Case pBuildType
                Case "Gear" : pathname = IO.sbGear
                Case "Furniture" : pathname = IO.sbFurniture
            End Select
            rawData = IO.ImportSquareBracketSelect(pathname, targetName)
        End If

        Dim bp As New ItemProject
        With bp
            .Name = targetName
            .BuildType = pBuildType

            For Each line In rawData
                Dim ln As String() = line.Split(":")
                Dim header As String = ln(0)
                Dim entry As String = ln(1)
                Select Case header
                    Case Else : .BaseImport(header, entry)
                End Select
            Next
        End With
        Return bp
    End Function
    Public Function Unpack() As Item
        Dim item As Item = Nothing
        Select Case BuildType
            Case "Furniture" : item = Furniture.import(Name)
            Case "Gear" : item = Gear.Import(Name)
            Case Else : Throw New Exception("Unhandled item.BuildType: " & BuildType)
        End Select
        item.SetHistory(Creator, World.TimeNow)
        Return item
    End Function

End Class
