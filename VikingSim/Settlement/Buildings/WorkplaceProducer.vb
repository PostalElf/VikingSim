Public Class WorkplaceProducer
    Inherits Workplace

#Region "Constructors"
    Public Shared Function Import(ByVal workplaceName As String, Optional ByVal location As SettlementLocation = Nothing) As WorkplaceProducer
        Dim rawData As List(Of String) = ImportSquareBracketSelect(sbProducers, workplaceName)

        Dim workplace As New WorkplaceProducer
        With workplace
            ._Name = workplaceName
            For Each line In rawData
                Dim ln As String() = line.Split(":")
                Dim header As String = ln(0).Trim
                Dim entry As String = ln(1).Trim
                Select Case header
                    Case "Labour" : .LabourThreshold = Convert.ToInt32(entry)
                    Case "Cost" : .ProductionCosts.ParsedAdd(entry)
                    Case "Produce" : .ProducedResources.ParsedAdd(entry)
                    Case "Resource"
                        If location Is Nothing Then Throw New Exception("Location not provided when NaturalResources is required.")
                        If TypeOf location Is NaturalResources = False Then Throw New Exception("NaturalResources expected in location.")
                        Dim nr As NaturalResources = CType(location, NaturalResources)
                        For Each r In nr.ResourceDict.Keys
                            .ProducedResources.Add(r, nr.ResourceDict(r))
                        Next
                    Case Else : .WorkplaceImport(header, entry)
                End Select
            Next
        End With
        Return workplace
    End Function
    Public Overrides Function ToString() As String
        Return Name & " - " & Workers.Count & "/" & WorkerCapacity
    End Function
#End Region

#Region "Production"
    Private MayTakeSettlementResources As Boolean = True
    Private HasProductionCosts As Boolean
    Private Labour As Integer
    Private LabourThreshold As Integer
    Private Function TakeProductionCosts() As Boolean
        If ProductionCosts.Keys.Count = 0 Then
            Return True
        ElseIf MayTakeSettlementResources = True AndAlso Settlement.CheckResources(ProductionCosts) = True Then
            Settlement.AddResources(ProductionCosts, True)
            Return True
        End If
        Return False
    End Function

    Public Overrides Sub Tick()
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
    Protected ProducedResources As New ResourceDict
#End Region
End Class
