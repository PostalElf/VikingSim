﻿Public Class World
    Public Shared TimeNow As New CalendarDate(1, 1, 1)

    Private Settlements As New List(Of Settlement)

    Public Sub Tick()
        TimeNow.Tick()

        For Each Settlement In Settlements
            Settlement.tick()
        Next
    End Sub
End Class