Public Class Inventory
    Private Items As New List(Of Item)
    Public Sub AddItem(ByVal item As Item)
        Items.Add(item)
    End Sub
    Public Sub RemoveItem(ByVal item As Item)
        If Items.Contains(item) = False Then Exit Sub
        Items.Remove(item)
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

    Public Function GetBonus(ByVal occ As String) As Integer
        Dim total As Integer = 0
        For Each i In Items
            total += i.GetBonus(occ)
        Next
        Return total
    End Function
End Class