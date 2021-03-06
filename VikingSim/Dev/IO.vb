﻿<DebuggerStepThrough()>
Public Class IO
#Region "Constants"
    Public Const sbHouses As String = "data/buildings/houses.txt"
    Public Const sbStorage As String = "data/buildings/storage.txt"
    Public Const sbProducers As String = "data/buildings/producers.txt"
    Public Const sbProjectors As String = "data/buildings/projectors.txt"
    Public Const sbPosts As String = "data/buildings/posts.txt"
    Public Const sbResources As String = "data/worldgen/resources.txt"
    Public Const sbTerrain As String = "data/worldgen/terrain.txt"
    Public Const sbDiseases As String = "data/worldgen/diseases.txt"
    Public Const sbModifiers As String = "data/worldgen/modifiers.txt"
    Public Const sbGear As String = "data/items/gear.txt"
    Public Const sbFurniture As String = "data/items/furniture.txt"

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
    Public Shared Function ImportTextList(ByVal rootPath As String, ByVal filename As String) As List(Of String)
        If rootPath.EndsWith("/") = False Then rootPath &= "/"
        Return ImportTextList(rootPath & filename)
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

    Public Shared Sub SaveTextList(ByVal rootPath As String, ByVal filename As String, ByVal raw As List(Of String))
        If System.IO.Directory.Exists(rootPath) = False Then System.IO.Directory.CreateDirectory(rootPath)
        If rootPath.EndsWith("/") = False Then rootPath &= "/"

        Using sr As New System.IO.StreamWriter(rootPath & filename)
            For Each ln In raw
                sr.WriteLine(ln)
            Next
        End Using
    End Sub
    Public Shared Sub SaveSquareBracketList(ByVal rootPath As String, ByVal filename As String, ByVal raw As Dictionary(Of String, List(Of String)))
        If System.IO.Directory.Exists(rootPath) = False Then System.IO.Directory.CreateDirectory(rootPath)
        If rootPath.EndsWith("/") = False Then rootPath &= "/"
        SaveSquareBracketList(rootPath & filename, raw)
    End Sub
    Public Shared Sub SaveSquareBracketList(ByVal pathname As String, ByVal raw As Dictionary(Of String, List(Of String)))
        Using sr As New System.IO.StreamWriter(pathname)
            For Each k In raw.Keys
                Dim rawList As List(Of String) = raw(k)
                sr.WriteLine("[" & k & "]")
                For Each ln In rawList
                    sr.WriteLine(ln)
                Next
                sr.WriteLine()
            Next
        End Using
    End Sub
End Class
