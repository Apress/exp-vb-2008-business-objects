Imports System
Imports Csla.Serialization.Mobile

''' <summary>
''' Provides a date data type that understands the concept
''' of an empty date value.
''' </summary>
''' <remarks>
''' See Chapter 5 for a full discussion of the need for this
''' data type and the design choices behind it.
''' </remarks>
<Serializable()> _
<System.ComponentModel.TypeConverter(GetType(Csla.Core.TypeConverters.SmartDateConverter))> _
Public Structure SmartDate

  Implements Core.ISmartField
  Implements IComparable
  Implements IConvertible
  Implements IFormattable
  Implements IMobileObject

  Private _date As Date
  Private _initialized As Boolean
  Private _emptyValue As EmptyValue
  Private _format As String
  Private Shared _defaultFormat As String


#Region " EmptyValue enum "

  ''' <summary>
  ''' Indicates the empty value of a
  ''' SmartDate.
  ''' </summary>
  Public Enum EmptyValue
    ''' <summary>
    ''' Indicates that an empty SmartDate
    ''' is the smallest date.
    ''' </summary>
    MinDate
    ''' <summary>
    ''' Indicates that an empty SmartDate
    ''' is the largest date.
    ''' </summary>
    MaxDate
  End Enum

#End Region

#Region " Constructors "

  Shared Sub New()
    _defaultFormat = "d"
  End Sub

  ''' <summary>
  ''' Creates a new SmartDate object.
  ''' </summary>
  ''' <param name="emptyIsMin">Indicates whether an empty date is the min or max date value.</param>
  Public Sub New(ByVal emptyIsMin As Boolean)
    _emptyValue = GetEmptyValue(emptyIsMin)
    _format = Nothing
    _initialized = False
    'provide a dummy value to allow real initialization
    _date = DateTime.MinValue
    SetEmptyDate(_emptyValue)
  End Sub

  ''' <summary>
  ''' Creates a new SmartDate object.
  ''' </summary>
  ''' <param name="emptyValue">Indicates whether an empty date is the min or max date value.</param>
  Public Sub New(ByVal emptyValue As EmptyValue)
    _emptyValue = emptyValue
    _format = Nothing
    _initialized = False
    'provide a dummy value to allow real initialization
    _date = DateTime.MinValue
    SetEmptyDate(_emptyValue)
  End Sub

  ''' <summary>
  ''' Creates a new SmartDate object.
  ''' </summary>
  ''' <remarks>
  ''' The SmartDate created will use the min possible
  ''' date to represent an empty date.
  ''' </remarks>
  ''' <param name="value">The initial value of the object.</param>
  Public Sub New(ByVal value As Date)
    _emptyValue = Csla.SmartDate.EmptyValue.MinDate
    _format = Nothing
    _initialized = False
    _date = DateTime.MinValue
    Me.Date = value
  End Sub

  ''' <summary>
  ''' Creates a new SmartDate object.
  ''' </summary>
  ''' <param name="value">The initial value of the object.</param>
  ''' <param name="emptyIsMin">Indicates whether an empty date is the min or max date value.</param>
  Public Sub New(ByVal value As Date, ByVal emptyIsMin As Boolean)
    _emptyValue = GetEmptyValue(emptyIsMin)
    _format = Nothing
    _initialized = False
    _date = DateTime.MinValue
    Me.Date = value
  End Sub

  ''' <summary>
  ''' Creates a new SmartDate object.
  ''' </summary>
  ''' <param name="value">The initial value of the object.</param>
  ''' <param name="emptyValue">Indicates whether an empty date is the min or max date value.</param>
  Public Sub New(ByVal value As Date, ByVal emptyValue As EmptyValue)
    _emptyValue = emptyValue
    _format = Nothing
    _initialized = False
    _date = DateTime.MinValue
    Me.Date = value
  End Sub

  ''' <summary>
  ''' Creates a new SmartDate object.
  ''' </summary>
  ''' <param name="value">The initial value of the object.</param>
  ''' <param name="emptyValue">Indicates whether an empty date is the min or max date value.</param>
  ''' <param name="kind">One of the DateTimeKind values.</param>
  Public Sub New(ByVal value As Date, ByVal emptyValue As EmptyValue, ByVal kind As DateTimeKind)
    _emptyValue = emptyValue
    _format = Nothing
    _initialized = False
    _date = DateTime.MinValue
    Me.Date = DateTime.SpecifyKind(value, kind)
  End Sub

  ''' <summary>
  ''' Creates a new SmartDate object.
  ''' </summary>
  ''' <remarks>
  ''' The SmartDate created will use the min possible
  ''' date to represent an empty date.
  ''' </remarks>
  ''' <param name="value">The initial value of the object.</param>
  Public Sub New(ByVal value As Date?)
    _emptyValue = Csla.SmartDate.EmptyValue.MinDate
    _format = Nothing
    _initialized = False
    _date = DateTime.MinValue
    If value.HasValue Then
      Me.Date = value.Value
    End If
  End Sub

  ''' <summary>
  ''' Creates a new SmartDate object.
  ''' </summary>
  ''' <param name="value">The initial value of the object.</param>
  ''' <param name="emptyIsMin">Indicates whether an empty date is the min or max date value.</param>
  Public Sub New(ByVal value As Date?, ByVal emptyIsMin As Boolean)
    _emptyValue = GetEmptyValue(emptyIsMin)
    _format = Nothing
    _initialized = False
    _date = DateTime.MinValue
    If value.HasValue Then
      Me.Date = value.Value
    End If
  End Sub

  ''' <summary>
  ''' Creates a new SmartDate object.
  ''' </summary>
  ''' <param name="value">The initial value of the object.</param>
  ''' <param name="emptyValue">Indicates whether an empty date is the min or max date value.</param>
  Public Sub New(ByVal value As Date?, ByVal emptyValue As EmptyValue)
    _emptyValue = emptyValue
    _format = Nothing
    _initialized = False
    _date = DateTime.MinValue
    If value.HasValue Then
      Me.Date = value.Value
    End If
  End Sub

  ''' <summary>
  ''' Creates a new SmartDate object.
  ''' </summary>
  ''' <remarks>
  ''' <para>
  ''' The SmartDate created will use the min possible
  ''' date to represent an empty date.
  ''' </para><para>
  ''' SmartDate maintains the date value as a DateTime,
  ''' so the provided DateTimeOffset is converted to a
  ''' DateTime in this constructor. You should be aware
  ''' that this can lead to a loss of precision in
  ''' some cases.
  ''' </para>
  ''' </remarks>
  ''' <param name="value">The initial value of the object.</param>
  Public Sub New(ByVal value As DateTimeOffset)
    _emptyValue = Csla.SmartDate.EmptyValue.MinDate
    _format = Nothing
    _initialized = False
    _date = DateTime.MinValue
    Me.Date = value.DateTime
  End Sub

  ''' <summary>
  ''' Creates a new SmartDate object.
  ''' </summary>
  ''' <param name="value">The initial value of the object.</param>
  ''' <param name="emptyIsMin">Indicates whether an empty date is the min or max date value.</param>
  ''' <remarks>
  ''' SmartDate maintains the date value as a DateTime,
  ''' so the provided DateTimeOffset is converted to a
  ''' DateTime in this constructor. You should be aware
  ''' that this can lead to a loss of precision in
  ''' some cases.
  ''' </remarks>
  Public Sub New(ByVal value As DateTimeOffset, ByVal emptyIsMin As Boolean)
    _emptyValue = GetEmptyValue(emptyIsMin)
    _format = Nothing
    _initialized = False
    _date = DateTime.MinValue
    Me.Date = value.DateTime
  End Sub

  ''' <summary>
  ''' Creates a new SmartDate object.
  ''' </summary>
  ''' <param name="value">The initial value of the object.</param>
  ''' <param name="emptyValue">Indicates whether an empty date is the min or max date value.</param>
  ''' <remarks>
  ''' SmartDate maintains the date value as a DateTime,
  ''' so the provided DateTimeOffset is converted to a
  ''' DateTime in this constructor. You should be aware
  ''' that this can lead to a loss of precision in
  ''' some cases.
  ''' </remarks>
  Public Sub New(ByVal value As DateTimeOffset, ByVal emptyValue As EmptyValue)
    _emptyValue = emptyValue
    _format = Nothing
    _initialized = False
    _date = DateTime.MinValue
    Me.Date = value.DateTime
  End Sub

  ''' <summary>
  ''' Creates a new SmartDate object.
  ''' </summary>
  ''' <remarks>
  ''' The SmartDate created will use the min possible
  ''' date to represent an empty date.
  ''' </remarks>
  ''' <param name="value">The initial value of the object (as text).</param>
  Public Sub New(ByVal value As String)
    _emptyValue = EmptyValue.MinDate
    _format = Nothing
    _initialized = False
    _date = DateTime.MinValue
    Me.Text = value
  End Sub

  ''' <summary>
  ''' Creates a new SmartDate object.
  ''' </summary>
  ''' <param name="value">The initial value of the object (as text).</param>
  ''' <param name="emptyIsMin">Indicates whether an empty date is the min or max date value.</param>
  Public Sub New(ByVal value As String, ByVal emptyIsMin As Boolean)
    _emptyValue = GetEmptyValue(emptyIsMin)
    _format = Nothing
    _initialized = False
    _date = DateTime.MinValue
    Me.Text = value
  End Sub

  ''' <summary>
  ''' Creates a new SmartDate object.
  ''' </summary>
  ''' <param name="value">The initial value of the object (as text).</param>
  ''' <param name="emptyValue">Indicates whether an empty date is the min or max date value.</param>
  Public Sub New(ByVal value As String, ByVal emptyValue As EmptyValue)
    _emptyValue = emptyValue
    _format = Nothing
    _initialized = False
    _date = DateTime.MinValue
    Me.Text = value
  End Sub

  Private Shared Function GetEmptyValue(ByVal emptyIsMin As Boolean) As EmptyValue
    If emptyIsMin Then
      Return EmptyValue.MinDate
    Else
      Return EmptyValue.MaxDate
    End If
  End Function

  Private Sub SetEmptyDate(ByVal emptyValue As EmptyValue)
    If emptyValue = SmartDate.EmptyValue.MinDate Then
      Me.Date = Date.MinValue

    Else
      Me.Date = Date.MaxValue
    End If
  End Sub

