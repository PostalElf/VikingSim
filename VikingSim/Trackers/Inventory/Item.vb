Public MustInherit Class Item
    Implements iHistorable

    Protected Sub BaseImport(ByVal header As String, ByVal entry As String)
        Select Case header

        End Select
    End Sub

    Public Name As String
    Public Property CreatorName As String Implements iHistorable.CreatorName
    Public Property CreationDate As CalendarDate Implements iHistorable.CreationDate
    Public Sub SetHistory(ByVal cr As Workplace, ByVal crdate As CalendarDate)
        If cr Is Nothing Then CreatorName = "Odinsson" Else CreatorName = cr.GetBestWorker.Name
        CreationDate = New CalendarDate(crdate)
    End Sub
    Public Function HistoryReport() As String Implements iHistorable.HistoryReport
        Return "Created by " & CreatorName & " at " & CreationDate.ToStringShort
    End Function
    Public Sub ConsoleReport()
        Console.WriteLine(Name)
        Console.WriteLine("└ History: " & CreatorName & ", " & CreationDate.ToStringShort)
    End Sub

    Public MustOverride Function GetBonus(ByVal k As String) As Integer
End Class
