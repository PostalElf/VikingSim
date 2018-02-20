Public Structure Sex
    Public Sub New(ByVal s As String)
        _Sex = s
    End Sub
    Private _Sex As String

    Public ReadOnly Property First As String
        Get
            Return _Sex.First
        End Get
    End Property
    Public ReadOnly Property Pronoun As String
        Get
            Select Case _Sex
                Case "Male" : Return "He"
                Case "Female" : Return "She"
                Case Else : Return "It"
            End Select
        End Get
    End Property
    Public ReadOnly Property PronounPossessive As String
        Get
            Select Case _Sex
                Case "Male" : Return "His"
                Case "Female" : Return "Her"
                Case Else : Return "Its"
            End Select
        End Get
    End Property
    Public Shared Operator =(ByVal s As Sex, ByVal value As String) As Boolean
        If s._Sex = value Then Return True Else Return False
    End Operator
    Public Shared Operator <>(ByVal s As Sex, ByVal value As String) As Boolean
        Return Not (s = value)
    End Operator
    Public Shared Operator =(ByVal s1 As Sex, ByVal s2 As Sex) As Boolean
        If s1._Sex = s2._Sex Then Return True Else Return False
    End Operator
    Public Shared Operator <>(ByVal s1 As Sex, ByVal s2 As Sex) As Boolean
        Return Not (s1 = s2)
    End Operator
End Structure
