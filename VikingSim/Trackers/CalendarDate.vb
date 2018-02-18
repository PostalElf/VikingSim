Public Class CalendarDate
    Public Year As Integer = 1
    Public Month As Integer = 1
    Public Week As Integer = 1
    Public ReadOnly Property Season As String
        Get
            If Month >= 7 AndAlso Month <= 12 Then Return "Winter" Else Return "Spring"
        End Get
    End Property

    Const WeeksInMonth As Integer = 4
    Const MonthsInYear As Integer = 12
    Public Sub Tick()
        Week += 1
        If Week >= WeeksInMonth Then
            Week = 1
            Month += 1
            If Month >= MonthsInYear Then
                Month = 1
                Year += 1
            End If
        End If
    End Sub
    Public Sub New(ByVal w As Integer)
        For n = 1 To w
            Tick()
        Next
    End Sub
    Public Sub New(ByVal y As Integer, ByVal m As Integer, ByVal w As Integer)
        Year = y
        Month = m
        Week = w
    End Sub
    Public Sub New(ByVal now As CalendarDate)
        Year = now.Year
        Month = now.Month
        Week = now.Week
    End Sub

    Public Overrides Function ToString() As String
        Return "Week " & Week & ", Month " & Month & ", Year " & Year
    End Function
    Public Function ToStringShort() As String
        Return Season & " " & Year
    End Function
End Class
