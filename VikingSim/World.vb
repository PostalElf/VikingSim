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

    Public Shared Alerts As New Dictionary(Of Integer, List(Of Alert))
    Public Shared AlertsShown As New Dictionary(Of Integer, Boolean)
    Private Shared AlertsColour As New Dictionary(Of Integer, ConsoleColor)
    Private Const DefaultForegroundColor As ConsoleColor = ConsoleColor.Gray
    Private Const AlertPriorityMin As Integer = 1
    Private Const AlertPriorityMax As Integer = 3
    Private Sub ConsoleReport()
        For n = AlertPriorityMin To AlertPriorityMax
            If AlertsShown(n) = True Then
                Console.ForegroundColor = AlertsColour(n)
                For Each a As Alert In Alerts(n)
                    Console.WriteLine(a.Report)
                Next
            End If
        Next
        Console.ForegroundColor = DefaultForegroundColor
    End Sub

    Public Shared TimeNow As New CalendarDate(1, 1, 1)
    Private Settlements As New List(Of Settlement)
    Public Sub AddSettlement(ByVal settlement As Settlement)
        Settlements.Add(settlement)
    End Sub

    Public Sub Tick()
        TimeNow.Tick()

        For Each Settlement In Settlements
            Settlement.tick()
        Next

        If Alerts.Count > 0 Then ConsoleReport()
    End Sub
End Class