#End Region

#Region " Text Support "

  ''' <summary>
  ''' Sets the global default format string used by all new
  ''' SmartDate values going forward.
  ''' </summary>
  ''' <remarks>
  ''' The default global format string is "d" unless this
  ''' method is called to change that value. Existing SmartDate
  ''' values are unaffected by this method, only SmartDate
  ''' values created after calling this method are affected.
  ''' </remarks>
  ''' <param name="formatString">
  ''' The format string should follow the requirements for the
  ''' .NET System.String.Format statement.
  ''' </param>
  Public Shared Sub SetDefaultFormatString(ByVal formatString As String)
    _defaultFormat = formatString
  End Sub

  ''' <summary>
  ''' Gets or sets the format string used to format a date
  ''' value when it is returned as text.
  ''' </summary>
  ''' <remarks>
  ''' The format string should follow the requirements for the
  ''' .NET <see cref="System.String.Format"/> statement.
  ''' </remarks>
  ''' <value>A format string.</value>
  Public Property FormatString() As String
    Get
      If _format Is Nothing Then
        _format = _defaultFormat
      End If
      Return _format
    End Get
    Set(ByVal value As String)
      _format = value
    End Set
  End Property

  ''' <summary>
  ''' Gets or sets the date value.
  ''' </summary>
  ''' <remarks>
  ''' <para>
  ''' This property can be used to set the date value by passing a
  ''' text representation of the date. Any text date representation
  ''' that can be parsed by the .NET runtime is valid.
  ''' </para><para>
  ''' When the date value is retrieved via this property, the text
  ''' is formatted by using the format specified by the 
  ''' <see cref="FormatString" /> property. The default is the
  ''' short date format (d).
  ''' </para>
  ''' </remarks>
  Public Property Text() As String Implements Core.ISmartField.Text
    Get
      Return DateToString(Me.Date, FormatString, _emptyValue)
    End Get
    Set(ByVal value As String)
      Me.Date = StringToDate(value, _emptyValue)
    End Set
  End Property

