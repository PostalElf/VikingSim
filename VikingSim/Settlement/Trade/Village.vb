Public Class Village
    Implements iTradable, iMapLocation, iBuildable, iTickable

#Region "Constructors"
    Public Shared Function Construct(ByVal ib As iBuildable)
        Dim village As New Village
        With village
            .X = ib.X
            .Y = ib.Y
            .Terrain = ib.Terrain
            .Locations = ib.Locations
        End With
        Return village
    End Function
#End Region

#Region "Personal Identifiers"
    Private _Name As String
    Public ReadOnly Property Name As String Implements iMapLocation.Name, iTradable.Name
        Get
            Return _Name
        End Get
    End Property
    Public Overrides Function ToString() As String
        Return "Village - " & Name
    End Function
    Public Function CheckFlags(ByVal flags As String) As Boolean Implements iMapLocation.CheckFlags
        For Each f In flags.Split(",")
            Select Case f.Trim
                Case "village" 'do nothing if true
                Case Else : Return False
            End Select
        Next
        Return True
    End Function

    Private Property X As Integer Implements iMapLocation.X, iTradable.X, iBuildable.X
    Private Property Y As Integer Implements iMapLocation.Y, iTradable.Y, iBuildable.Y

    Private Property Terrain As String Implements iBuildable.Terrain
    Private Property Locations As List(Of String) Implements iBuildable.Locations
#End Region

#Region "Trade"
    Private Property TradeOutpost As TradeOutpost Implements iTradable.TradeOutpost
#End Region

    Public Sub Tick(ByVal parent As iTickable) Implements iTickable.Tick

    End Sub
    Public Function GetTickWarnings() As List(Of Alert) Implements iTickable.GetTickWarnings
        Return Nothing
    End Function
End Class
