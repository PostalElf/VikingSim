Public Class Inventory
    Private Items As New List(Of Item)
    Default Public Property Item(ByVal i As Integer) As Item
        Get
            Return Items(i)
        End Get
        Set(ByVal value As Item)
            Items(i) = value
        End Set
    End Property
    Public Function Count() As Integer
        Return Items.Count
    End Function
    Public Sub AddItem(ByVal item As Item)
        Items.Add(item)
    End Sub
    Public Sub RemoveItem(ByVal item As Item)
        If Items.Contains(item) = False Then Exit Sub
        Items.Remove(item)
    End Sub
    Public Sub DumpItems(ByVal settlement As Settlement)
        For n = Items.Count - 1 To 0 Step -1
            settlement.AddItem(Items(n))
            Items.RemoveAt(n)
        Next
    End Sub
    Public Function CheckItem(ByVal item As Item) As Boolean
        For Each i In Items
            If i.Name = item.Name Then Return True
        Next
        Return False
    End Function
    Public Function CheckItems(ByVal item As Item) As Integer
        Dim count As Integer = 0
        For Each i In Items
            If i.Name = item.Name Then count += 1
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