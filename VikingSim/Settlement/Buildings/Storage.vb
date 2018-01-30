﻿Public Class Storage
    Inherits Building

    Public Shared Function Import(ByVal targetName As String) As Storage
        Dim rawData As List(Of String) = IO.ImportSquareBracketSelect(IO.sbStorage, targetName)
        If rawData Is Nothing Then Return Nothing

        Dim s As New Storage
        With s
            For Each line In rawData
                Dim split As String() = line.Split(":")
                Dim header As String = split(0).Trim
                Dim entry As String = split(1).Trim

                Select Case header
                    Case "Storage" : .Capacity.ParsedAdd(entry)
                    Case Else : .BaseImport(header, entry)
                End Select
            Next
        End With
        Return s
    End Function

    Private Capacity As New ResourceDict
    Public Function GetCapacity(ByVal r As String) As Integer
        If Capacity.ContainsKey(r) = False Then Return 0
        Return Capacity(r)
    End Function

    Public Overrides Sub ConsoleReport()
        Console.WriteLine(Name)
        Console.WriteLine("└ History: " & CreatorName & " in " & CreationDate.ToStringShort)
        For Each s In Capacity.Keys
            Console.WriteLine("└ Storage: " & s & " " & Capacity(s))
        Next
        Console.WriteLine()
    End Sub
    Public Overrides Sub Tick()
        'storage doesn't need to do shit during ticks
    End Sub
End Class