#End Region

#Region " Date Support "

  ''' <summary>
  ''' Gets or sets the date value.
  ''' </summary>
  Public Property [Date]() As Date
    Get
      If Not _initialized Then
        _date = Date.MinValue
        _initialized = True
      End If
      Return _date
    End Get
    Set(ByVal value As Date)
      _date = value
      _initialized = True
    End Set
  End Property

  ''' <summary>
  ''' Gets the value as a DateTimeOffset.
  ''' </summary>
  Public Function ToDateTimeOffset() As DateTimeOffset

    Return New DateTimeOffset(_date)

  End Function

  ''' <summary>
  ''' Gets the value as a Date?.
  ''' </summary>
  Public Function ToNullableDate() As Date?

    If Me.IsEmpty Then
      Return New Date?

    Else
      Return New Date?(Me.Date)
    End If

  End Function

  ' TODO: These methods do not exist in c# version, should they be deleted?
  'Public Sub SetDate(ByVal newDate As Date)

  '  Me.Date = newDate

  'End Sub

  'Public Sub SetDate(ByVal newDate As Date?)

  '  If newDate.HasValue Then
  '    Me.Date = newDate.Value

  '  Else
  '    If _emptyValue = EmptyValue.MinDate Then
  '      Me.Date = Date.MinValue

  '    Else
  '      Me.Date = Date.MaxValue
  '    End If
  '  End If

  'End Sub

  'Public Sub SetDate(ByVal newDate As DateTimeOffset)

  '  Me.Date = newDate.DateTime

  'End Sub

#End Region

#Region " System.Object overrides "

  ''' <summary>
  ''' Returns a text representation of the date value.
  ''' </summary>
  Public Overrides Function ToString() As String
    Return Me.Text
  End Function

  ''' <summary>
  ''' Returns a text representation of the date value.
  ''' </summary>
  ''' <param name="format">
  ''' A standard .NET format string.
  ''' </param>
  Public Overloads Function ToString(ByVal format As String) As String
    If String.IsNullOrEmpty(format) Then
      Return Me.ToString
    Else
      Return DateToString(Me.Date, format, _emptyValue)
    End If
  End Function

  ''' <summary>
  ''' Compares this object to another <see cref="SmartDate"/>
  ''' for equality.
  ''' </summary>
  ''' <param name="obj">Object to compare for equality.</param>
  Public Overloads Overrides Function Equals(ByVal obj As Object) As Boolean

    If TypeOf obj Is SmartDate Then
      Dim tmp As SmartDate = DirectCast(obj, SmartDate)
      If Me.IsEmpty AndAlso tmp.IsEmpty Then
        Return True
      Else
        Return Me.Date.Equals(tmp.Date)
      End If

    ElseIf TypeOf obj Is DateTime Then
      Return Me.Date.Equals(DirectCast(obj, Date))

    ElseIf TypeOf obj Is String Then
      Return Me.CompareTo(CStr(obj)) = 0

    Else
      Return False
    End If

  End Function

  ''' <summary>
  ''' Returns a hash code for this object.
  ''' </summary>
  Public Overrides Function GetHashCode() As Integer
    Return Me.Date.GetHashCode
  End Function

#End Region

