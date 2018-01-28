Public Class Item
    Implements iHistorable

    Public Name As String
    Public Property Creator As String Implements iHistorable.Creator
    Public Property CreationDate As CalendarDate Implements iHistorable.CreationDate
    Public Function HistoryReport() As String Implements iHistorable.HistoryReport
        Return "Created by " & Creator & " at " & CreationDate.ToStringShort
    End Function
    Public Sub ConsoleReport()
        Console.WriteLine(Name)
        Console.WriteLine("└ History: " & Creator & ", " & CreationDate.ToStringShort)
    End Sub
End Class
