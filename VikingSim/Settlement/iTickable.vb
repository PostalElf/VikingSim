Public Interface iTickable
    Sub Tick()
    Function GetTickWarnings() As List(Of Alert)
End Interface
