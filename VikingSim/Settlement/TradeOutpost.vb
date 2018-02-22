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

    Public Function ConvoySellGoodsCheck(ByVal inventory As Inventory, ByVal convoy As ConvoyTrade) As List(Of Alert)
        Dim total As New List(Of Alert)
        For n = 0 To inventory.Count - 1
            Dim i As Item = inventory(n)
            Dim err As String = ConvoySellGoodCheck(i)
            If err <> "" Then total.Add(New Alert(convoy, 1, err))
        Next
        Return total
    End Function
    Public Function ConvoySellGoodsCheck(ByVal resources As ResourceDict, ByVal convoy As ConvoyTrade) As List(Of Alert)
        Dim total As New List(Of Alert)
        For n = 0 To resources.Keys.Count - 1
            Dim r As String = resources.Keys(n)
            Dim qty As Integer = resources(r)
            Dim err As String = ConvoySellGoodCheck(r, qty)
            If err <> "" Then total.Add(New Alert(convoy, 1, err))
        Next
        Return total
    End Function
    Private Function ConvoySellGoodCheck(ByVal i As Item) As String
        If ItemsBuy.CheckItem(i) = False Then Return Owner.Name & " does not wish to purchase " & i.Name & "."
        If i.Cost > Money Then Return Owner.Name & " does not have enough silver to purchase " & i.Name & "."
        Return Nothing
    End Function
    Private Function ConvoySellGoodCheck(ByVal r As String, ByVal qty As Integer) As String
        Dim cost As Integer = ResourceDict.GetCost(r)
        Dim totalCost As Integer = cost * qty

        If ResourcesBuy.ContainsKey(r) = False Then Return Owner.Name & " does not wish to purchase " & r & "."
        If ResourcesBuy(r) > qty Then Return Owner.Name & " will only buy " & ResourcesBuy(r) & " " & r & "."
        If totalCost > Money Then Return Owner.Name & " does not have enough silver to purchase " & r & "."
        Return Nothing
    End Function
    Public Sub ConvoySellGoodsTrim(ByRef inventory As Inventory, ByVal convoy As ConvoyTrade)
        For n = inventory.Count - 1 To 0 Step -1
            Dim i As Item = inventory(n)
            If ConvoySellGoodCheck(i) <> "" Then inventory.RemoveItem(i)
        Next
    End Sub
    Public Sub ConvoySellGoodsTrim(ByRef resources As ResourceDict, ByVal convoy As ConvoyTrade)
        For n = resources.Keys.Count - 1 To 0 Step -1
            Dim r As String = resources.Keys(n)
            Dim qty As Integer = resources(r)
            If ConvoySellGoodCheck(r, qty) <> "" Then resources.RemoveKey(r)
        Next
    End Sub
    Public Sub ConvoySellGoods(ByVal inventory As Inventory, ByVal convoy As ConvoyTrade)
        Dim total As Integer = 0
        For n = 0 To inventory.Count - 1
            Dim i As Item = inventory(n)

            Money -= i.Cost
            total += i.Cost
            ItemsSell.RemoveItem(i)
        Next
        convoy.AddFunds(total)
    End Sub
    Public Sub ConvoySellGoods(ByVal resources As ResourceDict, ByVal convoy As ConvoyTrade)
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
        Convoy.addFunds(total)
    End Sub

    Public Function ConvoyBuyGoodsCheck(ByVal shoppingList As Inventory, ByVal convoy As ConvoyTrade) As List(Of Alert)
        Dim total As New List(Of Alert)
        For n = shoppingList.Count - 1 To 0 Step -1
            Dim i As Item = shoppingList(n)
            Dim err As String = ConvoyBuyGoodCheck(i, convoy)
            If err <> "" Then total.Add(New Alert(convoy, 1, err))
        Next
        Return total
    End Function
    Public Function ConvoyBuyGoodsCheck(ByVal shoppingList As ResourceDict, ByVal convoy As ConvoyTrade) As List(Of Alert)
        Dim total As New List(Of Alert)
        For n = shoppingList.Keys.Count - 1 To 0 Step -1
            Dim key As String = shoppingList.Keys(n)
            Dim qty As Integer = shoppingList(key)
            Dim err As String = ConvoyBuyGoodCheck(key, qty, convoy)
            If err <> "" Then total.Add(New Alert(convoy, 1, err))
        Next
        Return total
    End Function
    Private Function ConvoyBuyGoodCheck(ByVal i As Item, ByVal convoy As ConvoyTrade) As String
        If ItemsSell.CheckItem(i) = False Then Return Owner.Name & " does not sell " & i.Name & "."
        If convoy.CheckCost(i.Cost) = False Then Return "Convoy has insufficient funds to buy " & i.Name & "."
        Return Nothing
    End Function
    Private Function ConvoyBuyGoodCheck(ByVal r As String, ByVal qty As Integer, ByVal convoy As ConvoyTrade) As String
        Dim baseCost As Integer = ResourceDict.GetCost(r)
        Dim totalCost As Integer = baseCost * qty

        If ResourcesSell.ContainsKey(r) = False Then Return Owner.Name & " does not sell " & r & "."
        If ResourcesSell(r) > qty Then Return Owner.Name & " only sells " & ResourcesSell(r) & " " & r & "."
        If convoy.CheckCost(totalCost) = False Then Return "Convoy has insufficient funds to buy " & r & "."
        Return Nothing
    End Function
    Public Sub ConvoyBuyGoodsTrim(ByVal shoppingList As Inventory, ByVal convoy As ConvoyTrade)
        For n = shoppingList.Count - 1 To 0 Step -1
            Dim i As Item = shoppingList(n)
            If ConvoyBuyGoodCheck(i, convoy) <> "" Then shoppingList.RemoveItem(i)
        Next
    End Sub
    Public Sub ConvoyBuyGoodsTrim(ByVal shoppingList As ResourceDict, ByVal convoy As ConvoyTrade)
        For n = shoppingList.Keys.Count - 1 To 0 Step -1
            Dim r As String = shoppingList.Keys(n)
            Dim qty As Integer = shoppingList(r)
            If ConvoyBuyGoodCheck(r, qty, convoy) <> "" Then shoppingList.RemoveKey(r)
        Next
    End Sub
    Public Sub ConvoyBuyGoods(ByRef shoppingList As Inventory, ByVal convoy As ConvoyTrade)
        For n = 0 To shoppingList.Count - 1
            Dim i As Item = shoppingList(n)
            convoy.AddFunds(-i.Cost)
            Money += i.Cost
        Next
    End Sub
    Public Sub ConvoyBuyGoods(ByRef shoppingList As ResourceDict, ByVal convoy As ConvoyTrade)
        For n = 0 To shoppingList.Keys.Count - 1
            Dim r As String = shoppingList.Keys(n)
            Dim baseCost As Integer = ResourceDict.GetCost(r)
            Dim qty As Integer = shoppingList(r)
            Dim totalCost As Integer = baseCost * qty

            convoy.AddFunds(-totalCost)
            Money += totalCost
        Next
    End Sub
End Class
