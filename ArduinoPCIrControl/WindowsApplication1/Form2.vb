Imports System.IO




Public Class Form2
    Dim config(9, 2) As String
    Dim configtmp(9, 2) As String
    Dim cfg As String = "./config.cfg"
    Dim configEnabled = False

    Dim mainFileStream As New IO.FileStream(cfg, IO.FileMode.OpenOrCreate,
                                                 IO.FileAccess.ReadWrite,
                                                 IO.FileShare.None)
    Dim fileWriter As New IO.StreamWriter(mainFileStream)
    Dim fileReader As New IO.StreamReader(mainFileStream)
    Dim linesReaded As Integer = 0

    Private Sub Form2_FormClosing(sender As Object, e As FormClosingEventArgs) Handles Me.FormClosing
        Form1.config = config
        Form1.configData = linesReaded
        fileWriter.Close()
        fileReader.Close()
    End Sub
    Private Sub Form2_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Dim myFile As New FileInfo(cfg)
        Dim sizeInBytes As Long = myFile.Length
        If sizeInBytes > 4 Then
            Dim line As String = ""
            Dim id As Integer = 1
            line = fileReader.ReadLine()
            If line = "True" Or line = "TRUE" Or line = "true" Then
                CheckBox1.Checked = True
                configEnabled = True
                GroupBox1.Enabled = True
                Do While line IsNot Nothing
                    line = fileReader.ReadLine()
                    If line Is Nothing Then
                        Exit Do
                    End If
                    linesReaded = linesReaded + 1
                    config(id, 0) = line.Split(":"c)(0)
                    config(id, 1) = line.Split(":"c)(1)
                    config(id, 2) = line.Split(":"c)(2)
                    id = id + 1
                Loop
                id = id - 1
                For Each combo As ComboBox In GroupBox1.Controls.OfType(Of ComboBox).OrderBy(Function(x) x.Top())
                    For Each txt As TextBox In GroupBox1.Controls.OfType(Of TextBox).OrderBy(Function(x) x.Top())
                        If Strings.Right(txt.Name, 1) = Strings.Right(combo.Name, 1) Then
                            If id > 0 Then
                                txt.Text = config(id, 1)
                                combo.SelectedItem = config(id, 2)
                                id = id - 1
                                Exit For
                            End If
                        End If
                    Next
                Next
            End If
        End If
    End Sub
    Private Sub CheckBox1_CheckedChanged(sender As Object, e As EventArgs) Handles CheckBox1.CheckedChanged
        configEnabled = Not (configEnabled)
        GroupBox1.Enabled = Not (GroupBox1.Enabled)
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button99.Click

        Dim id = 1
        If configEnabled = True Then
            For Each combo As ComboBox In GroupBox1.Controls.OfType(Of ComboBox)()
                For Each txt As TextBox In GroupBox1.Controls.OfType(Of TextBox)()
                    If Strings.Right(txt.Name, 1) = Strings.Right(combo.Name, 1) Then
                        If Len(txt.Text) > 0 Then
                            configtmp(id, 0) = id
                            configtmp(id, 1) = txt.Text
                            configtmp(id, 2) = combo.SelectedItem
                            id = id + 1
                            Exit For
                        End If
                    End If
                Next
            Next
            If Len(configtmp(1, 0)) > 0 Then
                fileWriter.BaseStream.SetLength(0)
                fileWriter.Flush()
                fileWriter.Write("True")
                fileWriter.Write(vbNewLine)
                For i As Integer = 1 To id - 1
                    fileWriter.Write(configtmp(i, 0) & ":" & configtmp(i, 1) & ":" & configtmp(i, 2))
                    fileWriter.Write(vbNewLine)
                Next
            End If
        Else
            fileWriter.BaseStream.SetLength(0)
            fileWriter.Flush()
            fileWriter.Write("false")
            fileWriter.Write(vbNewLine)
            For i As Integer = 1 To id - 1
                fileWriter.Write(configtmp(i, 0) & ":" & configtmp(i, 1) & ":" & configtmp(i, 2))
                fileWriter.Write(vbNewLine)
            Next

        End If
        fileWriter.Close()
        fileReader.Close()
        Me.Close()
    End Sub


End Class