#Region " DBValue "

  ''' <summary>
  ''' Gets a database-friendly version of the date value.
  ''' </summary>
  ''' <remarks>
  ''' <para>
  ''' If the SmartDate contains an empty date, this returns <see cref="DBNull"/>.
  ''' Otherwise the actual date value is returned as type Date.
  ''' </para><para>
  ''' This property is very useful when setting parameter values for
  ''' a Command object, since it automatically stores null values into
  ''' the database for empty date values.
  ''' </para><para>
  ''' When you also use the SafeDataReader and its GetSmartDate method,
  ''' you can easily read a null value from the database back into a
  ''' SmartDate object so it remains considered as an empty date value.
  ''' </para>
  ''' </remarks>
  Public ReadOnly Property DBValue() As Object
    Get
      If Me.IsEmpty Then
        Return DBNull.Value

      Else
        Return Me.Date
      End If
    End Get
  End Property

#End Region

#Region " Empty Dates "

  ''' <summary>
  ''' Gets a value indicating whether this object contains an empty date.
  ''' </summary>
  Public ReadOnly Property IsEmpty() As Boolean Implements Core.ISmartField.IsEmpty
    Get
      If _emptyValue = EmptyValue.MinDate Then
        Return Me.Date.Equals(Date.MinValue)
      Else
        Return Me.Date.Equals(Date.MaxValue)
      End If
    End Get
  End Property

  ''' <summary>
  ''' Gets a value indicating whether an empty date is the 
  ''' min or max possible date value.
  ''' </summary>
  ''' <remarks>
  ''' Whether an empty date is considered to be the smallest or largest possible
  ''' date is only important for comparison operations. This allows you to
  ''' compare an empty date with a real date and get a meaningful result.
  ''' </remarks>
  Public ReadOnly Property EmptyIsMin() As Boolean
    Get
      Return _emptyValue = EmptyValue.MinDate
    End Get
  End Property

#End Region

#Region " Conversion Functions "

  ''' <summary>
  ''' Converts a string value into a SmartDate.
  ''' </summary>
  ''' <param name="value">String containing the date value.</param>
  ''' <returns>A new SmartDate containing the date value.</returns>
  ''' <remarks>
  ''' EmptyIsMin will default to <see langword="true"/>.
  ''' </remarks>
  Public Shared Function Parse(ByVal value As String) As SmartDate

    Return New SmartDate(value)

  End Function

  ''' <summary>
  ''' Converts a string value into a SmartDate.
  ''' </summary>
  ''' <param name="value">String containing the date value.</param>
  ''' <param name="emptyValue">Indicates whether an empty date is the min or max date value.</param>
  ''' <returns>A new SmartDate containing the date value.</returns>
  Public Shared Function Parse( _
    ByVal value As String, ByVal emptyValue As EmptyValue) As SmartDate

    Return New SmartDate(value, emptyValue)

  End Function

  ''' <summary>
  ''' Converts a string value into a SmartDate.
  ''' </summary>
  ''' <param name="value">String containing the date value.</param>
  ''' <param name="emptyIsMin">Indicates whether an empty date is the min or max date value.</param>
  ''' <returns>A new SmartDate containing the date value.</returns>
  Public Shared Function Parse( _
    ByVal value As String, ByVal emptyIsMin As Boolean) As SmartDate

    Return New SmartDate(value, emptyIsMin)

  End Function

  ''' <summary>
  ''' Converts a string value into a SmartDate.
  ''' </summary>
  ''' <param name="value">String containing the date value.</param>
  ''' <param name="result">The resulting SmartDate value if the parse was successful.</param>
  ''' <returns>A value indicating if the parse was successful.</returns>
  Public Shared Function TryParse(ByVal value As String, ByRef result As SmartDate) As Boolean

    Return TryParse(value, EmptyValue.MinDate, result)

  End Function

  ''' <summary>
  ''' Converts a string value into a SmartDate.
  ''' </summary>
  ''' <param name="value">String containing the date value.</param>
  ''' <param name="emptyValue">Indicates whether an empty date is the min or max date value.</param>
  ''' <param name="result">The resulting SmartDate value if the parse was successful.</param>
  ''' <returns>A value indicating if the parse was successful.</returns>
  Public Shared Function TryParse(ByVal value As String, ByVal emptyValue As EmptyValue, ByRef result As SmartDate) As Boolean

    Dim dateResult As DateTime = DateTime.MinValue
    If TryStringToDate(value, emptyValue, dateResult) Then
      result = New SmartDate(dateResult, emptyValue)
      Return True

    Else
      Return False
    End If

  End Function

  ''' <summary>
  ''' Converts a text date representation into a Date value.
  ''' </summary>
  ''' <remarks>
  ''' An empty string is assumed to represent an empty date. An empty date
  ''' is returned as the MinValue of the Date datatype.
  ''' </remarks>
  ''' <param name="value">The text representation of the date.</param>
  ''' <returns>A Date value.</returns>
  Public Shared Function StringToDate(ByVal value As String) As Date
    Return StringToDate(value, EmptyValue.MinDate)
  End Function

  ''' <summary>
  ''' Converts a text date representation into a Date value.
  ''' </summary>
  ''' <remarks>
  ''' An empty string is assumed to represent an empty date. An empty date
  ''' is returned as the MinValue or MaxValue of the Date datatype depending
  ''' on the EmptyIsMin parameter.
  ''' </remarks>
  ''' <param name="value">The text representation of the date.</param>
  ''' <param name="emptyIsMin">Indicates whether an empty date is the min or max date value.</param>
  ''' <returns>A Date value.</returns>
  Public Shared Function StringToDate( _
    ByVal value As String, ByVal emptyIsMin As Boolean) As Date

    Return StringToDate(value, GetEmptyValue(emptyIsMin))

  End Function

  ''' <summary>
  ''' Converts a text date representation into a Date value.
  ''' </summary>
  ''' <remarks>
  ''' An empty string is assumed to represent an empty date. An empty date
  ''' is returned as the MinValue or MaxValue of the Date datatype depending
  ''' on the EmptyIsMin parameter.
  ''' </remarks>
  ''' <param name="value">The text representation of the date.</param>
  ''' <param name="emptyValue">Indicates whether an empty date is the min or max date value.</param>
  ''' <returns>A Date value.</returns>
  Public Shared Function StringToDate( _
    ByVal value As String, ByVal emptyValue As EmptyValue) As Date

    Dim result As DateTime = DateTime.MinValue
    If TryStringToDate(value, emptyValue, result) Then
      Return result

    Else
      Throw New ArgumentException(My.Resources.StringToDateException)
    End If

  End Function

  Private Shared Function TryStringToDate(ByVal value As String, ByVal emptyValue As EmptyValue, ByRef result As DateTime) As Boolean

    If (String.IsNullOrEmpty(value)) Then
      If emptyValue = emptyValue.MinDate Then
        result = DateTime.MinValue
        Return True

      Else
        result = DateTime.MaxValue
        Return True
      End If

    ElseIf IsDate(value) Then
      result = CDate(value)
      Return True

    Else
      Select Case LCase(Trim(value))
        Case My.Resources.SmartDateT, My.Resources.SmartDateToday, "."
          result = Now
          Return True

        Case My.Resources.SmartDateY, My.Resources.SmartDateYesterday, "-"
          result = DateAdd(DateInterval.Day, -1, Now)
          Return True

        Case My.Resources.SmartDateTom, My.Resources.SmartDateTomorrow, "+"
          result = DateAdd(DateInterval.Day, 1, Now)
          Return True
      End Select
    End If

    Return False

  End Function

  ''' <summary>
  ''' Converts a date value into a text representation.
  ''' </summary>
  ''' <remarks>
  ''' The date is considered empty if it matches the min value for
  ''' the Date datatype. If the date is empty, this
  ''' method returns an empty string. Otherwise it returns the date
  ''' value formatted based on the FormatString parameter.
  ''' </remarks>
  ''' <param name="value">The date value to convert.</param>
  ''' <param name="formatString">The format string used to format the date into text.</param>
  ''' <returns>Text representation of the date value.</returns>
  Public Shared Function DateToString( _
    ByVal value As Date, ByVal formatString As String) As String

    Return DateToString(value, formatString, True)
  End Function

  ''' <summary>
  ''' Converts a date value into a text representation.
  ''' </summary>
  ''' <remarks>
  ''' Whether the date value is considered empty is determined by
  ''' the EmptyIsMin parameter value. If the date is empty, this
  ''' method returns an empty string. Otherwise it returns the date
  ''' value formatted based on the FormatString parameter.
  ''' </remarks>
  ''' <param name="value">The date value to convert.</param>
  ''' <param name="formatString">The format string used to format the date into text.</param>
  ''' <param name="emptyIsMin">Indicates whether an empty date is the min or max date value.</param>
  ''' <returns>Text representation of the date value.</returns>
  Public Shared Function DateToString( _
    ByVal value As Date, ByVal formatString As String, _
    ByVal emptyIsMin As Boolean) As String

    Return DateToString(value, formatString, GetEmptyValue(emptyIsMin))

  End Function

  ''' <summary>
  ''' Converts a date value into a text representation.
  ''' </summary>
  ''' <remarks>
  ''' Whether the date value is considered empty is determined by
  ''' the EmptyIsMin parameter value. If the date is empty, this
  ''' method returns an empty string. Otherwise it returns the date
  ''' value formatted based on the FormatString parameter.
  ''' </remarks>
  ''' <param name="value">The date value to convert.</param>
  ''' <param name="formatString">The format string used to format the date into text.</param>
  ''' <param name="emptyValue">Indicates whether an empty date is the min or max date value.</param>
  ''' <returns>Text representation of the date value.</returns>
  Public Shared Function DateToString( _
    ByVal value As Date, ByVal formatString As String, _
    ByVal emptyValue As EmptyValue) As String

    If emptyValue = emptyValue.MinDate Then
      If value = DateTime.MinValue Then
        Return String.Empty
      End If

    Else ' maxdate is empty
      If value = DateTime.MaxValue Then
        Return String.Empty
      End If
    End If
    Return String.Format("{0:" + formatString + "}", value)

  End Function

#End Region

#Region " Manipulation Functions "

  ''' <summary>
  ''' Compares one SmartDate to another.
  ''' </summary>
  ''' <remarks>
  ''' This method works the same as the <see cref="DateTime.CompareTo"/> method
  ''' on the Date datetype, with the exception that it
  ''' understands the concept of empty date values.
  ''' </remarks>
  ''' <param name="value">The date to which we are being compared.</param>
  ''' <returns>A value indicating if the comparison date is less than, equal to or greater than this date.</returns>
  Public Function CompareTo(ByVal value As SmartDate) As Integer
    If Me.IsEmpty AndAlso value.IsEmpty Then
      Return 0
    Else
      Return Me.Date.CompareTo(value.Date)
    End If
  End Function

  ''' <summary>
  ''' Compares one SmartDate to another.
  ''' </summary>
  ''' <remarks>
  ''' This method works the same as the <see cref="DateTime.CompareTo"/> method
  ''' on the Date datetype, with the exception that it
  ''' understands the concept of empty date values.
  ''' </remarks>
  ''' <param name="obj">The date to which we are being compared.</param>
  ''' <returns>A value indicating if the comparison date is less than, equal to or greater than this date.</returns>
  Public Function CompareTo(ByVal obj As Object) As Integer _
      Implements IComparable.CompareTo

    If TypeOf obj Is SmartDate Then
      Return CompareTo(DirectCast(obj, SmartDate))

    Else
      Throw New ArgumentException(My.Resources.ValueNotSmartDateException)
    End If

  End Function

  ''' <summary>
  ''' Compares a SmartDate to a text date value.
  ''' </summary>
  ''' <param name="value">The date to which we are being compared.</param>
  ''' <returns>A value indicating if the comparison date is less than, equal to or greater than this date.</returns>
  Public Function CompareTo(ByVal value As String) As Integer
    Return Me.Date.CompareTo(StringToDate(value, _emptyValue))
  End Function

  ''' <summary>
  ''' Compares a SmartDate to a date value.
  ''' </summary>
  ''' <param name="value">The date to which we are being compared.</param>
  ''' <returns>A value indicating if the comparison date is less than, equal to or greater than this date.</returns>
  ''' <remarks>
  ''' SmartDate maintains the date value as a DateTime,
  ''' so the provided DateTimeOffset is converted to a
  ''' DateTime for this comparison. You should be aware
  ''' that this can lead to a loss of precision in
  ''' some cases.
  ''' </remarks>
  Public Function CompareTo(ByVal value As DateTimeOffset) As Integer
    Return Me.Date.CompareTo(value.DateTime)
  End Function

  ''' <summary>
  ''' Compares a SmartDate to a date value.
  ''' </summary>
  ''' <param name="value">The date to which we are being compared.</param>
  ''' <returns>A value indicating if the comparison date is less than, equal to or greater than this date.</returns>
  Public Function CompareTo(ByVal value As Date) As Integer
    Return Me.Date.CompareTo(value)
  End Function

  ''' <summary>
  ''' Adds a TimeSpan onto the object.
  ''' </summary>
  ''' <param name="value">Span to add to the date.</param>
  Public Function Add(ByVal value As TimeSpan) As Date
    If IsEmpty Then
      Return Me.Date
    Else
      Return Me.Date.Add(value)
    End If
  End Function

  ''' <summary>
  ''' Subtracts a TimeSpan from the object.
  ''' </summary>
  ''' <param name="value">Span to subtract from the date.</param>
  Public Function Subtract(ByVal value As TimeSpan) As Date
    If IsEmpty Then
      Return Me.Date
    Else
      Return Me.Date.Subtract(value)
    End If
  End Function

  ''' <summary>
  ''' Subtracts a DateTimeOffset from the object.
  ''' </summary>
  ''' <param name="value">DateTimeOffset to subtract from the date.</param>
  ''' <remarks>
  ''' SmartDate maintains the date value as a DateTime,
  ''' so the provided DateTimeOffset is converted to a
  ''' DateTime for this comparison. You should be aware
  ''' that this can lead to a loss of precision in
  ''' some cases.
  ''' </remarks>
  Public Function Subtract(ByVal value As DateTimeOffset) As TimeSpan
    If IsEmpty Then
      Return TimeSpan.Zero
    Else
      Return Me.Date.Subtract(value.DateTime)
    End If
  End Function

  ''' <summary>
  ''' Subtracts a Date from the object.
  ''' </summary>
  ''' <param name="value">Date to subtract from the date.</param>
  Public Function Subtract(ByVal value As Date) As TimeSpan
    If IsEmpty Then
      Return TimeSpan.Zero
    Else
      Return Me.Date.Subtract(value)
    End If
  End Function

