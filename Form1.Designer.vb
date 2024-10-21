<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class Form1
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()>
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        btnFolder = New Button()
        txtSearch = New TextBox()
        ListView1 = New ListView()
        Label1 = New Label()
        Label2 = New Label()
        Label3 = New Label()
        SuspendLayout()
        ' 
        ' btnFolder
        ' 
        btnFolder.Location = New Point(12, 12)
        btnFolder.Name = "btnFolder"
        btnFolder.Size = New Size(60, 23)
        btnFolder.TabIndex = 1
        btnFolder.Text = "Project"
        btnFolder.UseVisualStyleBackColor = True
        ' 
        ' txtSearch
        ' 
        txtSearch.Anchor = AnchorStyles.Top Or AnchorStyles.Right
        txtSearch.Location = New Point(846, 12)
        txtSearch.Name = "txtSearch"
        txtSearch.PlaceholderText = "Search"
        txtSearch.Size = New Size(273, 23)
        txtSearch.TabIndex = 2
        ' 
        ' ListView1
        ' 
        ListView1.Anchor = AnchorStyles.Top Or AnchorStyles.Bottom Or AnchorStyles.Left Or AnchorStyles.Right
        ListView1.Location = New Point(12, 41)
        ListView1.Name = "ListView1"
        ListView1.Size = New Size(1107, 491)
        ListView1.TabIndex = 3
        ListView1.UseCompatibleStateImageBehavior = False
        ' 
        ' Label1
        ' 
        Label1.Anchor = AnchorStyles.Bottom Or AnchorStyles.Right
        Label1.AutoSize = True
        Label1.Location = New Point(1058, 537)
        Label1.Name = "Label1"
        Label1.Size = New Size(63, 15)
        Label1.TabIndex = 4
        Label1.Text = "Version 1.0"
        Label1.TextAlign = ContentAlignment.MiddleRight
        ' 
        ' Label2
        ' 
        Label2.Anchor = AnchorStyles.Bottom Or AnchorStyles.Left
        Label2.AutoSize = True
        Label2.Font = New Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, CByte(0))
        Label2.Location = New Point(12, 537)
        Label2.Name = "Label2"
        Label2.Size = New Size(109, 15)
        Label2.TabIndex = 5
        Label2.Text = "COMC Engineering"
        Label2.TextAlign = ContentAlignment.MiddleRight
        ' 
        ' Label3
        ' 
        Label3.Anchor = AnchorStyles.Bottom Or AnchorStyles.Left
        Label3.AutoSize = True
        Label3.Font = New Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, CByte(0))
        Label3.Location = New Point(78, 16)
        Label3.Name = "Label3"
        Label3.Size = New Size(0, 15)
        Label3.TabIndex = 6
        Label3.TextAlign = ContentAlignment.MiddleRight
        ' 
        ' Form1
        ' 
        AutoScaleDimensions = New SizeF(7F, 15F)
        AutoScaleMode = AutoScaleMode.Font
        ClientSize = New Size(1131, 559)
        Controls.Add(Label3)
        Controls.Add(Label2)
        Controls.Add(Label1)
        Controls.Add(ListView1)
        Controls.Add(txtSearch)
        Controls.Add(btnFolder)
        MinimumSize = New Size(650, 400)
        Name = "Form1"
        Text = "COMC Engineering"
        ResumeLayout(False)
        PerformLayout()
    End Sub
    Friend WithEvents btnFolder As Button
    Friend WithEvents txtSearch As TextBox
    Friend WithEvents ListView1 As ListView
    Friend WithEvents Label1 As Label
    Friend WithEvents Label2 As Label
    Friend WithEvents Label3 As Label

End Class
