Public Class ResourceDict
    Private Data As New Dictionary(Of String, Integer)
    Public Name As String

    Public Sub Add(ByVal key As String, ByVal value As Integer)
        If Data.ContainsKey(key) = False Then Data.Add(key, 0)
        Data.Item(key) += value
    End Sub
    Public Sub ParsedAdd(ByVal data As String)
        Dim ds As String() = data.Split(",")
        Dim key As String = ds(0).Trim
        Dim value As Integer = Convert.ToInt32(ds(1).Trim)

        Add(key, value)
    End Sub

    Public Function Keys() As List(Of String)
        Dim total As New List(Of String)
        For Each k In Data.Keys
            total.Add(k.ToString)
        Next
        Return total
    End Function
    Default Public Property Item(ByVal key As String) As Integer
        Get
            Return Data(key)
        End Get
        Set(ByVal value As Integer)
            Data(key) = value
        End Set
    End Property
    Public Function ContainsKey(ByVal key As String) As Boolean
        Return Keys.Contains(key)
    End Function

    Public Overrides Function ToString() As String
        Dim qtyList As New List(Of String)
        For Each k In Keys()
            qtyList.Add(k & " x" & Item(k))
        Next

        Return Name & " (" & ListToCommaString(qtyList) & ")"
    End Function
End Class