#End Region

#Region " Operators "

  ''' <summary>
  ''' Equality operator
  ''' </summary>
  ''' <param name="obj1">First object</param>
  ''' <param name="obj2">Second object</param>
  ''' <returns></returns>
  Public Shared Operator =(ByVal obj1 As SmartDate, ByVal obj2 As SmartDate) As Boolean
    Return obj1.Equals(obj2)
  End Operator

  ''' <summary>
  ''' Inequality operator
  ''' </summary>
  ''' <param name="obj1">First object</param>
  ''' <param name="obj2">Second object</param>
  ''' <returns></returns>
  Public Shared Operator <>(ByVal obj1 As SmartDate, ByVal obj2 As SmartDate) As Boolean
    Return Not obj1.Equals(obj2)
  End Operator

  ''' <summary>
  ''' Convert a SmartDate to a String.
  ''' </summary>
  ''' <param name="obj1">SmartDate value.</param>
  Public Shared Widening Operator CType(ByVal obj1 As SmartDate) As String
    Return obj1.Text
  End Operator

  ''' <summary>
  ''' Convert a SmartDate to a DateTime.
  ''' </summary>
  ''' <param name="obj1">SmartDate value.</param>
  Public Shared Widening Operator CType(ByVal obj1 As SmartDate) As Date
    Return obj1.Date
  End Operator

  ''' <summary>
  ''' Convert a SmartDate to a nullable DateTime.
  ''' </summary>
  ''' <param name="obj1">SmartDate value.</param>
  Public Shared Widening Operator CType(ByVal obj1 As SmartDate) As Date?
    Return obj1.ToNullableDate
  End Operator

  ''' <summary>
  ''' Convert a SmartDate to a DateTimeOffset.
  ''' </summary>
  ''' <param name="obj1">SmartDate value.</param>
  Public Shared Widening Operator CType(ByVal obj1 As SmartDate) As DateTimeOffset
    Return obj1.ToDateTimeOffset
  End Operator

  ''' <summary>
  ''' Convert a value to a SmartDate.
  ''' </summary>
  ''' <param name="dateValue">Value to convert.</param>
  Public Shared Narrowing Operator CType(ByVal dateValue As String) As SmartDate
    Return New SmartDate(dateValue)
  End Operator

  ''' <summary>
  ''' Convert a value to a SmartDate.
  ''' </summary>
  ''' <param name="dateValue">Value to convert.</param>
  Public Shared Widening Operator CType(ByVal dateValue As Date) As SmartDate
    Return New SmartDate(dateValue)
  End Operator

  ''' <summary>
  ''' Convert a value to a SmartDate.
  ''' </summary>
  ''' <param name="dateValue">Value to convert.</param>
  Public Shared Widening Operator CType(ByVal dateValue As Date?) As SmartDate
    Return New SmartDate(dateValue)
  End Operator

  ''' <summary>
  ''' Convert a value to a SmartDate.
  ''' </summary>
  ''' <param name="dateValue">Value to convert.</param>
  Public Shared Narrowing Operator CType(ByVal dateValue As DateTimeOffset) As SmartDate
    Return New SmartDate(dateValue)
  End Operator

  ''' <summary>
  ''' Equality operator
  ''' </summary>
  ''' <param name="obj1">First object</param>
  ''' <param name="obj2">Second object</param>
  ''' <returns></returns>
  Public Shared Operator =(ByVal obj1 As SmartDate, ByVal obj2 As DateTime) As Boolean
    Return obj1.Equals(obj2)
  End Operator

  ''' <summary>
  ''' Inequality operator
  ''' </summary>
  ''' <param name="obj1">First object</param>
  ''' <param name="obj2">Second object</param>
  ''' <returns></returns>
  Public Shared Operator <>(ByVal obj1 As SmartDate, ByVal obj2 As DateTime) As Boolean
    Return Not obj1.Equals(obj2)
  End Operator

  ''' <summary>
  ''' Equality operator
  ''' </summary>
  ''' <param name="obj1">First object</param>
  ''' <param name="obj2">Second object</param>
  ''' <returns></returns>
  Public Shared Operator =(ByVal obj1 As SmartDate, ByVal obj2 As String) As Boolean
    Return obj1.Equals(obj2)
  End Operator

  ''' <summary>
  ''' Inequality operator
  ''' </summary>
  ''' <param name="obj1">First object</param>
  ''' <param name="obj2">Second object</param>
  ''' <returns></returns>
  Public Shared Operator <>(ByVal obj1 As SmartDate, ByVal obj2 As String) As Boolean
    Return Not obj1.Equals(obj2)
  End Operator

  ''' <summary>
  ''' Addition operator
  ''' </summary>
  ''' <param name="start">Original date/time</param>
  ''' <param name="span">Span to add</param>
  ''' <returns></returns>
  Public Shared Operator +(ByVal start As SmartDate, ByVal span As TimeSpan) As SmartDate
    Return New SmartDate(start.Add(span), start.EmptyIsMin)
  End Operator

  ''' <summary>
  ''' Subtraction operator
  ''' </summary>
  ''' <param name="start">Original date/time</param>
  ''' <param name="span">Span to subtract</param>
  ''' <returns></returns>
  Public Shared Operator -(ByVal start As SmartDate, ByVal span As TimeSpan) As SmartDate
    Return New SmartDate(start.Subtract(span), start.EmptyIsMin)
  End Operator

  ''' <summary>
  ''' Subtraction operator
  ''' </summary>
  ''' <param name="start">Original date/time</param>
  ''' <param name="finish">Second date/time</param>
  ''' <returns></returns>
  Public Shared Operator -(ByVal start As SmartDate, ByVal finish As SmartDate) As TimeSpan
    Return start.Subtract(finish.Date)
  End Operator

  ''' <summary>
  ''' Greater than operator
  ''' </summary>
  ''' <param name="obj1">First object</param>
  ''' <param name="obj2">Second object</param>
  ''' <returns></returns>
  Public Shared Operator >(ByVal obj1 As SmartDate, ByVal obj2 As SmartDate) As Boolean
    Return obj1.CompareTo(obj2) > 0
  End Operator

  ''' <summary>
  ''' Less than operator
  ''' </summary>
  ''' <param name="obj1">First object</param>
  ''' <param name="obj2">Second object</param>
  ''' <returns></returns>
  Public Shared Operator <(ByVal obj1 As SmartDate, ByVal obj2 As SmartDate) As Boolean
    Return obj1.CompareTo(obj2) < 0
  End Operator

  ''' <summary>
  ''' Greater than operator
  ''' </summary>
  ''' <param name="obj1">First object</param>
  ''' <param name="obj2">Second object</param>
  ''' <returns></returns>
  Public Shared Operator >(ByVal obj1 As SmartDate, ByVal obj2 As DateTime) As Boolean
    Return obj1.CompareTo(obj2) > 0
  End Operator

  ''' <summary>
  ''' Less than operator
  ''' </summary>
  ''' <param name="obj1">First object</param>
  ''' <param name="obj2">Second object</param>
  ''' <returns></returns>
  Public Shared Operator <(ByVal obj1 As SmartDate, ByVal obj2 As DateTime) As Boolean
    Return obj1.CompareTo(obj2) < 0
  End Operator

  ''' <summary>
  ''' Greater than operator
  ''' </summary>
  ''' <param name="obj1">First object</param>
  ''' <param name="obj2">Second object</param>
  ''' <returns></returns>
  Public Shared Operator >(ByVal obj1 As SmartDate, ByVal obj2 As String) As Boolean
    Return obj1.CompareTo(obj2) > 0
  End Operator

  ''' <summary>
  ''' Less than operator
  ''' </summary>
  ''' <param name="obj1">First object</param>
  ''' <param name="obj2">Second object</param>
  ''' <returns></returns>
  Public Shared Operator <(ByVal obj1 As SmartDate, ByVal obj2 As String) As Boolean
    Return obj1.CompareTo(obj2) < 0
  End Operator

  ''' <summary>
  ''' Greater than or equals operator
  ''' </summary>
  ''' <param name="obj1">First object</param>
  ''' <param name="obj2">Second object</param>
  ''' <returns></returns>
  Public Shared Operator >=(ByVal obj1 As SmartDate, ByVal obj2 As SmartDate) As Boolean
    Return obj1.CompareTo(obj2) >= 0
  End Operator

  ''' <summary>
  ''' Less than or equals operator
  ''' </summary>
  ''' <param name="obj1">First object</param>
  ''' <param name="obj2">Second object</param>
  ''' <returns></returns>
  Public Shared Operator <=(ByVal obj1 As SmartDate, ByVal obj2 As SmartDate) As Boolean
    Return obj1.CompareTo(obj2) <= 0
  End Operator

  ''' <summary>
  ''' Greater than or equals operator
  ''' </summary>
  ''' <param name="obj1">First object</param>
  ''' <param name="obj2">Second object</param>
  ''' <returns></returns>
  Public Shared Operator >=(ByVal obj1 As SmartDate, ByVal obj2 As DateTime) As Boolean
    Return obj1.CompareTo(obj2) >= 0
  End Operator

  ''' <summary>
  ''' Less than or equals operator
  ''' </summary>
  ''' <param name="obj1">First object</param>
  ''' <param name="obj2">Second object</param>
  ''' <returns></returns>
  Public Shared Operator <=(ByVal obj1 As SmartDate, ByVal obj2 As DateTime) As Boolean
    Return obj1.CompareTo(obj2) <= 0
  End Operator

  ''' <summary>
  ''' Greater than or equals operator
  ''' </summary>
  ''' <param name="obj1">First object</param>
  ''' <param name="obj2">Second object</param>
  ''' <returns></returns>
  Public Shared Operator >=(ByVal obj1 As SmartDate, ByVal obj2 As String) As Boolean
    Return obj1.CompareTo(obj2) >= 0
  End Operator

  ''' <summary>
  ''' Less than or equals operator
  ''' </summary>
  ''' <param name="obj1">First object</param>
  ''' <param name="obj2">Second object</param>
  ''' <returns></returns>
  Public Shared Operator <=(ByVal obj1 As SmartDate, ByVal obj2 As String) As Boolean
    Return obj1.CompareTo(obj2) <= 0
  End Operator

