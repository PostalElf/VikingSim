Public Interface iTickable
    Sub Tick(ByVal parent As iTickable)
    Function GetTickWarnings() As List(Of Alert)
End Interface
