Public Interface iHistorable
    Property CreationDate As CalendarDate
    Property Creator As String
    Function HistoryReport() As String
End Interface
