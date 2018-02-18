Public MustInherit Class Building
    Implements iHistorable, iModifiable
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

    Public Sub SetHistory(ByVal cr As String, ByVal crdate As CalendarDate) Implements iHistorable.SetHistory
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
            Select Case f.ToLower.Trim
                Case "house" : If TypeOf Me Is House = False Then Return False
                Case "space", "hasspace" : If TypeOf Me Is House AndAlso CType(Me, House).AddResidentcheck(Nothing) = False Then Return False
                Case "nospace" : If TypeOf Me Is House AndAlso CType(Me, House).AddResidentcheck(Nothing) = True Then Return False

                Case "workplace" : If TypeOf Me Is Workplace = False Then Return False
                Case "producer" : If TypeOf Me Is WorkplaceProducer = False Then Return False
                Case "projector" : If TypeOf Me Is WorkplaceProjector = False Then Return False
                Case "storage" : If TypeOf Me Is Storage = False Then Return False
                Case "employed" : If TypeOf Me Is Workplace = False OrElse CType(Me, Workplace).GetBestWorker Is Nothing Then Return False
                Case "employable" : If TypeOf Me Is Workplace = False OrElse CType(Me, Workplace).AddWorkerCheck(Nothing) = False Then Return False
                Case "unemployable" : If TypeOf Me Is Workplace = False OrElse CType(Me, Workplace).AddWorkerCheck(Nothing) = True Then Return False

                Case Else : If CheckFlagAdvanced(f) = False Then Return False
            End Select
        Next
        Return True
    End Function
    Private Function CheckFlagAdvanced(ByVal flag As String) As Boolean
        'advanced flags use = to specify search terms
        'in the flag, link spaces with +, eg. name=ash+woodcutter

        If flag.Contains("=") = False Then Return False

        Dim fsplit As String() = flag.ToLower.Split("=")
        Dim header As String = fsplit(0)
        Dim entry As String = fsplit(1).Replace("+", " ")
        Select Case header
            Case "name"
                If Name.ToLower = entry Then Return True
                If Name.Contains("#") AndAlso Name.ToLower.Contains(entry) Then Return True
        End Select

        Return False
    End Function

    Public MustOverride Sub ConsoleReport()
#End Region

#Region "Land"
    Public Settlement As Settlement
    Private _LandUsed As Integer = 1
    Public ReadOnly Property LandUsed As Integer
        Get
            Return _LandUsed
        End Get
    End Property
#End Region

#Region "Modifiers"
    Private Property ModifierList As New List(Of Modifier) Implements iModifiable.ModifierList
    Private Sub RemoveModifier(ByVal m As Modifier) Implements iModifiable.RemoveModifier
        If ModifierList.Contains(m) = False Then Exit Sub
        ModifierList.Remove(m)
    End Sub
    Private Sub TickModifier() Implements iModifiable.TickModifier
        For n = ModifierList.Count - 1 To 0 Step -1
            Dim m As Modifier = ModifierList(n)
            m.Tick()
        Next
    End Sub
#End Region

    Public Inventory As New Inventory

    Public Overridable Sub Tick()
        TickModifier()
    End Sub
    Public MustOverride Function GetTickWarnings() As List(Of Alert)
End Class
