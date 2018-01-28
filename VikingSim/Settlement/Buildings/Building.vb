Public MustInherit Class Building
    Protected _Name As String
    Public ReadOnly Property Name As String
        Get
            Return _Name
        End Get
    End Property
    Public Settlement As Settlement
    Private _LandUsed As Integer = 5
    Public ReadOnly Property LandUsed As Integer
        Get
            Return _LandUsed
        End Get
    End Property
    Private Inventory As New Inventory
    Public MustOverride Sub Tick()

    Protected Sub BaseImport(ByVal header As String, ByVal entry As String)
        Select Case header
            Case "Land" : _LandUsed = Convert.ToInt32(entry)
        End Select
    End Sub
End Class
