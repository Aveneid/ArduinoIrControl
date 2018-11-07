Imports System.Threading
Imports System.IO

Public Class Form1
    Public config(9, 2) As String
    Public configData As Integer
    Dim com As IO.Ports.SerialPort = Nothing
    Private trd As Thread
    Dim cancelling As Boolean = False
    Private dataC As String
    Private dataB As Integer

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        ComboBox2.SelectedItem = "9600"
        dataB = CInt(ComboBox2.SelectedItem)
        Control.CheckForIllegalCrossThreadCalls = False
        Form2.Show()
        Form2.Hide()
        Form2.Close()

    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        If com IsNot Nothing Then
            com.Close()
        End If

        ComboBox1.Items.Clear()
        For Each sp As String In My.Computer.Ports.SerialPortNames
            ComboBox1.Items.Add(sp)
        Next
    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        If com Is Nothing Then
            cancelling = False
            trd = New Thread(AddressOf ReadDataSerial)
            trd.IsBackground = True
            trd.Start()
        End If

    End Sub
    Private Sub ReadDataSerial()
        Dim data As String = Nothing
        If Strings.Left(ComboBox1.SelectedItem, 3) = "COM" Then
            Try
                com = My.Computer.Ports.OpenSerialPort(dataC)
                com.BaudRate = CInt(dataB)
                'com.ReadTimeout = 10000
                Do
                    If cancelling = True Then
                        Exit Do
                    Else
                        If com IsNot Nothing Then
                            data = com.ReadLine()
                            If data IsNot Nothing Then
                                RichTextBox1.AppendText("< ")
                                RichTextBox1.AppendText(data)
                                RichTextBox1.SelectionStart = Len(RichTextBox1.Text)
                                RichTextBox1.ScrollToCaret()
                                executeKeys(data.Replace(vbCr, "").Replace(vbLf, ""))
                            Else
                                Exit Do
                            End If
                        End If
                        Thread.Sleep(250)
                        data = Nothing
                    End If
                Loop
            Catch ex As TimeoutException
                ' RichTextBox1.AppendText("Error: read timeout (10s)")
                com.Close()
                com = Nothing
                ReadDataSerial()
            End Try
        End If
    End Sub

    Private Sub Button3_Click(sender As Object, e As EventArgs) Handles Button3.Click
        If com IsNot Nothing Then
            cancelling = True
            com.Close()
            com = Nothing
            trd.Abort()
        End If
    End Sub

    Private Sub ComboBox1_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ComboBox1.SelectedIndexChanged
        dataC = ComboBox1.SelectedItem
    End Sub

    Private Sub ComboBox2_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ComboBox2.SelectedIndexChanged
        dataB = CInt(ComboBox2.SelectedItem)
    End Sub

    Private Sub Button4_Click(sender As Object, e As EventArgs) Handles Button4.Click
        RichTextBox1.Text = ""
    End Sub

    Private Sub OptionsToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles OptionsToolStripMenuItem.Click
        Form2.show()
    End Sub
    Private Sub executeKeys(data As String)
        For i As Integer = 1 To configData
            If String.Compare(data, config(i, 1).Replace(vbCr, "").Replace(vbLf, "")) = 0 Then
                SendKeys.Flush()
                SendKeys.SendWait(config(i, 2))
            End If
        Next i
    End Sub
End Class
