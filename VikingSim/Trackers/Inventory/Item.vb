Public Class Item
    Implements iHistorable

    Public Shared Function Import(ByVal targetName As String) As Item
        Dim rawData As List(Of String) = IO.ImportSquareBracketSelect(IO.sbItems, targetName)
        Dim item As New Item
        With item
            .Name = targetName
        End With
        Return item
    End Function

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
End Class
