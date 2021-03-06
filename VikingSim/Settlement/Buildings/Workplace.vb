﻿Public MustInherit Class Workplace
    Inherits Building

#Region "Constructors"
    Protected Sub WorkplaceImport(ByVal header As String, ByVal entry As String)
        Select Case header
            Case "Occupation" : _Occupation = StringToEnum(Of Skill)(entry)
            Case "Capacity" : WorkerCapacity = Convert.ToInt32(entry)
            Case "Efficiency"
                Dim ds As String() = entry.Split("/")
                For n = 0 To ds.Count - 1
                    LabourPerWorker.Add(n, ds(n))
                Next
            Case Else : BaseParsedLoad(header, entry)
        End Select
    End Sub
#End Region

#Region "Workers"
    Protected Workers As New List(Of Person)
    Protected WorkerCapacity As Integer
    Protected _Occupation As Skill
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
        If Workers.Count = 0 Then Apprentices.Clear()
    End Sub
    Public Function GetBestWorker() As Person
        Dim bestWorker As Person = Nothing
        Dim bestSkill As Integer = -1
        For Each w In Workers
            If w.SkillRank(Occupation) > bestSkill Then
                bestWorker = w
                bestSkill = w.SkillRank(Occupation)
            End If
        Next
        Return bestWorker
    End Function

    Protected Apprentices As New List(Of Person)
    Protected ApprenticeCapacity As Integer = 1
    Public Sub AddApprentice(ByVal p As Person)
        Apprentices.Add(p)
    End Sub
    Public Function AddApprenticeCheck(ByVal p As Person) As Boolean
        If p.CheckFlags("apprenticable") = False Then Return False
        If Apprentices.Count + 1 > ApprenticeCapacity Then Return False
        If Workers.Count = 0 Then Return False

        Return True
    End Function
    Public Sub RemoveApprentice(ByVal p As Person)
        If Apprentices.Contains(p) = False Then Exit Sub
        Apprentices.Remove(p)
    End Sub

    Protected LabourPerWorker As New Dictionary(Of Integer, Integer)
    Protected ProductionCosts As New ResourceDict
#End Region

    Public Overrides Sub ConsoleReport()
        Console.WriteLine(Name)
        Console.WriteLine("└ Made By:     " & CreatorName & " in " & CreationDate.ToStringShort)
        Console.Write("└ Employees:   " & Workers.Count & "/" & WorkerCapacity)
        If Apprentices.Count > 0 Then Console.Write(" +" & Apprentices.Count)
        Console.WriteLine()
        For Each r In Workers
            Console.WriteLine("  └ " & r.Name)
        Next
        If Apprentices.Count > 0 Then
            For Each a In Apprentices
                Console.WriteLine("  └ " & a.Name & " [A]")
            Next
        End If
    End Sub
End Class
