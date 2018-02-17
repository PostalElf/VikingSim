Public Interface iHistorable
    Property CreationDate As CalendarDate
    Property CreatorName As String
    Sub SetHistory(ByVal creator As String, ByVal createdate As CalendarDate)
    Function HistoryReport() As String
End Interface
