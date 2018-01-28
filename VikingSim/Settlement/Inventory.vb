Public Class Inventory
    Private Items As New List(Of Item)
    Public Sub AddItem(ByVal item As Item)
        Items.Add(item)
    End Sub
    Public Function CheckItem(ByVal item As Item) As Boolean
        For Each i In Items
            If i.name = item.name Then Return True
        Next
        Return False
    End Function
    Public Function CheckItems(ByVal item As Item) As Integer
        Dim count As Integer = 0
        For Each i In Items
            If i.name = item.name Then count += 1
        Next
        Return count
    End Function
End Class