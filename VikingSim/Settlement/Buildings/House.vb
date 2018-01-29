Public Class House
    Inherits Building

#Region "Constructors"
    Private Shared HouseNumber As Integer = 1
    Public Shared Function Import(ByVal targetName As String) As House
        Dim data As List(Of String) = IO.ImportSquareBracketSelect(IO.sbHouses, targetName)
        If data Is Nothing Then Throw New Exception("House type not found")

        Dim h As New House
        With h
            ._Name = targetName & " #" & HouseNumber
            HouseNumber += 1

            For Each line In data
                Dim ln As String() = line.Split(":")
                Dim header As String = ln(0).Trim
                Dim entry As String = ln(1).Trim
                Select Case header
                    Case "Capacity" : .ResidentCapacity = Convert.ToInt32(entry)
                    Case "Morale" : .Morale = Convert.ToInt32(entry)
                    Case Else : .BaseImport(header, entry)
                End Select
            Next
        End With
        Return h
    End Function
#End Region

#Region "Residents"
    Private ResidentCapacity As Integer = 5
    Private Residents As New List(Of Person)
    Public Function GetResident(ByVal name As String) As Person
        For Each r In Residents
            If r.Name = name Then Return r
        Next
        Return Nothing
    End Function
    Public Function GetResidents(ByVal name As String, Optional ByVal flags As String = "") As List(Of Person)
        Dim total As New List(Of Person)
        For Each r In Residents
            If name = "" OrElse r.Name = name Then
                If r.CheckFlags(flags) = True Then total.Add(r)
            End If
        Next
        Return total
    End Function
    Public Sub AddResident(ByVal p As Person)
        If Residents.Contains(p) = True Then Exit Sub

        Residents.Add(p)
    End Sub
    Public Function AddResidentcheck(ByVal p As Person) As Boolean
        If Residents.Count + 1 > ResidentCapacity Then Return False
        If p Is Nothing = False Then
            'person specific checks
        End If

        Return True
    End Function
    Public Sub RemoveResident(ByVal p As Person)
        If Residents.Contains(p) = False Then Exit Sub

        Residents.Remove(p)
    End Sub

    Private Morale As Integer

    Public Overrides Sub Tick()
        For Each r In Residents
            r.Tick()
        Next
    End Sub
#End Region

    Public Overrides Sub ConsoleReport()
        Console.WriteLine(Name)
        Console.WriteLine("└ History:   " & Creator & " in " & CreationDate.ToStringShort)
        Console.WriteLine("└ Residents: " & Residents.Count & "/" & ResidentCapacity)
        For Each r In Residents
            Console.WriteLine("  └ " & r.Name)
        Next
        Console.WriteLine()
    End Sub
    Public Overrides Function ToString() As String
        Return _Name & " - " & Residents.Count & "/" & ResidentCapacity
    End Function
End Class
