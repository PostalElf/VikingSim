Public Class Alert
    Public Ref As Object
    Public RefType As Type
    Public Report As String
    Public Priority As Integer

    Public Sub New(ByVal aRef As Object, ByVal p As Integer, ByVal str As String)
        Ref = aRef
        RefType = Ref.GetType
        Priority = p
        Report = str
    End Sub
    Public Overrides Function ToString() As String
        Return "[" & Priority & "] " & Report
    End Function
End Class
