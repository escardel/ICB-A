Imports System.IO

Public Class Form2

    ' Part number associated with this form instance
    Private partNumber As String
    Private partPropertiesPath As String

    ' Initialize Form2 with the part number
    Public Sub New(partNumber As String, partPropertiesDirectory As String)
        ' This call is required by the designer.
        InitializeComponent()

        ' Store the part number and properties directory
        Me.partNumber = partNumber
        Me.partPropertiesPath = Path.Combine(partPropertiesDirectory, partNumber & ".txt")

        ' Update the window title with the part number
        UpdateWindowTitle()

        ' Set up the DataGridView
        SetupDataGridView()

        ' Load existing properties if available
        LoadProperties()
    End Sub

    ' Update the Form2 window title to match the selected part number
    Private Sub UpdateWindowTitle()
        Me.Text = "Properties for Part Number: " & partNumber
    End Sub

    ' Set up the DataGridView with two columns
    Private Sub SetupDataGridView()
        ' Set the DataGridView to fill the form
        DataGridView1.Dock = DockStyle.Fill

        ' Disable the last empty row by not allowing the user to add rows
        DataGridView1.AllowUserToAddRows = False

        ' Set up the first column (Labels, non-editable)
        DataGridView1.Columns.Add("Property", "Property")
        DataGridView1.Columns(0).ReadOnly = True
        DataGridView1.Columns(0).AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells ' Auto-size to fit content

        ' Set up the second column (Editable values)
        DataGridView1.Columns.Add("Value", "Value")
        DataGridView1.Columns(1).AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill ' Stretch to fill available space

        ' Add rows for each property
        DataGridView1.Rows.Add("Purchase Link")
        DataGridView1.Rows.Add("Item Cost")
        DataGridView1.Rows.Add("Inventory") ' Inventory row
        DataGridView1.Rows.Add("Consumed") ' New Consumed row
        DataGridView1.Rows.Add("Notes") ' Notes row

        ' Hide row headers for a cleaner look
        DataGridView1.RowHeadersVisible = False
    End Sub

    ' Load the part properties from the text file (if it exists)
    Private Sub LoadProperties()
        ' Check if the properties file exists
        If File.Exists(partPropertiesPath) Then
            ' Read the file content and split into lines
            Dim lines As String() = File.ReadAllLines(partPropertiesPath)

            ' Load each line into the DataGridView
            For i As Integer = 0 To lines.Length - 1
                ' Ensure we don't exceed the row count
                If i < DataGridView1.Rows.Count Then
                    DataGridView1.Rows(i).Cells(1).Value = lines(i)
                End If
            Next
        End If
    End Sub

    ' Save the properties to the text file
    Private Sub SaveProperties()
        ' Prepare an array to store the values
        Dim values(DataGridView1.Rows.Count - 1) As String

        ' Extract values from the second column (editable values)
        For i As Integer = 0 To DataGridView1.Rows.Count - 1
            values(i) = DataGridView1.Rows(i).Cells(1).Value?.ToString() ' Get value as string or empty if null
        Next

        ' Write the values to the file
        File.WriteAllLines(partPropertiesPath, values)
    End Sub

    ' Handle Form Closing event to save the properties
    Private Sub Form2_FormClosing(sender As Object, e As FormClosingEventArgs) Handles MyBase.FormClosing
        SaveProperties()
    End Sub

End Class
