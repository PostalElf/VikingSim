Public Class IO
#Region "Constants"
    Public Const sbHouses As String = "data/buildings/houses.txt"
    Public Const sbProducers As String = "data/buildings/producers.txt"
    Public Const sbProjectors As String = "data/buildings/projectors.txt"
    Public Const sbResources As String = "data/worldgen/resources.txt"
    Public Const sbTerrain As String = "data/worldgen/terrain.txt"
    Public Const sbNaturalResources As String = "data/worldgen/naturalresources.txt"
    Public Const sbItems As String = "data/worldgen/items.txt"

    Public Const tlGirlNames As String = "data/girlFirstNames.txt"
    Public Const tlMaleNames As String = "data/maleFirstNames.txt"
#End Region

    Public Shared Function ImportTextList(ByVal pathname As String) As List(Of String)
        Dim l As New List(Of String)
        Using sr As New System.IO.StreamReader(pathname)
            While sr.Peek <> -1
                Dim c As String = sr.ReadLine.Trim
                If c = "" Then Continue While
                l.Add(c)
            End While
        End Using
        Return l
    End Function
    Public Shared Function ImportSquareBracketList(ByVal pathname As String) As Dictionary(Of String, List(Of String))
        Dim total As New Dictionary(Of String, List(Of String))
        Using sr As New System.IO.StreamReader(pathname)
            Dim currentHeader As String = ""
            Dim currentList As New List(Of String)
            While sr.Peek <> -1
                Dim c As String = sr.ReadLine.Trim
                If c = "" Then Continue While

                If c.StartsWith("[") AndAlso c.EndsWith("]") Then
                    'if previous list has already been populated, add to total
                    If currentList.Count > 0 AndAlso currentHeader <> "" Then total.Add(currentHeader, currentList)

                    'now get new headertitle and currentlist
                    currentHeader = c.Replace("[", "")
                    currentHeader = currentHeader.Replace("]", "")
                    currentList = New List(Of String)
                    Continue While
                Else
                    currentList.Add(c)
                End If
            End While

            'add last list
            If currentList.Count > 0 AndAlso currentHeader <> "" Then total.Add(currentHeader, currentList)
        End Using
        Return total
    End Function
    Public Shared Function ImportSquareBracketHeaders(ByVal pathname As String) As List(Of String)
        Dim total As New List(Of String)
        Dim rawSquareBracketList As Dictionary(Of String, List(Of String)) = ImportSquareBracketList(pathname)
        For Each r In rawSquareBracketList.Keys
            total.Add(r.ToString)
        Next
        Return total
    End Function
    Public Shared Function ImportSquareBracketSelect(ByVal pathname As String, ByVal targetName As String) As List(Of String)
        Dim rawSquareBracketList As Dictionary(Of String, List(Of String)) = ImportSquareBracketList(pathname)
        For Each r In rawSquareBracketList.Keys
            If r = targetName Then Return rawSquareBracketList(r)
        Next
        Return Nothing
    End Function
End Class
