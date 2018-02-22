Public Class TradeOutpost
    Private Owner As iMapLocation
    Public Sub New(ByVal _owner As iMapLocation)
        Owner = _owner
    End Sub

    Private Money As Integer
    Private ItemsBuy As New Inventory
    Private ItemsSell As New Inventory
    Private ResourcesBuy As New ResourceDict
    Private ResourcesSell As New ResourceDict

    Public Function ConvoySellGoodsCheck(ByVal inventory As Inventory) As List(Of Alert)
        Dim total As New List(Of Alert)
        For n = 0 To inventory.Count - 1
            Dim i As Item = inventory(n)
            If ItemsBuy.CheckItem(i) = False Then total.Add(New Alert(i, 1, Owner.Name & " does not wish to purchase " & i.Name & ".")) : Continue For
            If i.Cost > Money Then total.Add(New Alert(i, 1, Owner.Name & " does not have enough silver to purchase " & i.Name & ".")) : Continue For
        Next
        Return total
    End Function
    Public Function ConvoySellGoodsCheck(ByVal resources As ResourceDict) As List(Of Alert)
        Dim total As New List(Of Alert)
        For n = 0 To resources.Keys.Count - 1
            Dim r As String = resources.Keys(n)
            Dim qty As Integer = resources(r)
            Dim baseCost As Integer = ResourceDict.GetCost(r)
            Dim totalCost As Integer = baseCost * qty

            If ResourcesSell.ContainsKey(r) = False Then total.Add(New Alert(Me, 1, Owner.Name & " does not wish to purchase " & r & "."))
            If ResourcesSell(r) > resources(r) Then total.Add(New Alert(Me, 1, Owner.Name & " will only sell " & ResourcesSell(r) & " " & r & "."))
            If totalCost > Money Then total.Add(New Alert(Me, 1, Owner.Name & " does not have enough silver to purchase " & r & "."))
        Next
        Return total
    End Function
    Public Function ConvoySellGoods(ByVal inventory As Inventory) As Integer
        Dim total As Integer = 0
        For n = 0 To inventory.Count - 1
            Dim i As Item = inventory(n)

            Money -= i.Cost
            total += i.Cost
            ItemsSell.RemoveItem(i)
        Next
        Return total
    End Function
    Public Function ConvoySellGoods(ByVal resources As ResourceDict) As Integer
        Dim total As Integer = 0
        For Each r In resources.Keys
            Dim baseCost As Integer = ResourceDict.GetCost(r)
            Dim qty As Integer = resources(r)
            Dim totalCost As Integer = baseCost * qty

            Money -= totalCost
            total += baseCost
            If ResourcesSell.ContainsKey(r) = False Then Continue For
            ResourcesSell(r) -= qty
            If ResourcesSell(r) <= 0 Then ResourcesSell.RemoveKey(r)
        Next
        Return total
    End Function
    Public Function ConvoyBuyGoodsCheck(ByVal shoppingList As Inventory, ByVal convoy As ConvoyTrade) As List(Of Alert)
        Dim total As New List(Of Alert)

        'TODO: AI shit to determine if the natives sell the shit

        For n = shoppingList.Count - 1 To 0 Step -1
            Dim i As Item = shoppingList(n)
            If convoy.CheckCost(i.Cost) = False Then total.Add(New Alert(convoy, 1, "Convoy has insufficient funds to buy " & i.Name & "."))
        Next

        Return total
    End Function
    Public Function ConvoyBuyGoodsCheck(ByVal shoppingList As ResourceDict, ByVal convoy As ConvoyTrade) As List(Of Alert)
        Dim total As New List(Of Alert)

        'TODO: AI shit to determine if the natives sell the shit

        For n = shoppingList.Keys.Count - 1 To 0 Step -1
            Dim key As String = shoppingList.Keys(n)
            Dim baseCost As Integer = ResourceDict.GetCost(key)
            Dim qty As Integer = shoppingList(key)
            Dim totalCost As Integer = baseCost * qty
            If convoy.CheckCost(totalCost) = False Then total.Add(New Alert(convoy, 1, "Convoy has insufficient funds to buy " & key & "."))
        Next

        Return total
    End Function
    Public Sub ConvoyBuyGoods(ByVal shoppingList As Inventory, ByVal convoy As ConvoyTrade)

    End Sub
    Public Sub ConvoyBuyGoods(ByVal shoppingList As ResourceDict, ByVal convoy As ConvoyTrade)

    End Sub
End Class
