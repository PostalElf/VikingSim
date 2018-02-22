Public Interface iMapLocation
    ReadOnly Property Name As String
    Property X As Integer
    Property Y As Integer

    Function SellGoodsCheck(ByVal inventory As Inventory) As String
    Function SellGoodsCheck(ByVal resources As ResourceDict) As String
    Function SellGoods(ByVal inventory As Inventory) As Integer
    Function SellGoods(ByVal resources As ResourceDict) As Integer
    Function BuyGoodsCheck(ByVal shoppingList As Inventory, ByVal convoy As ConvoyTrade) As String
    Function BuyGoodsCheck(ByVal shoppingList As ResourceDict, ByVal convoy As ConvoyTrade) As String
    Sub BuyGoods(ByVal shoppingList As Inventory, ByVal convoy As ConvoyTrade)
    Sub BuyGoods(ByVal shoppingList As ResourceDict, ByVal convoy As ConvoyTrade)
End Interface
