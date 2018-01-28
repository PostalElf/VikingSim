Public Class Inventory
    Private Items As New List(Of String)
    Public Sub AddItem(ByVal item As String)
        Items.Add(item)
    End Sub
    Public Function CheckItem(ByVal item As String) As Boolean
        Return Items.Contains(item)
    End Function
    Public Function CheckItems(ByVal item As String) As Integer
        Dim count As Integer = 0
        For Each i In Items
            If i = item Then count += 1
        Next
        Return count
    End Function
End Class
