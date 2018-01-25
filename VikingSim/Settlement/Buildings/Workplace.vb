Public Class Workplace
    Inherits Building

#Region "Constructors"
    Public Shared Function Import(ByVal workplaceName As String, Optional ByVal naturalResources As NaturalResources = Nothing) As Workplace
        Dim rawData As List(Of String) = ImportSquareBracketSelect("data/buildings/workplaces.txt", workplaceName)

        Dim workplace As New Workplace
        With workplace
            ._Name = workplaceName
            For Each line In rawData
                Dim ln As String() = line.Split(":")
                Dim header As String = ln(0).Trim
                Dim entry As String = ln(1).Trim
                Select Case header
                    Case "Occupation" : ._Occupation = StringToEnum(Of Skill)(entry)
                    Case "Capacity" : .WorkerCapacity = Convert.ToInt32(entry)
                    Case "Labour" : .LabourThreshold = Convert.ToInt32(entry)
                    Case "Efficiency"
                        Dim ds As String() = entry.Split("/")
                        For n = 0 To ds.Count - 1
                            .LabourPerWorker.Add(n, ds(n))
                        Next
                    Case "Cost" : .ProductionCosts.ParsedAdd(entry)
                    Case "Produce" : .ProducedResources.ParsedAdd(entry)
                    Case "Resource"
                        If naturalResources Is Nothing Then Throw New Exception("No natural resources provided for " & workplaceName)
                        If naturalResources.Name <> entry Then Throw New Exception("Natural resources mismatch: expected " & naturalResources.Name & " but got " & entry)
                        For Each r In naturalResources.Keys
                            .ProducedResources.Add(r, naturalResources(r))
                        Next
                    Case Else : .BaseImport(header, entry)
                End Select
            Next
        End With
        Return workplace
    End Function
#End Region

#Region "Workers"
    Private Workers As New List(Of Person)
    Private WorkerCapacity As Integer
    Private _Occupation As Skill
    Public ReadOnly Property Occupation As Skill
        Get
            Return _Occupation
        End Get
    End Property
    Public Sub AddWorker(ByVal p As Person)
        If Workers.Contains(p) Then Exit Sub

        Workers.Add(p)
    End Sub
    Public Function AddWorkerCheck(ByVal p As Person) As Boolean
        If Workers.Count + 1 > WorkerCapacity Then Return False
        If p Is Nothing = False Then
            'worker specific checks go here
        End If

        Return True
    End Function
    Public Sub RemoveWorker(ByVal p As Person)
        If Workers.Contains(p) = False Then Exit Sub

        Workers.Remove(p)
    End Sub

    Private Labour As Integer
    Private LabourThreshold As Integer
    Private LabourPerWorker As New Dictionary(Of Integer, Integer)
    Private ProductionCosts As New ResourceDict
    Private ProducedResources As New ResourceDict

    Public Sub Tick()
        'attempt to take production costs if possible, otherwise exit sub
        If HasProductionCosts = False Then
            If TakeProductionCosts() = False Then Exit Sub
            HasProductionCosts = True
        End If

        'add labour
        For Each p In Workers
            Labour += LabourPerWorker(p.PerformWork)
        Next

        'check if labour threshold met
        While Labour >= LabourThreshold
            'reduce labour and add product
            Labour -= LabourThreshold
            Settlement.AddResources(ProducedResources)

            'take next batch of costs
            HasProductionCosts = TakeProductionCosts()

            'reduce excess labour to 0 if productioncosts not met
            If HasProductionCosts = False Then Labour = 0
        End While
    End Sub
#End Region

#Region "Production"
    Private MayTakeSettlementResources As Boolean = True
    Private HasProductionCosts As Boolean
    Private Function TakeProductionCosts() As Boolean
        If ProductionCosts.Keys.Count = 0 Then
            Return True
        ElseIf MayTakeSettlementResources = True AndAlso Settlement.CheckResources(ProductionCosts) = True Then
            Settlement.AddResources(ProductionCosts, True)
            Return True
        End If
        Return False
    End Function
#End Region
End Class
