Public Class World
    Implements iTickable
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

        Dim world As New World
        With world
            'create settlement sites
            For n = 1 To siteNum
                Dim x, y As Integer
                While True
                    x = rng.Next(1, xMax + 1)
                    y = rng.Next(1, yMax + 1)
                    If .Map(x, y) Is Nothing Then Exit While
                End While
                Dim ss As SettlementSite = SettlementSite.Construct(x, y)
                .AddMapLocation(ss)
            Next

            'create initial settlement
            Dim site As SettlementSite = SettlementSite.Construct(1, 1, "Tundra")
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
                End If
                child.MoveHouse(house)
            Next

            'add to the world proper
            .AddMapLocation(settlement)
        End With
        Return world
    End Function
#End Region

#Region "Save/Load"
    Private Const saveRoot As String = "saves/"
    Private Shared SaveDirectory As String = "01/"
    Private Shared SavePath As String = saveRoot & SaveDirectory

    Public Sub Save()
        Dim raw As New List(Of String)
        With raw
            .Add(TimeNow.GetSaveString)
            For Each ml In MapLocations
                Dim savable As iSaveable = TryCast(ml, iSaveable)
                If savable Is Nothing Then Continue For
                'savable.save()
            Next
        End With

        IO.SaveTextList(SavePath, "world.txt", raw)
    End Sub

    Public Shared Function Load() As World
        Dim raw As List(Of String) = IO.ImportTextList(SavePath, "world.txt")

        Dim world As New World
        With world
            For Each ln In raw
                .ParseLoad(ln)
            Next
        End With
        Return world
    End Function
    Private Sub ParseLoad(ByVal raw As String)
        Dim fs As String() = raw.Split(":")
        Dim header As String = fs(0)
        Dim entry As String = fs(1)
        ParseLoad(header, entry)
    End Sub
    Private Sub ParseLoad(ByVal header As String, ByVal entry As String)
        Select Case header
            Case "TimeNow" : TimeNow = CalendarDate.Load(entry)
            Case "Settlement" : AddMapLocation(Settlement.Load(SavePath, entry))
        End Select
    End Sub
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
    Const xMax As Integer = 40
    Const yMax As Integer = 10

    Private MapLocations As New List(Of iMapLocation)
    Private Map(xMax, yMax) As iMapLocation
    Public Sub AddMapLocation(ByVal ml As iMapLocation)
        MapLocations.Add(ml)
        Map(ml.X, ml.Y) = ml
    End Sub
    Public Function GetMapLocations(ByVal flags As String) As List(Of iMapLocation)
        Dim total As New List(Of iMapLocation)
        For Each ml In MapLocations
            If ml.CheckFlags(flags) = True Then total.Add(ml)
        Next
        Return total
    End Function
    Public Sub RemoveMapLocation(ByVal ml As iMapLocation)
        If MapLocations.Contains(ml) = False Then Exit Sub
        MapLocations.Remove(ml)
        Map(ml.X, ml.Y) = Nothing
    End Sub

    Public Shared Function GetDistance(ByVal origin As iMapLocation, ByVal target As iMapLocation) As Double
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
        For y = 1 To yMax
            For x = 1 To xMax
                If Map(x, y) Is Nothing = False Then
                    Dim loc As iMapLocation = Map(x, y)
                    Select Case loc.GetType
                        Case GetType(Settlement) : Console.Write("%")
                        Case GetType(SettlementSite) : Console.Write(loc.Name.ToLower.First)
                    End Select
                Else
                    Console.Write("*")
                End If
            Next
            Console.WriteLine()
        Next
    End Sub
    Public Sub Tick(ByVal parent As iTickable) Implements iTickable.Tick
        TimeNow.Tick()

        For n = MapLocations.Count - 1 To 0 Step -1
            Dim ml As iMapLocation = MapLocations(n)
            Dim tickable As iTickable = TryCast(ml, iTickable)
            If tickable Is Nothing = False Then tickable.Tick(Me)
        Next
    End Sub
    Public Function GetTickWarnings() As List(Of Alert) Implements iTickable.GetTickWarnings
        Dim total As New List(Of Alert)
        For Each ml In MapLocations
            Dim tickable As iTickable = TryCast(ml, iTickable)
            If tickable Is Nothing = False Then Continue For
            Dim warnings As List(Of Alert) = tickable.GetTickWarnings()
            If warnings Is Nothing OrElse warnings.Count = 0 Then Continue For
            total.AddRange(warnings)
        Next
        Return total
    End Function
End Class
