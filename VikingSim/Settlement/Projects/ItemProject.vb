Public Class ItemProject
    Inherits Project
    Public Shared Function Import(ByVal targetName As String, Optional ByVal pItemType As String = "") As ItemProject
        Dim rawData As List(Of String) = Nothing
        If pItemType = "" Then
            pItemType = "Gear"
            rawData = IO.ImportSquareBracketSelect(IO.sbGear, targetName)
            If rawData Is Nothing Then rawData = IO.ImportSquareBracketSelect(IO.sbFurniture, targetName) : pItemType = "Furniture"
            If rawData Is Nothing Then Return Nothing
        Else
            Dim pathname As String = ""
            Select Case pItemType
                Case "Gear" : pathname = IO.sbGear
                Case "Furniture" : pathname = IO.sbFurniture
            End Select
            rawData = IO.ImportSquareBracketSelect(pathname, targetName)
        End If

        Dim bp As New ItemProject
        With bp
            .Name = targetName
            .ItemType = pItemType

            For Each line In rawData
                Dim ln As String() = line.Split(":")
                Dim header As String = ln(0).Trim
                Dim entry As String = ln(1).Trim
                Select Case header
                    Case Else : .BaseImport(header, entry)
                End Select
            Next
        End With
        Return bp
    End Function

    Private ItemType As String
    Public Overrides Function Unpack()
        Dim item As Item = Nothing
        Select Case ItemType
            Case "Gear" : item = Gear.Import(Name)
            Case "Furniture" : item = Furniture.Import(Name)
        End Select
        item.SetHistory(Creator, World.TimeNow)
        Return item
    End Function

End Class
