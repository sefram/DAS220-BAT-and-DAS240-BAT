Option Strict Off
Option Explicit On
Friend Class frmConfig
	Inherits System.Windows.Forms.Form
	
	Private Sub frmConfig_Load(ByVal eventSender As System.Object, ByVal eventArgs As System.EventArgs) Handles MyBase.Load
		
		Text = "Configuration TCPIP"
        cmdValider.Text = "Valider"
		
	End Sub
	
	Private Sub frmConfig_FormClosed(ByVal eventSender As System.Object, ByVal eventArgs As System.Windows.Forms.FormClosedEventArgs) Handles Me.FormClosed
		
		frmMain.Enabled = True

	End Sub

	Private Sub cmdValider_Click(ByVal eventSender As System.Object, ByVal eventArgs As System.EventArgs) Handles cmdValider.Click
		
		ErreurRemote = False
        AdresseIP = _txtIP_0.Text.Trim & "." & _txtIP_1.Text.Trim & "." & _txtIP_2.Text.Trim & "." & _txtIP_3.Text.Trim

        frmMain.Enabled = True
        Me.Close()

    End Sub

    Private Sub txtIP_TextChanged(ByVal eventSender As System.Object, ByVal eventArgs As System.EventArgs) Handles txtIP.TextChanged
        Dim Index As Short = txtIP.GetIndex(eventSender)

        AdresseIP = _txtIP_0.Text.Trim & "." & _txtIP_1.Text.Trim & "." & _txtIP_2.Text.Trim & "." & _txtIP_3.Text.Trim

    End Sub

End Class