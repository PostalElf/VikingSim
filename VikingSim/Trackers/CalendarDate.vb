Public Class CalendarDate
    Public Year As Integer = 1
    Public Month As Integer = 1
    Public Week As Integer = 1

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

    Private Function GetWeeks() As Integer
        Dim totalMonths As Integer = (Year - 1) * MonthsInYear
        Dim totalWeeks As Integer = (totalMonths * MonthsInYear) + Week - 1
        Return totalWeeks
    End Function

    Public Overrides Function ToString() As String
        Return "Week " & Week & ", Month " & Month & ", Year " & Year
    End Function
    Public Function ToStringShort() As String
        Return Week & "-" & Month & "-" & Year
    End Function
    Shared Operator -(ByVal t1 As CalendarDate, ByVal t2 As CalendarDate) As CalendarDate
        Dim t1w As Integer = t1.GetWeeks
        Dim t2w As Integer = t2.GetWeeks
        Dim weeksDif As Integer = Math.Abs(t1w - t2w)
        Return New CalendarDate(weeksDif)
    End Operator
End Class
