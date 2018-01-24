Public Class Settlement
    Public Sub New()
        If AllResources.Count = 0 Then AllResources = ImportSquareBracketList("data/resources.txt")
        For Each resCategory In AllResources.Keys
            For Each r In AllResources(resCategory)
                Resources.Add(r, 0)
            Next
        Next
    End Sub
    Private Shared AllResources As New Dictionary(Of String, List(Of String))

    Public Function GetResidents(ByVal name As String, Optional ByVal flags As String = "")
        Dim total As New List(Of Person)
        For Each h In Houses
            total.AddRange(h.GetResidents(name, flags))
        Next
        Return total
    End Function
    Public Function GetSingleCouple() As List(Of Person)
        Dim girls As List(Of Person) = GetResidents("", "single female")
        If girls.Count = 0 Then Return Nothing
        Dim boys As List(Of Person) = GetResidents("", "single male")
        If boys.Count = 0 Then Return Nothing

        Dim pair As New List(Of Person)
        pair.Add(GetRandom(Of Person)(boys))
        pair.Add(GetRandom(Of Person)(girls))
        Return pair
    End Function

    Private Buildings As New List(Of Building)
    Private ReadOnly Property Houses As List(Of House)
        Get
            Dim total As New List(Of House)
            For Each b In Buildings
                If TypeOf b Is House Then total.Add(CType(b, House))
            Next
            Return total
        End Get
    End Property
    Public Sub AddBuilding(ByVal b As Building)
        Buildings.Add(b)
        b.settlement = Me
    End Sub

    Private Resources As New Dictionary(Of String, Integer)
    Public Sub AddResources(ByVal res As Dictionary(Of String, Integer), Optional ByVal remove As Boolean = False)
        For Each r In res.Keys
            If remove = True Then Resources(r) -= res(r) Else Resources(r) += res(r)
        Next
    End Sub
    Public Sub AddResources(ByVal r As String, ByVal qty As Integer)
        Resources(r) += qty
    End Sub
    Public Function CheckResources(ByVal res As Dictionary(Of String, Integer)) As Boolean
        For Each r In res.Keys
            If Resources(r) < res(r) Then Return False
        Next
        Return True
    End Function
End Class
