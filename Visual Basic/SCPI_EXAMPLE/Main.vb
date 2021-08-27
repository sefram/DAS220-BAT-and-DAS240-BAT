Option Strict Off
Option Explicit On
Imports VB = Microsoft.VisualBasic
Imports System.Net.Sockets
Imports System.IO

Friend Class frmMain
	Inherits System.Windows.Forms.Form

    Sub LitTCPIP()

        Dim bytes(32768) As Byte
        Dim data As String = Nothing
        Dim i As Integer

        If Stream.CanRead Then

            i = Stream.Read(bytes, 0, bytes.Length)

            ' Translate data bytes to a string.
            data = System.Text.Encoding.GetEncoding(1252).GetString(bytes, 0, i)

            PtEcrFifoRxD = PtEcrFifoRxD + data.Length
            FinReception = False

            ' Filtrage séparateurs
            While InStr(data, Chr(4))
                Mid(data, InStr(data, Chr(4)), 1) = ";"
            End While
            While InStr(data, Chr(10))
                Mid(data, InStr(data, Chr(10)), 1) = ";"
            End While
            While InStr(data, Chr(13))
                Mid(data, InStr(data, Chr(13)), 1) = ";"
            End While

            ' Ecriture dans la fifo
            FifoRxD = FifoRxD & data

        End If


    End Sub

    Sub Emission(ByRef st As String)

        'Préparation réception message
        If InStr(st, "?") Then
            FifoRxD = ""
            PtEcrFifoRxD = 0
            PtLecFifoRxD = 0
            FinReception = False
        End If

        If ErreurRemote Then Exit Sub

        MessageAEmettre = st & vbLf

        ErreurTimeout = 0
        tmrTimeout.Interval = 1000
        tmrTimeout.Enabled = True

        ' Translate the passed message into ASCII and store it as a Byte array.
        Dim data As [Byte]() = System.Text.Encoding.ASCII.GetBytes(MessageAEmettre)

        If first_connection = True Then
            Client = New TcpClient(AdresseIP, 23)

            ' Get a client stream for reading and writing.
            Stream = Client.GetStream()
            first_connection = False
        End If


        ' Send the message to the connected TcpServer. 
        Stream.Write(data, 0, data.Length)

        tmrTimeout.Enabled = False

        'Cas d'une interrogation
        If InStr(st, "?") Then Reception()

    End Sub

    Public Sub Reception()

        If ErreurRemote Then Exit Sub

        Dim car As Object

        ' Attente réception message complet
        tmrTimeout.Enabled = True
        Do
            LitTCPIP()
            If ErreurRemote Then Exit Do
        Loop While InStr(FifoRxD, ";") = 0

        tmrTimeout.Enabled = False

        If ErreurRemote Then Exit Sub

        ' Lecture message
        MessageRecu = FifoRxD
        PtLecFifoRxD = PtLecFifoRxD + Len(MessageRecu)

        ' Lecture séparateur
        Do
            System.Windows.Forms.Application.DoEvents()
            car = Mid(FifoRxD, PtLecFifoRxD + 1, 1)
            If car <> ";" Then Exit Do
            PtLecFifoRxD = PtLecFifoRxD + 1
        Loop While PtLecFifoRxD <> PtEcrFifoRxD

        ' Ecriture message à l'écran
        txtReception.Text = txtReception.Text & " " & MessageRecu & Chr(13) & Chr(10)

    End Sub

    Private Sub cmdEmission_Click(ByVal eventSender As System.Object, ByVal eventArgs As System.EventArgs) Handles cmdEmission.Click

        txtEmission.Text = ""

    End Sub

   

    Private Sub CmdReception_Click(ByVal eventSender As System.Object, ByVal eventArgs As System.EventArgs) Handles CmdReception.Click

        txtReception.Text = ""

    End Sub

    Private Sub cmdStop_Click(ByVal eventSender As System.Object, ByVal eventArgs As System.EventArgs) Handles cmdStop.Click

        ErreurRemote = True
        cmdTransfert.Enabled = True

    End Sub

    Private Sub cmdTransfert_Click(ByVal eventSender As System.Object, ByVal eventArgs As System.EventArgs) Handles cmdTransfert.Click

        Dim mes As String

        ErreurRemote = False

        cmdTransfert.Enabled = False
        chanq_btn.Enabled = False

        txtEmission.SelectAll()
        txtEmission.SelectionBackColor = Color.White
        txtEmission.Update()

        If VB.Right(txtEmission.Text, 1) <> vbLf Then txtEmission.Text = txtEmission.Text & vbCrLf

        Debut = 1
        Do
            Fin = 1 + InStr(Debut, txtEmission.Text, vbLf)

            If Fin <= Debut Or ErreurRemote Then Exit Do

            txtEmission.SelectAll()
            txtEmission.SelectionBackColor = Color.White
            txtEmission.Update()
            mes = Mid(txtEmission.Text, Debut, Fin - Debut - 1)
            If mes <> "" Then
                txtEmission.Select(txtEmission.Find(mes.ToCharArray, Debut - 1), mes.Length + 1)
                txtEmission.SelectionBackColor = Color.Cyan
                txtEmission.Update()
                Emission((mes))
            End If
            Debut = Fin
        Loop

        cmdTransfert.Enabled = True
        chanq_btn.Enabled = True

    End Sub

    Private Sub frmMain_Load(ByVal eventSender As System.Object, ByVal eventArgs As System.EventArgs) Handles MyBase.Load

        Dim reg As Object
        Dim dot1, dot2, dot3 As Integer
        Dim a0, a1, a2, a3 As Integer

        first_connection = True

        Device_combobox.Items.Add("DAS220/DAS240")
        Device_combobox.Items.Add("DAS30")
        Device_combobox.Items.Add("DAS50")
        Device_combobox.Items.Add("DAS60")
        Device_combobox.SelectedIndex = 0

        Channel_combobox.Items.Add("A1")
        Channel_combobox.Items.Add("A2")
        Channel_combobox.Items.Add("A3")
        Channel_combobox.Items.Add("A4")
        Channel_combobox.Items.Add("A5")
        Channel_combobox.Items.Add("A6")
        Channel_combobox.Items.Add("A10")
        Channel_combobox.Items.Add("A11")
        Channel_combobox.Items.Add("A12")
        Channel_combobox.Items.Add("P1")
        Channel_combobox.Items.Add("P2")
        Channel_combobox.Items.Add("K1")
        Channel_combobox.Items.Add("K2")
        Channel_combobox.Items.Add("K3")
        Channel_combobox.Items.Add("K4")
        Channel_combobox.SelectedIndex = 0

        type_combobox.Items.Add("VOLtage DC")
        type_combobox.Items.Add("SHUNT DC,S10M")
        type_combobox.Items.Add("THErmo J,COMP,CEL")
        type_combobox.Items.Add("FREQ")
        type_combobox.Items.Add("PWM")
        type_combobox.Items.Add("PT100 W3,0")
        type_combobox.SelectedIndex = 0



        With frmConfig

            reg = GetSetting("SEFRAM", "TCPIP", "IPAddress", "192.168.0.166")
            'reg = GetSetting("SEFRAM", "TCPIP", "IPAddress", reg.ToString)
            dot1 = InStr(1, reg, ".")
            dot2 = InStr(dot1 + 1, reg, ".")
            dot3 = InStr(dot2 + 2, reg, ".")
            If dot1 <= 0 Or dot2 <= 0 Or dot3 <= 0 Then reg = "192.168.0.166"

            a0 = Val(VB.Left(reg, dot1 - 1))
            If a0 < 0 Or a0 > 255 Then a0 = 192

            a1 = Val(Mid(reg, dot1 + 1, dot2 - dot1 - 1))
            If a1 < 0 Or a1 > 255 Then a1 = 168

            a2 = Val(Mid(reg, dot2 + 1, dot3 - dot2 - 1))
            If a2 < 0 Or a2 > 255 Then a2 = 0

            a3 = Val(VB.Right(reg, Len(reg) - dot3))
            If a3 < 0 Or a3 > 255 Then a3 = 166

            .txtIP(0).Text = Str(a0)
            .txtIP(1).Text = Str(a1)
            .txtIP(2).Text = Str(a2)
            .txtIP(3).Text = Str(a3)
            AdresseIP = .txtIP(0).Text.Trim & "." & .txtIP(1).Text.Trim & "." & .txtIP(2).Text.Trim & "." & .txtIP(3).Text.Trim

        End With

        Server = New TcpListener(LocalAddr, 23)

        ' Start listening for client requests.
        Server.Start()

        Enabled = True

    End Sub

    Private Sub frmMain_FormClosed(ByVal eventSender As System.Object, ByVal eventArgs As System.Windows.Forms.FormClosedEventArgs) Handles Me.FormClosed

        SaveSetting("SEFRAM", "TCPIP", "IPAddress", AdresseIP)

        End

    End Sub
	
	Public Sub mnuConfig_Click(ByVal eventSender As System.Object, ByVal eventArgs As System.EventArgs) Handles mnuConfig.Click
		
		Enabled = False
		frmConfig.Visible = True
		
	End Sub
	
	Public Sub mnuEnregistrer_Click(ByVal eventSender As System.Object, ByVal eventArgs As System.EventArgs) Handles mnuEnregistrer.Click

        Dim myStream As Stream
        Dim SaveFileDialog As New SaveFileDialog()

		Enabled = False

        SaveFileDialog.Filter = "txt files (*.txt)|*.txt"
        SaveFileDialog.ShowDialog()

        If SaveFileDialog.ShowDialog() = Windows.Forms.DialogResult.OK Then
            myStream = SaveFileDialog.OpenFile()
            If Not (myStream Is Nothing) Then

                txtEmission.SaveFile(myStream, RichTextBoxStreamType.PlainText)
                myStream.Close()

            End If
        End If

		Enabled = True
	End Sub
	
	Public Sub mnuOuvrir_Click(ByVal eventSender As System.Object, ByVal eventArgs As System.EventArgs) Handles mnuOuvrir.Click
		
        Dim myStream As Stream
        Dim openFileDialog As New OpenFileDialog()

		Enabled = False
		
        On Error GoTo GereErreur ' Active la routine de gestion d'erreur

        OpenFileDialog.Filter = "txt files (*.txt)|*.txt"

        If openFileDialog.ShowDialog() = Windows.Forms.DialogResult.OK Then
            myStream = openFileDialog.OpenFile()
            If Not (myStream Is Nothing) Then

                txtEmission.LoadFile(myStream, RichTextBoxStreamType.PlainText)
                myStream.Close()

            End If
        End If
		
		On Error GoTo 0
		
		Enabled = True
		Exit Sub
		
