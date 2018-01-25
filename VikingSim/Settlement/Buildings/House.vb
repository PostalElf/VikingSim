﻿Public Class House
    Inherits Building

#Region "Constructors"
    Private Shared HouseNumber As Integer = 1
    Public Shared Function Import(ByVal targetName As String) As House
        Dim data As List(Of String) = ImportSquareBracketSelect("data/buildings/houses.txt", targetName)
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
                Dim failedSelection As Boolean = False
                Dim fs As String() = flags.Split(" ")
                For Each f In fs
                    Select Case f.ToLower
                        Case "single", "unmarried" : If r.GetRelative("spouse") Is Nothing = False Then failedSelection = True
                        Case "married" : If r.GetRelative("spouse") Is Nothing Then failedSelection = True
                        Case "male", "men" : If r.Sex <> "Male" Then failedSelection = True
                        Case "female", "women" : If r.Sex <> "Female" Then failedSelection = True
                        Case "employed" : If r.Occupation = Skill.Vagrant Then failedSelection = True
                        Case "unemployed" : If r.Occupation <> Skill.Vagrant Then failedSelection = True
                    End Select
                    If failedSelection = True Then Exit For
                Next
                If failedSelection = True Then Continue For
                total.Add(r)
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
#End Region

    Private Morale As Integer

    Public Overrides Function ToString() As String
        Return _Name & " - " & Residents.Count & "/" & ResidentCapacity
    End Function
End Class