Public Class World
    Public Sub New()
        For n = AlertPriorityMin To AlertPriorityMax
            Alerts.Add(n, New List(Of Alert))
            AlertsShown.Add(n, True)
            Select Case n
                Case 1 : AlertsColour.Add(n, ConsoleColor.DarkGray)
                Case 2 : AlertsColour.Add(n, ConsoleColor.Gray)
                Case 3 : AlertsColour.Add(n, ConsoleColor.White)
            End Select
        Next
    End Sub
    Public Overrides Function ToString() As String
        Return TimeNow.ToString
    End Function

#Region "Alerts"
    Public Shared AlertsShown As New Dictionary(Of Integer, Boolean)
    Private Shared Alerts As New Dictionary(Of Integer, List(Of Alert))
    Private Shared AlertsColour As New Dictionary(Of Integer, ConsoleColor)
    Private Const DefaultForegroundColor As ConsoleColor = ConsoleColor.Gray
    Private Const AlertPriorityMin As Integer = 1
    Private Const AlertPriorityMax As Integer = 3
    Public Shared Sub AddAlert(ByVal ref As Object, ByVal priority As Integer, ByVal str As String)
        Dim alert As New Alert(ref, priority, str)
        Alerts(priority).Add(alert)
    End Sub
    Public Sub AlertConsoleReport()
        For n = AlertPriorityMin To AlertPriorityMax
            If AlertsShown(n) = True Then
                Console.ForegroundColor = AlertsColour(n)
                For Each a As Alert In Alerts(n)
                    Console.WriteLine(a.Report)
                Next
            End If
            Alerts(n).Clear()
        Next
        Console.ForegroundColor = DefaultForegroundColor
    End Sub
#End Region

#Region "Timekeeping"
    Public Shared TimeNow As New CalendarDate(1, 1, 1)
#End Region

#Region "World Map"
    Private Settlements As New List(Of Settlement)
    Public Sub AddSettlement(ByVal settlement As Settlement)
        Settlements.Add(settlement)
    End Sub

    Private SettlementSites As New List(Of SettlementSite)

    Public Function GetDistance(ByVal origin As iMapLocation, ByVal target As iMapLocation) As Double
        Dim xDist As Integer = Math.Abs(origin.X - target.X)
        Dim yDist As Integer = Math.Abs(origin.Y - target.Y)
        Dim total As Double = Math.Sqrt((xDist * xDist) + (yDist * yDist))
        Return Math.Round(total, 2)
    End Function
#End Region

    Public Sub Tick()
        TimeNow.Tick()

        For Each Settlement In Settlements
            Settlement.tick()
        Next
    End Sub
End Class
