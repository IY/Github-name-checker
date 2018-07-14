Imports System.Text
Imports System.IO
Imports System.Net
Imports System.Web

Public Class Form1

    Dim logincookie As CookieContainer
    Dim good, bad As Integer
    Dim pencil As Boolean = False

    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button1.Click
        Button2.Show()
        Button1.Hide()
        pencil = False
        ProgressBar1.Value = 0
        BackgroundWorker1.RunWorkerAsync()
    End Sub

    Private Sub Button3_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button3.Click
        OpenFileDialog1.Filter = "Text files|*.txt"
        If OpenFileDialog1.ShowDialog = DialogResult.OK Then
            ListBox1.Items.AddRange(File.ReadAllLines(OpenFileDialog1.FileName))
            Label1.Text = "Loaded: " & ListBox1.Items.Count.ToString
            ProgressBar1.Value = 0
            ProgressBar1.Maximum = ListBox1.Items.Count - 1
            ListBox1.SelectedIndex = 0
        End If
    End Sub
    Sub HelloMumPipl(ByVal sender As Object, ByVal e As DataReceivedEventArgs)
        UpdateTextBox(e.Data)
    End Sub
    Private Delegate Sub UpdateTextBoxDelegate(ByVal Text As String)
    Private Sub UpdateTextBox(ByVal Tex As String)
        If Me.InvokeRequired Then
            Dim del As New UpdateTextBoxDelegate(AddressOf UpdateTextBox)
            Dim args As Object() = {Tex}
            Me.Invoke(del, args)
        Else
            TextBox2.Text &= Tex & Environment.NewLine
        End If
    End Sub

    Private Sub BackgroundWorker1_DoWork(ByVal sender As System.Object, ByVal e As System.ComponentModel.DoWorkEventArgs) Handles BackgroundWorker1.DoWork
        Do While pencil = False
            If pencil = False Then
goagain:
                Dim currentaccount As String = ListBox1.SelectedItem.ToString
                Try
                    ListBox1.SelectedIndex += 1
                Catch ex As Exception
                    MsgBox("Done")
                    Exit Sub
                End Try


                TextBox2.Clear()

                Dim p As New Process
                p.StartInfo.FileName = "curl.exe"

                p.StartInfo.Arguments = "C:\Windows\System32\curl.exe -u " & TextBox3.Text & ":" & TextBox4.Text & " https://api.github.com/users/" & currentaccount


                p.StartInfo.UseShellExecute = False
                p.StartInfo.RedirectStandardOutput = True
                p.StartInfo.CreateNoWindow = True
                AddHandler p.OutputDataReceived, AddressOf HelloMumPipl
                p.Start()
                p.BeginOutputReadLine()

                p.WaitForExit()

                If TextBox2.Text.ToString.Contains("API rate limit exceeded") Then
                    MsgBox("Rate limited!")
                    Exit Sub

                Else
                    If TextBox2.Text.ToString.Contains("Not Found") Then
                    good += 1
                    Label2.Text = "Good: " & good.ToString

                    If pencil = False Then
                        TextBox1.Text += currentaccount & vbNewLine
                        Me.Refresh()
                    End If
                Else
                    bad += 1
                    Label3.Text = "Bad: " & bad.ToString
                End If

                Try
                    ProgressBar1.Value += 1
                Catch ex As Exception
                End Try

                If pencil = False Then
                    GoTo goagain
                End If
            End If
            End If
        Loop
    End Sub

    Private Sub Form1_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        Button2.Hide()
        Control.CheckForIllegalCrossThreadCalls = False
    End Sub

    Private Sub Button2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button2.Click
        pencil = True
        BackgroundWorker1.CancelAsync()
        MsgBox("Stopped")
    End Sub


    Private Sub Button4_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button4.Click
        SaveFileDialog1.Filter = "text files|*.txt"
        If SaveFileDialog1.ShowDialog = DialogResult.OK Then
            File.WriteAllText(OpenFileDialog1.FileName, TextBox1.Text)
        End If
    End Sub
End Class
