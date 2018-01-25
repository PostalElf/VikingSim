Public Class Workplace
    Inherits Building

#Region "Constructors"
    Public Shared Function Import(ByVal workplaceName As String, Optional ByVal naturalResources As NaturalResources = Nothing) As Workplace
        Dim rawData As List(Of String) = ImportSquareBracketSelect("data/buildings/workplaces.txt", workplaceName)

        Dim workplace As New Workplace
        With workplace
            .Name = workplaceName
            For Each line In rawData
                Dim entry As String() = line.Split(":")
                Dim data As String = entry(1).Trim
                Select Case entry(0).Trim
                    Case "Occupation" : ._Occupation = StringToEnum(Of Skill)(data)
                    Case "Capacity" : .WorkerCapacity = Convert.ToInt32(data)
                    Case "Labour" : .LabourThreshold = Convert.ToInt32(data)
                    Case "Efficiency"
                        Dim ds As String() = data.Split("/")
                        For n = 0 To ds.Count - 1
                            .LabourPerWorker.Add(n, ds(n))
                        Next
                    Case "Cost" : ParseAddResource(data, .ProductionCosts)
                    Case "Produce" : ParseAddResource(data, .ProductionResources)
                    Case "Resource"
                        If naturalResources Is Nothing Then Throw New Exception("No natural resources provided for " & workplaceName)
                        If naturalResources.Name <> data Then Throw New Exception("Natural resources mismatch: expected " & naturalResources.Name & " but got " & data)
                        For Each r In naturalResources.Keys
                            .ProductionResources.Add(r, naturalResources(r))
                        Next
                End Select
            Next
        End With
        Return workplace
    End Function
    Private Shared Sub ParseAddResource(ByVal data As String, ByRef targetDictionary As Dictionary(Of String, Integer))
        Dim ds As String() = data.Split(",")
        Dim resType As String = ds(0).Trim
        Dim qty As Integer = Convert.ToInt32(ds(1).Trim)

        If targetDictionary.ContainsKey(resType) = False Then targetDictionary.Add(resType, 0)
        targetDictionary(resType) += qty
    End Sub
#End Region

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

    Private LabourThreshold As Integer
    Private LabourPerWorker As New Dictionary(Of Integer, Integer)
    Private ProductionCosts As New Dictionary(Of String, Integer)
    Private MayTakeSettlementResources As Boolean = True

    Private HasProductionCosts As Boolean
    Private Labour As Integer
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
            Settlement.AddResources(ProductionResources)

            'take next batch of costs
            HasProductionCosts = TakeProductionCosts()

            'reduce excess labour to 0 if productioncosts not met
            If HasProductionCosts = False Then Labour = 0
        End While
    End Sub
    Private Function TakeProductionCosts() As Boolean
        If ProductionCosts.Keys.Count = 0 Then
            Return True
        ElseIf MayTakeSettlementResources = True AndAlso Settlement.CheckResources(ProductionCosts) = True Then
            Settlement.AddResources(ProductionCosts, True)
            Return True
        End If
        Return False
    End Function
    Private ProductionResources As New Dictionary(Of String, Integer)
End Class
