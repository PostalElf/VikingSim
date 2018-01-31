Public MustInherit Class Project
    Protected Sub BaseImport(ByVal header As String, ByVal entry As String)
        Select Case header
            Case "Buildtime", "Time" : BuildTime = Convert.ToInt32(entry)
            Case "Buildcost", "Build" : ConstructionCosts.ParsedAdd(entry)
            Case "Buildtype", "Type" : BuildType = entry
        End Select
    End Sub

    Public Creator As Workplace
    Protected Name As String
    Protected BuildType As String
    Protected BuildTime As Integer
    Protected BuildTimeProgress As Integer
    Protected ConstructionCosts As New ResourceDict

    Public Function CheckBuildType(ByVal type As String) As Boolean
        Return BuildType = type
    End Function
    Public Function CheckBuildCost(ByVal settlement As Settlement) As Boolean
        Return settlement.CheckResources(ConstructionCosts)
    End Function
    Public Sub PayCost(ByVal settlement As Settlement)
        settlement.AddResources(ConstructionCosts, True)
    End Sub

    Public Function Tick(ByVal progress As Integer) As Boolean
        'return true when completed
        BuildTimeProgress += progress
        If BuildTimeProgress >= BuildTime Then Return True

        Return False
    End Function
    Public MustOverride Function Unpack()
    Public Overrides Function ToString() As String
        Dim buildTimePercent As Integer = BuildTimeProgress / BuildTime * 100
        Return Name & " (" & buildTimePercent & "%)"
    End Function
End Class