GereErreur: 
		On Error GoTo 0
		MsgBox("Erreur fichier non conforme !", MsgBoxStyle.Critical + MsgBoxStyle.OKOnly, "Erreur de type de fichier")
		Err.Clear()
		FileClose(1)
		Enabled = True
		
	End Sub
	
	Public Sub mnuQuitter_Click(ByVal eventSender As System.Object, ByVal eventArgs As System.EventArgs) Handles mnuQuitter.Click
		
        frmMain_FormClosed(Me, New System.Windows.Forms.FormClosedEventArgs((0)))
		
	End Sub
	
	Private Sub tmrTimeout_Tick(ByVal eventSender As System.Object, ByVal eventArgs As System.EventArgs) Handles tmrTimeout.Tick
		
		If ErreurRemote = False Then
            ErreurTimeout = MsgBox("Vérifiez le câble ou " & Chr(13) & Chr(10) & "les adresses IP", MsgBoxStyle.Critical + MsgBoxStyle.OkOnly, "Dépassement temps de transfert")
			tmrTimeout.Enabled = False
			ErreurRemote = True
		End If
		
	End Sub

    Private Sub cmdREM_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdREM.Click

        Emission("*REM;")

    End Sub

    Private Sub cmdLOC_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdLOC.Click

        Emission("*LOC;")

    End Sub

    Private Sub cmdIDN_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cmdIDN.Click

        Emission("*IDN ?;")

    End Sub


    Private Sub valid_btn_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles valid_btn.Click
        Emission("VALID " & Channel_combobox.SelectedItem & ",ON;")
    End Sub

    Private Sub chanq_btn_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles chanq_btn.Click
        Emission("CHAN " & Channel_combobox.SelectedItem & ";")
        Emission("CHAN ?;")
    End Sub

    Private Sub Device_combobox_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Device_combobox.SelectedIndexChanged

        Channel_combobox.Items.Clear()
        type_combobox.Items.Clear()

        If Device_combobox.SelectedIndex = 0 Then
            Channel_combobox.Items.Add("A1")
            Channel_combobox.Items.Add("A2")
            Channel_combobox.Items.Add("A3")
            Channel_combobox.Items.Add("A4")
            Channel_combobox.Items.Add("A5")
            Channel_combobox.Items.Add("A6")
            Channel_combobox.Items.Add("A10")
            Channel_combobox.Items.Add("A11")
            Channel_combobox.Items.Add("A12")
            Channel_combobox.Items.Add("A13")
            Channel_combobox.Items.Add("A14")
            Channel_combobox.Items.Add("A15")
            Channel_combobox.Items.Add("A16")
            Channel_combobox.Items.Add("A17")
            Channel_combobox.Items.Add("A18")
            Channel_combobox.Items.Add("A19")
            Channel_combobox.Items.Add("A20")
            Channel_combobox.Items.Add("K1")
            Channel_combobox.Items.Add("K2")
            Channel_combobox.Items.Add("K3")
            Channel_combobox.Items.Add("K4")
            Channel_combobox.SelectedIndex = 0
        End If

        If Device_combobox.SelectedIndex = 1 Then 'DAS30
            Channel_combobox.Items.Add("1")
            Channel_combobox.Items.Add("2")
            Channel_combobox.Items.Add("PT1")
            Channel_combobox.Items.Add("PT2")
            Channel_combobox.SelectedIndex = 0
        End If

        If Device_combobox.SelectedIndex = 2 Then 'DAS50
            Channel_combobox.Items.Add("1")
            Channel_combobox.Items.Add("2")
            Channel_combobox.Items.Add("3")
            Channel_combobox.Items.Add("4")
            Channel_combobox.Items.Add("PT1")
            Channel_combobox.Items.Add("PT2")
            Channel_combobox.SelectedIndex = 0
        End If

        If Device_combobox.SelectedIndex = 3 Then 'DAS60
            Channel_combobox.Items.Add("1")
            Channel_combobox.Items.Add("2")
            Channel_combobox.Items.Add("3")
            Channel_combobox.Items.Add("4")
            Channel_combobox.Items.Add("5")
            Channel_combobox.Items.Add("6")
            Channel_combobox.Items.Add("PT1")
            Channel_combobox.Items.Add("PT2")
            Channel_combobox.SelectedIndex = 0
        End If



        If Device_combobox.SelectedIndex = 0 Then 'DAS240/220
            type_combobox.Items.Add("VOLtage DC")
            type_combobox.Items.Add("SHUNT DC,S10M")
            type_combobox.Items.Add("THErmo J,COMP,CEL")
            type_combobox.Items.Add("FREQ")
            type_combobox.Items.Add("PWM")
            type_combobox.Items.Add("PT100 W3,0")
            type_combobox.SelectedIndex = 0
        Else
            type_combobox.Items.Add("VOLtage DC")
            type_combobox.Items.Add("SHUNT DC,S10M")
            type_combobox.Items.Add("THErmo J,COMP")
            type_combobox.Items.Add("FREQ")
            type_combobox.Items.Add("PT100 W3,0")
            type_combobox.SelectedIndex = 0
        End If






    End Sub

    Private Sub type_btn_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles type_btn.Click
        ' Emission("*CLS;")
        Emission("CHAN " & Channel_combobox.SelectedItem & ";")
        Emission("TYPe:" & type_combobox.SelectedItem & ";")
    End Sub

    Private Sub range_btn_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles range_btn.Click
        Emission("CHAN " & Channel_combobox.SelectedItem & ";")
        Emission("RANGE " & val_range.Value.ToString() & "," & range_zero.Value.ToString() & "," & range_pos.Value.ToString() & ";")
    End Sub

End Class