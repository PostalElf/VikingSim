Public Class TradeOutpost
    Public Function ConvoySellGoodsCheck(ByVal inventory As Inventory) As List(Of Alert)
        For n = 0 To inventory.Count - 1
            Dim i As Item = inventory(n)

            'TODO: AI shit to determine if the natives want to buy the shit
            'CODE GOES HERE

            'they want to buy your shit? cool
        Next
    End Function
    Public Function ConvoySellGoodsCheck(ByVal resources As ResourceDict) As List(Of Alert)
        'TODO: AI shit to determine if the natives want to buy the shit
    End Function
    Public Function ConvoySellGoods(ByVal inventory As Inventory) As Integer
        Dim total As Integer = 0
        For n = 0 To inventory.Count - 1
            Dim i As Item = inventory(n)
            total += i.Cost
        Next
        Return total
    End Function
    Public Function ConvoySellGoods(ByVal resources As ResourceDict) As Integer
        Dim total As Integer = 0
        For Each r In resources.Keys
            Dim baseCost As Integer = resources(r)
            total += baseCost
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
