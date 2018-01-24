<DebuggerStepThrough()>
Module Common
    Public rng As New Random(3)

    Public Function FudgeRoll(Optional ByVal dice As Integer = 4) As Integer
        Dim total As Integer = 0
        For n = 1 To dice
            Dim roll As Integer = rng.Next(1, 4)
            Select Case roll
                Case 1 : total -= 1
                Case 2 : total += 1
            End Select
        Next
        Return total
    End Function

    Public Function ImportTextList(ByVal pathname As String) As List(Of String)
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
    Public Function ImportSquareBracketList(ByVal pathname As String) As Dictionary(Of String, List(Of String))
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
    Public Function ImportSquareBracketSelect(ByVal pathname As String, ByVal targetName As String) As List(Of String)
        Dim rawSquareBracketList As Dictionary(Of String, List(Of String)) = ImportSquareBracketList(pathname)
        For Each r In rawSquareBracketList.Keys
            If r = targetName Then Return rawSquareBracketList(r)
        Next
        Throw New Exception("Targetname " & targetName & " not found in " & pathname)
    End Function

    Public Function GrabRandom(Of T)(ByRef sourceList As List(Of T)) As T
        Dim roll As Integer = rng.Next(sourceList.Count)
        GrabRandom = sourceList(roll)
        sourceList.RemoveAt(roll)
    End Function
    Public Function GetRandom(Of T)(ByVal sourceList As List(Of T)) As T
        Dim roll As Integer = rng.Next(sourceList.Count)
        Return sourceList(roll)
    End Function

    Public Function StringToEnum(Of T)(ByVal str As String) As T
        For Each e In [Enum].GetValues(GetType(T))
            If e.ToString = str Then Return e
        Next
        Throw New Exception("StringToEnum: no match found for " & str)
    End Function
End Module
