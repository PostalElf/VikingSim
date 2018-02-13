Public Interface iModifiable
    Property ModifierList As List(Of Modifier)
    Sub RemoveModifier(ByVal m As Modifier)
    Sub TickModifier()
End Interface
