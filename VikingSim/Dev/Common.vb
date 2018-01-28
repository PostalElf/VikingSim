<DebuggerStepThrough()>
Module Common
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
    Public Function ListToCommaString(ByVal str As String()) As String
        Dim total As String = ""
        For n = 0 To str.Count - 1
            total &= str(n)
            If n <> str.Count - 1 Then total &= ", "
        Next
        Return total
    End Function
    Public Function ListToCommaString(ByVal str As List(Of String)) As String
        Dim total As String = ""
        For n = 0 To str.Count - 1
            total &= str(n)
            If n <> str.Count - 1 Then total &= ", "
        Next
        Return total
    End Function
    Public Function CommaStringToList(ByVal str As String) As List(Of String)
        Dim total As New List(Of String)
        Dim ln As String() = str.Split(",")
        For Each l In ln
            total.Add(l.Trim)
        Next
        Return total
    End Function
End Module
