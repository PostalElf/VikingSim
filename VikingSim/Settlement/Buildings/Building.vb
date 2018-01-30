﻿Public MustInherit Class Building
    Implements iHistorable
    Protected Sub BaseImport(ByVal header As String, ByVal entry As String)
        Select Case header
            Case "Land", "LandUsed" : _LandUsed = Convert.ToInt32(entry)
        End Select
    End Sub

#Region "Personal Identifiers"
    Protected _Name As String
    Public ReadOnly Property Name As String
        Get
            Return _Name
        End Get
    End Property

    Public Sub SetHistory(ByVal cr As String, ByVal crdate As CalendarDate)
        CreatorName = cr
        CreationDate = New CalendarDate(crdate)
    End Sub
    Public Sub SetHistory(ByVal cr As Workplace, ByVal crdate As CalendarDate)
        If cr Is Nothing Then CreatorName = "Odinsson" Else CreatorName = cr.GetBestWorker.Name
        CreationDate = New CalendarDate(crdate)
    End Sub
    Protected Property CreatorName As String Implements iHistorable.CreatorName
    Protected Property CreationDate As CalendarDate Implements iHistorable.CreationDate
    Protected Function HistoryReport() As String Implements iHistorable.HistoryReport
        Return "Created by " & CreatorName & " at " & CreationDate.ToStringShort
    End Function

    Public Function CheckFlags(ByVal flags As String) As Boolean
        Dim fs As String() = flags.Split(" ")
        For Each f In fs
            Select Case f.ToLower
                Case "house" : If TypeOf Me Is House = False Then Return False
                Case "space", "hasspace" : If TypeOf Me Is House AndAlso CType(Me, House).AddResidentcheck(Nothing) = False Then Return False
                Case "nospace" : If TypeOf Me Is House AndAlso CType(Me, House).AddResidentcheck(Nothing) = True Then Return False

                Case "workplace" : If TypeOf Me Is Workplace = False Then Return False
                Case "producer" : If TypeOf Me Is WorkplaceProducer = False Then Return False
                Case "projector" : If TypeOf Me Is WorkplaceProjector = False Then Return False
                Case "employed" : If TypeOf Me Is Workplace = False OrElse CType(Me, Workplace).GetBestWorker Is Nothing Then Return False
                Case "employable" : If TypeOf Me Is Workplace = False OrElse CType(Me, Workplace).AddWorkerCheck(Nothing) = False Then Return False
                Case "unemployable" : If TypeOf Me Is Workplace = False OrElse CType(Me, Workplace).AddWorkerCheck(Nothing) = True Then Return False

                Case "storage" : If TypeOf Me Is Storage = False Then Return False
            End Select
        Next
        Return True
    End Function

    Public MustOverride Sub ConsoleReport()
#End Region

#Region "Land"
    Public Settlement As Settlement
    Private _LandUsed As Integer = 5
    Public ReadOnly Property LandUsed As Integer
        Get
            Return _LandUsed
        End Get
    End Property
#End Region

    Public Inventory As New Inventory

    Public MustOverride Sub Tick()
End Class