#End Region

#Region " IConvertible "

  Private Function GetTypeCode() As System.TypeCode Implements System.IConvertible.GetTypeCode
    Return DirectCast(_date, IConvertible).GetTypeCode
  End Function

  Private Function ToBoolean(ByVal provider As System.IFormatProvider) As Boolean Implements System.IConvertible.ToBoolean
    Return DirectCast(_date, IConvertible).ToBoolean(provider)
  End Function

  Private Function ToByte(ByVal provider As System.IFormatProvider) As Byte Implements System.IConvertible.ToByte
    Return DirectCast(_date, IConvertible).ToByte(provider)
  End Function

  Private Function ToChar(ByVal provider As System.IFormatProvider) As Char Implements System.IConvertible.ToChar
    Return DirectCast(_date, IConvertible).ToChar(provider)
  End Function

  Private Function ToDateTime(ByVal provider As System.IFormatProvider) As Date Implements System.IConvertible.ToDateTime
    Return DirectCast(_date, IConvertible).ToDateTime(provider)
  End Function

  Private Function ToDecimal(ByVal provider As System.IFormatProvider) As Decimal Implements System.IConvertible.ToDecimal
    Return DirectCast(_date, IConvertible).ToDecimal(provider)
  End Function

  Private Function ToDouble(ByVal provider As System.IFormatProvider) As Double Implements System.IConvertible.ToDouble
    Return DirectCast(_date, IConvertible).ToDouble(provider)
  End Function

  Private Function ToInt16(ByVal provider As System.IFormatProvider) As Short Implements System.IConvertible.ToInt16
    Return DirectCast(_date, IConvertible).ToInt16(provider)
  End Function

  Private Function ToInt32(ByVal provider As System.IFormatProvider) As Integer Implements System.IConvertible.ToInt32
    Return DirectCast(_date, IConvertible).ToInt32(provider)
  End Function

  Private Function ToInt64(ByVal provider As System.IFormatProvider) As Long Implements System.IConvertible.ToInt64
    Return DirectCast(_date, IConvertible).ToInt64(provider)
  End Function

  Private Function ToSByte(ByVal provider As System.IFormatProvider) As SByte Implements System.IConvertible.ToSByte
    Return DirectCast(_date, IConvertible).ToSByte(provider)
  End Function

  Private Function ToSingle(ByVal provider As System.IFormatProvider) As Single Implements System.IConvertible.ToSingle
    Return DirectCast(_date, IConvertible).ToSingle(provider)
  End Function

  Private Function IConvertible_ToString(ByVal provider As System.IFormatProvider) As String Implements System.IConvertible.ToString
    Return DirectCast(Text, IConvertible).ToString(provider)
  End Function

  Private Function ToType(ByVal conversionType As System.Type, ByVal provider As System.IFormatProvider) As Object Implements System.IConvertible.ToType
    If conversionType.Equals(GetType(String)) Then
      Return DirectCast(Text, IConvertible).ToType(conversionType, provider)

    ElseIf conversionType.Equals(GetType(SmartDate)) Then
      Return Me

    Else
      Return DirectCast(_date, IConvertible).ToType(conversionType, provider)
    End If
  End Function

  Private Function ToUInt16(ByVal provider As System.IFormatProvider) As UShort Implements System.IConvertible.ToUInt16
    Return DirectCast(_date, IConvertible).ToUInt16(provider)
  End Function

  Private Function ToUInt32(ByVal provider As System.IFormatProvider) As UInteger Implements System.IConvertible.ToUInt32
    Return DirectCast(_date, IConvertible).ToUInt32(provider)
  End Function

  Private Function ToUInt64(ByVal provider As System.IFormatProvider) As ULong Implements System.IConvertible.ToUInt64
    Return DirectCast(_date, IConvertible).ToUInt64(provider)
  End Function

