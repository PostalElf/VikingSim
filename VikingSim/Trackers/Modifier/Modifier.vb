Public Class Modifier
    Public Shared Function Import(ByVal targetTitle As String) As Modifier
        Dim rawData As List(Of String) = IO.ImportSquareBracketSelect(IO.sbDiseases, targetTitle)
        If rawData Is Nothing Then rawData = IO.ImportSquareBracketSelect(IO.sbModifiers, targetTitle)

        Dim m As New Modifier
        With m
            .Title = targetTitle
            For Each line In rawData
                Dim ls As String() = line.Split(":")
                Dim header As String = ls(0).Trim
                Dim entry As String = ls(1).Trim
                Select Case header
                    Case "Timer"
                        If entry.Contains("-") Then
                            Dim es As String() = entry.Split("-")
                            Dim min As Integer = Convert.ToInt32(es(0))
                            Dim max As Integer = Convert.ToInt32(es(1))
                            Dim roll As Integer = rng.Next(min, max + 1)
                            .Timer = roll
                        Else
                            .Timer = Convert.ToInt32(entry)
                        End If

                    Case "Quality"
                        Dim qs As String() = entry.Split(",")
                        Dim quality As String = qs(0).Trim
                        Dim value As Integer = Convert.ToInt32(qs(1).Trim)
                        If .Qualities.ContainsKey(quality) = False Then .Qualities.Add(quality, 0)
                        .Qualities(quality) += value
                End Select
            Next
        End With
        Return m
    End Function

    Private Title As String
    Private Timer As Integer
    Private Qualities As New Dictionary(Of String, Integer)
    Private Owner As iModifiable

    Public Sub Tick()
        If Timer = -1 Then Exit Sub

        Timer -= 1
        If Timer = 0 Then
            Owner.RemoveModifier(Me)
        End If
    End Sub
    Public Shared Sub AddModifier(ByVal m As Modifier, ByVal owner As iModifiable)
        owner.ModifierList.Add(m)
        m.Owner = owner
    End Sub
    Public Shared Function GetModifier(ByVal quality As String, ByVal modifierList As List(Of Modifier))
        Dim total As Integer = 0
        For Each m In modifierList
            If m.Qualities.ContainsKey(quality) = False Then Continue For
            total += m.Qualities(quality)
        Next
        Return total
    End Function
End Class
