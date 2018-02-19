Public Interface iModifiable
    Property ModifierList As List(Of Modifier)
    Function GetModifier(ByVal quality As String) As Integer
    Function GetModifiers(ByVal category As String) As List(Of Modifier)
    Sub AddModifier(ByVal m As Modifier)
    Sub RemoveModifier(ByVal m As Modifier)
    Sub TickModifier()
End Interface
