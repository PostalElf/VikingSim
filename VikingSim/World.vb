Public Class World
#Region "Constructors"
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
    Public Shared Function Construct() As World
        Const siteNum As Integer = 10
        Const xMax As Integer = 20
        Const yMax As Integer = 60

        'create 2d array to hold used locations
        'all points in array are marked false to start with
        Dim xy(xMax, yMax) As Boolean
        For x = 1 To xMax
            For y = 1 To yMax
                xy(x, y) = False
            Next
        Next

        Dim world As New World
        With world
            'create settlement sites
            For n = 1 To siteNum
                Dim s As SettlementSite = SettlementSite.Construct
                .SettlementSites.Add(s)
            Next

            'create initial settlement
            Dim site As SettlementSite = SettlementSite.Construct("Tundra")
            Dim settlement As Settlement = settlement.Construct(site, "Askton")

            'create godparents to be the parents of each seed colonist
            Dim godfather As Person = Person.Ancestor("Male")
            Dim godmother As Person = Person.Ancestor("Female")
            Dim house As House = Nothing

            'birth 10 seed colonists and put them into huts of 2 pax each
            For n = 1 To 10
                Dim child As Person = Person.Birth(godfather, godmother, 16)
                If n Mod 2 <> 0 Then
                    house = house.Import("Hut")
                    house.SetHistory("Odin", world.TimeNow)
                    settlement.AddBuilding(house)
                    house.AddFoodEaten("Bread", 1)
                End If
                child.MoveHouse(house)
            Next

            'add to the world proper
            .AddSettlement(settlement)
        End With
        Return world
    End Function
#End Region

#Region "Alerts"
    'higher priority number, the more important it is
    'highest priority is currently 3; remember to add colour in New().AlertsColour if new priorities are added

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
            If AlertsShown(n) = True Then AlertConsoleReport(Alerts(n))
            Alerts(n).Clear()
        Next
    End Sub
    Public Sub AlertConsoleReport(ByVal pAlerts As List(Of Alert))
        If pAlerts.Count = 0 Then Exit Sub

        Dim p As Integer = pAlerts(0).Priority
        Console.ForegroundColor = AlertsColour(p)
        For Each a As Alert In pAlerts
            Console.WriteLine(a.Report)
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
    Public Function GetSettlements(ByVal flags As String) As List(Of Settlement)
        Dim total As New List(Of Settlement)
        For Each s In Settlements
            If s.checkFlags(flags) = True Then total.Add(s)
        Next
        Return total
    End Function

    Private SettlementSites As New List(Of SettlementSite)

    Public Function GetDistance(ByVal origin As iMapLocation, ByVal target As iMapLocation) As Double
        Dim xDist As Integer = Math.Abs(origin.X - target.X)
        Dim yDist As Integer = Math.Abs(origin.Y - target.Y)
        Dim total As Double = Math.Sqrt((xDist * xDist) + (yDist * yDist))
        Return Math.Round(total, 2)
    End Function
#End Region

    Public Overrides Function ToString() As String
        Return TimeNow.ToString
    End Function
    Public Sub ConsoleReport()

    End Sub
    Public Sub Tick()
        TimeNow.Tick()

        For Each Settlement In Settlements
            Settlement.Tick()
        Next
    End Sub
    Public Function GetTickWarnings() As List(Of Alert)
        Dim total As New List(Of Alert)
        For Each Settlement In Settlements
            Dim warnings As List(Of Alert) = Settlement.GetTickWarnings()
            If warnings Is Nothing OrElse warnings.Count = 0 Then Continue For
            total.AddRange(warnings)
        Next
        Return total
    End Function
End Class
