Public MustInherit Class Project
    Protected Sub BaseImport(ByVal header As String, ByVal entry As String)
        Select Case header
            Case "Buildtime", "Time" : BuildTime = Convert.ToInt32(entry)
            Case "Buildcost", "Build" : ConstructionCosts.ParsedAdd(entry)
        End Select
    End Sub

    Protected Name As String
    Public Creator As String
    Protected BuildTime As Integer
    Protected BuildTimeProgress As Integer
    Protected ConstructionCosts As New ResourceDict

    Public Function Tick(ByVal progress As Integer) As Boolean
        'return true when completed
        BuildTimeProgress += progress
        If BuildTimeProgress >= BuildTime Then Return True

        Return False
    End Function
End Class