#End Region

#Region " IFormattable "

  Private Function IFormattable_ToString( _
    ByVal format As String, ByVal formatProvider As System.IFormatProvider) As String _
    Implements System.IFormattable.ToString

    Return Me.ToString(format)

  End Function

#End Region

#Region " IMobileObject Members "

  Private Sub GetState(ByVal info As Serialization.Mobile.SerializationInfo) Implements Serialization.Mobile.IMobileObject.GetState
    info.AddValue("SmartDate._date", _date)
    info.AddValue("SmartDate._defaultFormat", _defaultFormat)
    info.AddValue("SmartDate._emptyValue", _emptyValue.ToString())
    info.AddValue("SmartDate._initialized", _initialized)
    info.AddValue("SmartDate._format", _format)
  End Sub

  Private Sub SetState(ByVal info As Serialization.Mobile.SerializationInfo) Implements Serialization.Mobile.IMobileObject.SetState
    _date = info.GetValue(Of DateTime)("SmartDate._date")
    _defaultFormat = info.GetValue(Of String)("SmartDate._defaultFormat")
    _emptyValue = CType(System.Enum.Parse(GetType(EmptyValue), info.GetValue(Of String)("SmartDate._emptyValue"), True), EmptyValue)
    _format = info.GetValue(Of String)("SmartDate._format")
    _initialized = info.GetValue(Of Boolean)("SmartDate._initialized")
  End Sub

  Private Sub GetChildren(ByVal info As Serialization.Mobile.SerializationInfo, ByVal formatter As Serialization.Mobile.MobileFormatter) Implements Serialization.Mobile.IMobileObject.GetChildren

  End Sub

  Private Sub SetChildren(ByVal info As Serialization.Mobile.SerializationInfo, ByVal formatter As Serialization.Mobile.MobileFormatter) Implements Serialization.Mobile.IMobileObject.SetChildren

  End Sub

#End Region
  
End Structure
