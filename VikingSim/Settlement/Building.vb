Public MustInherit Class Building
    Protected Name As String
    Public Settlement As Settlement
    Private _LandUsed As Integer = 5
    Public ReadOnly Property LandUsed As Integer
        Get
            Return _LandUsed
        End Get
    End Property
End Class
