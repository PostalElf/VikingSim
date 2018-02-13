Public Class WorkplaceProducer
    Inherits Workplace

#Region "Constructors"
    Public Shared Function Import(ByVal workplaceName As String) As WorkplaceProducer
        Dim rawData As List(Of String) = IO.ImportSquareBracketSelect(IO.sbProducers, workplaceName)

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
        MyBase.Tick()

        'attempt to take production costs if possible, otherwise exit sub
        If HasProductionCosts = False Then
            If TakeProductionCosts() = False Then Exit Sub
            HasProductionCosts = True
        End If

        'add labour
        For Each p In Workers
            Labour += LabourPerWorker(p.PerformWork) + p.GetInventoryBonus(Occupation)
        Next

        'check if labour threshold met
        While Labour >= LabourThreshold
            'reduce labour and add product
            Labour -= LabourThreshold
            Settlement.AddResources(ProducedResources)
            World.AddAlert(Me, 2, Name & " has completed production: " & ProducedResources.ToString)

            'take next batch of costs
            HasProductionCosts = TakeProductionCosts()
            If HasProductionCosts = False Then World.AddAlert(Me, 2, Name & " does not have enough resources to continue production.")

            'reduce excess labour to 0 if productioncosts not met
            If HasProductionCosts = False Then Labour = 0
        End While
    End Sub
    Protected ProducedResources As New ResourceDict
#End Region

    Public Overrides Sub ConsoleReport()
        Console.WriteLine(Name)
        Console.WriteLine("└ Made By:   " & CreatorName & " in " & CreationDate.ToStringShort)
        If ProductionCosts.count > 0 Then Console.WriteLine("└ Cost:      " & ProductionCosts.ToString)
        If ProducedResources.Count > 0 Then Console.WriteLine("└ Produces:  " & ProducedResources.ToString)
        Console.WriteLine("└ Employees: " & Workers.Count & "/" & WorkerCapacity)
        For Each r In Workers
            Console.WriteLine("  └ " & r.Name)
        Next
        Console.WriteLine()
    End Sub
End Class
