Imports System.IO
Imports System.Drawing
Imports System.Runtime.InteropServices
Imports System.Diagnostics

Public Class Form1

    ' Declare SHCreateItemFromParsingName to use IShellItemImageFactory
    <DllImport("shell32.dll", CharSet:=CharSet.Unicode, PreserveSig:=False)>
    Private Shared Function SHCreateItemFromParsingName(ByVal pszPath As String, ByVal pbc As IntPtr, ByRef riid As Guid, <MarshalAs(UnmanagedType.Interface)> ByRef ppv As IShellItemImageFactory) As Integer
    End Function

    ' Define IShellItemImageFactory interface
    <ComImport(), InterfaceType(ComInterfaceType.InterfaceIsIUnknown), Guid("bcc18b79-ba16-442f-80c4-8a59c30c463b")>
    Private Interface IShellItemImageFactory
        <PreserveSig()>
        Function GetImage(ByVal size As ThumbnailSize, ByVal flags As Integer, ByRef phbm As IntPtr) As Integer
    End Interface

    ' Define ThumbnailSize structure to specify the size of the image
    <StructLayout(LayoutKind.Sequential)>
    Public Structure ThumbnailSize
        Public cx As Integer
        Public cy As Integer

        Public Sub New(ByVal x As Integer, ByVal y As Integer)
            cx = x
            cy = y
        End Sub
    End Structure

    ' List to store all file items for searching
    Private originalItems As New List(Of ListViewItem)
    Private selectedItem As ListViewItem ' Store the item selected for Properties

    ' Add the context menu strip
    Private contextMenu As New ContextMenuStrip()

    Private partPropertiesPath As String ' Store Part Properties Path

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        ' Create context menu items
        Dim openPartItem As New ToolStripMenuItem("Open in SW")
        Dim purchaseItem As New ToolStripMenuItem("Purchase")
        Dim print3DItem As New ToolStripMenuItem("3D Print")
        Dim propertiesItem As New ToolStripMenuItem("Properties")

        ' Add handlers for context menu items
        AddHandler openPartItem.Click, AddressOf OpenPart_Click
        AddHandler purchaseItem.Click, AddressOf Purchase_Click ' Purchase action
        AddHandler print3DItem.Click, AddressOf Print3D_Click
        AddHandler propertiesItem.Click, AddressOf Properties_Click

        ' Add items to context menu
        contextMenu.Items.Add(openPartItem)
        contextMenu.Items.Add(purchaseItem)
        contextMenu.Items.Add(print3DItem)
        contextMenu.Items.Add(New ToolStripSeparator()) ' Add a separator
        contextMenu.Items.Add(propertiesItem)

        ' Assign the context menu to ListView
        ListView1.ContextMenuStrip = contextMenu
    End Sub

    Private Sub btnFolder_Click(sender As Object, e As EventArgs) Handles btnFolder.Click
        ' Create a new instance of FolderBrowserDialog
        Dim folderDialog As New FolderBrowserDialog()

        ' Show the dialog and check if the user selected a folder
        If folderDialog.ShowDialog() = DialogResult.OK Then
            ' Get the selected folder path
            Dim targetPath As String = folderDialog.SelectedPath

            ' Update Label3 to show the name of the selected folder
            Label3.Text = Path.GetFileName(targetPath) ' Show only the folder name

            ' Clear any existing items and icons in the ListView
            ListView1.Items.Clear()
            ListView1.SmallImageList = New ImageList()
            ListView1.SmallImageList.ImageSize = New Size(64, 64) ' Set the icon size for list view

            ' Clear the original items list
            originalItems.Clear()

            ' Ensure "Part Properties" folder exists in the selected path
            partPropertiesPath = Path.Combine(targetPath, "Part Properties")
            If Not Directory.Exists(partPropertiesPath) Then
                Directory.CreateDirectory(partPropertiesPath)
            End If

            ' Set the ListView to Details view to display a vertical list
            ListView1.View = View.Details

            ' Add columns if they do not already exist
            If ListView1.Columns.Count = 0 Then
                ListView1.Columns.Add("Preview", 100) ' Image Preview column (icon index will be used)
                ListView1.Columns.Add("Item", 50) ' Item number column
                ListView1.Columns.Add("Part Number", 300) ' Part Number column
                ListView1.Columns.Add("Quantity", 100) ' Quantity column
                ListView1.Columns.Add("Total Cost", 100) ' Total Cost column (default to "$0")
                ListView1.Columns.Add("Inventory", 100) ' Inventory column
                ListView1.Columns.Add("Revised", 200) ' Revised column (editor and date)
                ListView1.Columns.Add("Notes", 150) ' Notes column
            End If

            ' Check if the path exists
            If Directory.Exists(targetPath) Then
                ' Get all .SLDPRT files from the directory
                Dim files As String() = Directory.GetFiles(targetPath, "*.SLDPRT")
                Dim itemNumber As Integer = 1

                ' Loop through each file and extract the thumbnail
                For Each file As String In files
                    ' Skip files that start with ~ (e.g., temporary or hidden files)
                    If Path.GetFileName(file).StartsWith("~") Then
                        Continue For
                    End If

                    ' Get the file thumbnail using IShellItemImageFactory
                    Dim thumbnail As Bitmap = GetThumbnailForFile(file)

                    ' If a thumbnail was found, add it to the ImageList
                    If thumbnail IsNot Nothing Then
                        ListView1.SmallImageList.Images.Add(thumbnail)

                        ' File information
                        Dim partNumber As String = Path.GetFileNameWithoutExtension(file)
                        Dim revisedInfo As String = System.IO.File.GetLastWriteTime(file).ToString("MM/dd/yyyy") & " by " & Environment.UserName.ToUpper()

                        ' Add items with the relevant information
                        Dim item As New ListViewItem("") ' Image will be loaded into the first column (Preview)
                        item.ImageIndex = ListView1.SmallImageList.Images.Count - 1 ' Set the image index to the last added thumbnail
                        item.SubItems.Add(itemNumber.ToString()) ' Item number in second column
                        item.SubItems.Add(partNumber) ' Part Number
                        item.SubItems.Add("1") ' Quantity (default value)
                        item.SubItems.Add("$0") ' Total Cost (default to "$0")
                        item.SubItems.Add("0") ' Inventory (default value)
                        item.SubItems.Add(revisedInfo) ' Revised column (last edit date and user)
                        item.SubItems.Add("None") ' Notes (default "None")

                        ' Store file path in Tag for opening
                        item.Tag = file

                        ' Call LoadPartProperties to update the ListView with the part's properties if they exist
                        LoadPartProperties(partNumber, partPropertiesPath, item)

                        ' Add the item to the ListView
                        ListView1.Items.Add(item)

                        ' Increment item number for each row
                        itemNumber += 1

                        ' Add item to originalItems list for future searching
                        originalItems.Add(item)
                    End If
                Next
            Else
                MessageBox.Show("The specified path does not exist.")
            End If
        End If
    End Sub

    ' Function to load part properties from the text file if it exists
    Private Sub LoadPartProperties(partNumber As String, partPropertiesDirectory As String, ByRef listViewItem As ListViewItem)
        ' Build the path to the part's properties file
        Dim partPropertiesFile As String = Path.Combine(partPropertiesDirectory, partNumber & ".txt")

        ' Check if the properties file exists
        If File.Exists(partPropertiesFile) Then
            ' Load the properties from the file
            Dim properties As String() = File.ReadAllLines(partPropertiesFile)

            ' Ensure there are enough lines to match the expected properties
            If properties.Length >= 5 Then ' Assuming we have Purchase Link, Item Cost, Inventory, Consumed, and Notes
                ' Retrieve Inventory and Consumed values
                Dim inventory As Integer = Convert.ToInt32(properties(2)) ' Inventory from Form2 (3rd property line)
                Dim consumed As Integer = Convert.ToInt32(properties(3)) ' Consumed from Form2 (4th property line)

                ' Update the Inventory column in Form1 to show "Inventory - Consumed pcs."
                Dim finalInventory As Integer = inventory - consumed
                listViewItem.SubItems(5).Text = finalInventory.ToString() & " pcs." ' Inventory column (SubItem index 5)

                ' Update Notes in Form1
                listViewItem.SubItems(7).Text = If(String.IsNullOrEmpty(properties(4)), "None", properties(4)) ' Notes column, default to "None"

                ' Update Total Cost in Form1 based on Quantity and Item Cost
                Dim quantity As Integer = Convert.ToInt32(listViewItem.SubItems(3).Text) ' Quantity column in Form1 (SubItems(3))
                Dim itemCost As Decimal = Convert.ToDecimal(properties(1)) ' Item Cost from Form2 (2nd property line)
                Dim totalCost As Decimal = quantity * itemCost
                listViewItem.SubItems(4).Text = "$" & totalCost.ToString("F2") ' Total Cost column, formatted with $
            End If
        End If
    End Sub

    ' Function to retrieve a high-quality thumbnail using IShellItemImageFactory
    Private Function GetThumbnailForFile(ByVal filePath As String) As Bitmap
        Dim factory As IShellItemImageFactory = Nothing
        Dim size As New ThumbnailSize(64, 64) ' Request a 64x64 thumbnail for the list
        Dim hbitmap As IntPtr = IntPtr.Zero
        Dim iid As Guid = New Guid("bcc18b79-ba16-442f-80c4-8a59c30c463b")

        ' Create a shell item from the file path
        If SHCreateItemFromParsingName(filePath, IntPtr.Zero, iid, factory) = 0 Then
            ' Get the thumbnail image
            If factory.GetImage(size, 0, hbitmap) = 0 And hbitmap <> IntPtr.Zero Then
                ' Convert the HBITMAP to a .NET Bitmap
                Dim thumbnail As Bitmap = Image.FromHbitmap(hbitmap)
                ' Clean up the HBITMAP to avoid memory leaks
                DeleteObject(hbitmap)
                Return thumbnail
            End If
        End If

        Return Nothing
    End Function

    ' Import DeleteObject to release unmanaged resources
    <DllImport("gdi32.dll")>
    Private Shared Function DeleteObject(ByVal hObject As IntPtr) As Boolean
    End Function

    ' Handle the right-click context menu item "Open Part"
    Private Sub OpenPart_Click(sender As Object, e As EventArgs)
        If selectedItem IsNot Nothing Then
            Try
                ' Open the file using ShellExecute (as if from File Explorer)
                Dim filePath As String = selectedItem.Tag.ToString()
                Process.Start(New ProcessStartInfo(filePath) With {.UseShellExecute = True})
            Catch ex As Exception
                MessageBox.Show("Error opening file: " & ex.Message)
            End Try
        End If
    End Sub

    ' Handle the right-click context menu item "Purchase"
    Private Sub Purchase_Click(sender As Object, e As EventArgs)
        If selectedItem IsNot Nothing Then
            ' Get the part number
            Dim partNumber As String = selectedItem.SubItems(2).Text
            ' Define the path for the part properties file
            Dim partPropertiesFile As String = Path.Combine(partPropertiesPath, partNumber & ".txt")

            ' Check if the part properties file exists
            If File.Exists(partPropertiesFile) Then
                ' Read all lines of the properties file
                Dim properties As String() = File.ReadAllLines(partPropertiesFile)

                ' Find the line that contains "http" to identify the purchase link
                Dim purchaseLink As String = properties _
                .FirstOrDefault(Function(line) line.Trim().StartsWith("http", StringComparison.OrdinalIgnoreCase))

                ' Check if we found a valid purchase link
                If Not String.IsNullOrEmpty(purchaseLink) Then
                    Try
                        ' Open the purchase link in the default browser
                        Process.Start(New ProcessStartInfo(purchaseLink.Trim()) With {.UseShellExecute = True})
                    Catch ex As Exception
                        MessageBox.Show("Error opening link: " & ex.Message)
                    End Try
                Else
                    MessageBox.Show("No valid purchase link found in the part properties file.")
                End If
            Else
                MessageBox.Show("Part properties file not found.")
            End If
        End If
    End Sub

    ' Handle the right-click context menu item "3D Print"
    Private Sub Print3D_Click(sender As Object, e As EventArgs)
        If selectedItem IsNot Nothing Then
            ' Get the path of the selected part
            Dim partPath As String = selectedItem.Tag.ToString()
            Dim partName As String = Path.GetFileNameWithoutExtension(partPath)
            Dim stepFolder As String = Path.Combine(Path.GetDirectoryName(partPath), "3MF")
            Dim stepFilePath As String = Path.Combine(stepFolder, partName & ".3MF")

            ' Check if the .3MF file exists
            If File.Exists(stepFilePath) Then
                ' Open the .3MF file using ShellExecute
                Process.Start(New ProcessStartInfo(stepFilePath) With {.UseShellExecute = True})
            Else
                ' Show error if .3MF file is not found
                MessageBox.Show("Error: 3MF file not found.")
            End If
        End If
    End Sub

    ' Handle the right-click context menu item "Properties"
    Private Sub Properties_Click(sender As Object, e As EventArgs)
        If selectedItem IsNot Nothing Then
            ' Get the part number from the selected item
            Dim partNumber As String = selectedItem.SubItems(2).Text ' Assuming Part Number is in SubItem(2)

            ' Get the path to the Part Properties folder
            Dim partPropertiesPath As String = Path.Combine(Path.GetDirectoryName(selectedItem.Tag.ToString()), "Part Properties")

            ' Open Form2 and pass the part number and part properties directory
            Dim form2 As New Form2(partNumber, partPropertiesPath)
            form2.ShowDialog()

            ' After Form2 closes, reload the part properties in Form1
            LoadPartProperties(partNumber, partPropertiesPath, selectedItem)
        End If
    End Sub

    ' Handle mouse right-click to select the item and show the context menu
    Private Sub ListView1_MouseUp(sender As Object, e As MouseEventArgs) Handles ListView1.MouseUp
        If e.Button = MouseButtons.Right Then
            Dim hitTestInfo = ListView1.HitTest(e.Location)
            If hitTestInfo.Item IsNot Nothing Then
                ' Store the selected item for use in the context menu action
                selectedItem = hitTestInfo.Item
            End If
        End If
    End Sub

    ' Search function to filter ListView by Part Number
    Private Sub txtSearch_TextChanged(sender As Object, e As EventArgs) Handles txtSearch.TextChanged
        Dim searchQuery As String = txtSearch.Text.ToLower()

        ' Clear current items
        ListView1.Items.Clear()

        ' Add items that match the search query from the originalItems list
        For Each item As ListViewItem In originalItems
            If item.SubItems(2).Text.ToLower().Contains(searchQuery) Then ' Part Number is in SubItems(2)
                ListView1.Items.Add(item)
            End If
        Next
    End Sub

End Class
