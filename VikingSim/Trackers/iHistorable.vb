Public Interface iHistorable
    Property CreationDate As CalendarDate
    Property CreatorName As String
    Function HistoryReport() As String
End Interface
