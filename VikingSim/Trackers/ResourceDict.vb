Public Class ResourceDict
    Private Data As New Dictionary(Of String, Integer)
    Public Shared Function GetCategory(ByVal key As String) As String
        Dim allResources As Dictionary(Of String, List(Of String)) = IO.ImportSquareBracketList(IO.sbResources)
        For Each k In allResources.Keys
            For Each klist In allResources(k)
                If klist.Contains(key) Then Return k
            Next
        Next
        Return Nothing
    End Function


    Public Sub Add(ByVal key As String, ByVal value As Integer)
        If Data.ContainsKey(key) = False Then Data.Add(key, 0)
        Data.Item(key) += value
        If Data(key) = 0 Then RemoveKey(key)
    End Sub
    Public Sub ParsedAdd(ByVal data As String)
        Dim ds As String() = data.Split(",")
        Dim key As String = ds(0).Trim
        Dim value As Integer = Convert.ToInt32(ds(1).Trim)

        Add(key, value)
    End Sub
    Public Sub RemoveKey(ByVal key As String)
        Data.Remove(key)
    End Sub

    Public Function Keys() As List(Of String)
        Dim total As New List(Of String)
        For Each k In Data.Keys
            total.Add(k.ToString)
        Next
        Return total
    End Function
    Public Function ContainsKey(ByVal key As String) As Boolean
        Return Keys.Contains(key)
    End Function
    Public Function Count() As Integer
        Return Data.Count
    End Function
    Default Public Property Item(ByVal key As String) As Integer
        Get
            If Data.ContainsKey(key) = False Then Return 0
            Return Data(key)
        End Get
        Set(ByVal value As Integer)
            Data(key) = value
        End Set
    End Property

    Public Overrides Function ToString() As String
        Dim qtyList As New List(Of String)
        For Each k In Keys()
            qtyList.Add(k & " x" & Item(k))
        Next

        Return ListToCommaString(qtyList)
    End Function
    Public Shared Operator *(ByVal rd As ResourceDict, ByVal qty As Integer) As ResourceDict
        Dim total As New ResourceDict
        For Each k In rd.Keys
            total.Add(k, rd(k) * qty)
        Next
        Return total
    End Operator
    Public Shared Function GetCost(ByVal value As String) As Integer
        'TODO: add base costs for resources
        Return 1
    End Function
End Class
