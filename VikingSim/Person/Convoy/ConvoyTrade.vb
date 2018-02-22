Public Class ConvoyTrade
    Inherits Convoy
    Public Sub New(ByVal _leader As Person, ByVal _people As List(Of Person), ByVal _origin As iMapLocation, ByVal _destination As iMapLocation, Optional ByVal _isRoundtrip As Boolean = False)
        MyBase.New(_leader, _people, _origin, _destination, _isRoundtrip)
    End Sub

    Private OnWayBack As Boolean = False
    Private SaleInventory As New Inventory
    Private SaleResources As New ResourceDict
    Private ShoppingListInventory As New Inventory
    Private ShoppingListResources As New ResourceDict
    Protected Overrides Sub ArriveDestination()
        If OnWayBack = False Then
            'perform trade
            With Destination
                Dim err As String = .SellGoodsCheck(SaleInventory)
                If err = "" Then .SellGoods(SaleInventory) Else World.AddAlert(Me, 2, err)
                err = .SellGoodsCheck(SaleResources)
                If err = "" Then .SellGoods(SaleResources) Else World.AddAlert(Me, 2, err)
                err = .BuyGoodsCheck(ShoppingListInventory, Me)
                If err = "" Then .BuyGoods(ShoppingListInventory, Me) Else World.AddAlert(Me, 2, err)
                err = .BuyGoodsCheck(ShoppingListResources, Me)
                If err = "" Then .BuyGoods(ShoppingListResources, Me) Else World.AddAlert(Me, 2, err)
            End With

            'turn around and return
            OnWayBack = True
            Dim newDestination As iMapLocation = Origin
            Origin = Destination
            Destination = newDestination
        Else
            'arrived back home
            Dim settlement As Settlement = CType(Destination, Settlement)
            settlement.AddItems(ShoppingListInventory)
            settlement.AddResources(ShoppingListResources)

            Leader.returnhome()
            For Each Person In People
                Person.returnHome()
            Next
        End If
    End Sub
End Class
