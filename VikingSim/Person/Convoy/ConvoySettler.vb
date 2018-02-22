Public Class ConvoySettler
    Inherits Convoy
    Public Sub New(ByVal _leader As Person, ByVal _people As List(Of Person), ByVal _origin As iMapLocation, ByVal _destination As iMapLocation, Optional ByVal _isRoundtrip As Boolean = False)
        MyBase.New(_leader, _people, _origin, _destination, _isRoundtrip)
    End Sub

    Protected Overrides Sub ArriveDestination()

    End Sub
End Class
