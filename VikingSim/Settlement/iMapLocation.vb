Public Interface iMapLocation
    ReadOnly Property Name As String
    Property X As Integer
    Property Y As Integer
    Function CheckFlags(ByVal flags As String) As Boolean
End Interface
