﻿Imports Simiweb.BusinessFacade
Imports System.Data.Linq
Imports DevExpress.Utils
Imports Simiweb.DataLinq
Partial Class Focolaio_AnalisiReport
    Inherits System.Web.UI.Page
    Private _mobjAnalisi As New BllAnalisi
    Private _MyDataDa As Nullable(Of Date) = Nothing
    Private _MyDataA As Nullable(Of Date) = Nothing
    Private _MydataRiferimento As String = Nothing
    Private _MyProvenienza As String = Nothing
    Private _idAsl As String = Nothing
    Private _idRegione As String = Nothing
    Public Enum CustomerReportKind
        malattia
    End Enum
    Protected Overloads Overrides Function SaveViewState() As Object
        Me.ViewState.Add("_idAsl", _idAsl)
        Me.ViewState.Add("_idRegione", _idRegione)
        Return MyBase.SaveViewState
    End Function
    Protected Overloads Overrides Sub LoadViewState(ByVal savedState As Object)
        MyBase.LoadViewState(savedState)
        _idAsl = ViewState("_idAsl")
        _idRegione = ViewState("_idRegione")

    End Sub
    Protected Sub Page_Load(sender As Object, e As System.EventArgs) Handles Me.Load
        If Not Page.IsPostBack Then

            impostaCampi()
            Dim mobjProfilo As New BllUser
            Dim profilo As Utenti_Profilo = mobjProfilo.GetProfiloByUsername(User.Identity.Name)
            _idAsl = profilo.idAsl
            _idRegione = profilo.idRegione
            dataDa.Text = Request.QueryString("dataDa")
            dataA.Text = Request.QueryString("dataA")
            Dim criterio = Request.QueryString("criterio")
            Dim prov As String = Request.QueryString("provenienza")
            Select Case prov
                Case "1"
                    Provenienza.Text = "Notificati da questa asl"
                Case "2"
                    Provenienza.Text = "Residenti in questa asl ma segnalate da altre asl"
                Case "3"
                    Provenienza.Text = "Notifcati e residenti in questa asl"
            End Select


            Select Case criterio
                Case "1"
                    dataRiferimento.Text = "data inizio sintomi/data segnalazione"
                Case "2"
                    dataRiferimento.Text = "data segnalazione"
                Case "3"
                    dataRiferimento.Text = "data notifica/data segnalazione"

            End Select

            ObjAnalisiDs.SelectParameters.Add("NotificatoDa", TypeCode.String, Helper.ConvertEmptyStringToNothing(prov))
            ObjAnalisiDs.SelectParameters.Add("Criterio", TypeCode.String, Helper.ConvertEmptyStringToNothing(criterio))
            ObjAnalisiDs.SelectParameters.Add("DataDa", TypeCode.DateTime, Helper.ConvertEmptyDateToNothing(dataDa.Text))
            ObjAnalisiDs.SelectParameters.Add("DataA", TypeCode.DateTime, Helper.ConvertEmptyDateToNothing(dataA.Text))
            ObjAnalisiDs.SelectParameters.Add("Asl", TypeCode.String, Helper.ConvertEmptyStringToNothing(_idAsl))
            ObjAnalisiDs.SelectParameters.Add("IdRegione", TypeCode.String, _idRegione)
        End If
    End Sub
    Private Sub impostaCampi()
        If User.IsInRole("regione") Then
            filter_asl.Visible = True
        End If
    End Sub
    Private Sub Export(ByVal saveAs As Boolean)
        gridExport.OptionsPrint.PrintHeadersOnEveryPage = checkPrintHeadersOnEveryPage.Checked
        If checkPrintFilterHeaders.Checked Then
            gridExport.OptionsPrint.PrintFilterHeaders = True
        Else
            gridExport.OptionsPrint.PrintFilterHeaders = False
        End If
        If checkPrintColumnHeaders.Checked Then
            gridExport.OptionsPrint.PrintColumnHeaders = True
        Else
            gridExport.OptionsPrint.PrintColumnHeaders = False
        End If
        If checkPrintRowHeaders.Checked Then
            gridExport.OptionsPrint.PrintRowHeaders = True
        Else
            gridExport.OptionsPrint.PrintRowHeaders = False
        End If
        If checkPrintDataHeaders.Checked Then
            gridExport.OptionsPrint.PrintDataHeaders = True
        Else
            gridExport.OptionsPrint.PrintDataHeaders = False
        End If

        Dim fileName As String = "Simiweb Report"

        gridExport.OptionsPrint.PageSettings.PaperKind = System.Drawing.Printing.PaperKind.A4Rotated
        Select Case listExportFormat.SelectedIndex
            Case 0
                gridExport.ExportPdfToResponse(fileName, saveAs)
            Case 1
                gridExport.ExportXlsToResponse(fileName, saveAs)
            Case 2
                gridExport.ExportMhtToResponse(fileName, "utf-8", "Report simiweb", True, saveAs)
            Case 3
                gridExport.ExportRtfToResponse(fileName, saveAs)
            Case 4
                gridExport.ExportTextToResponse(fileName, saveAs)
            Case 5 ' TODO
                gridExport.ExportHtmlToResponse(fileName, "utf-8", "Report simiweb", True, saveAs)
        End Select
    End Sub

    Protected Sub buttonSaveAs_Click(sender As Object, e As System.EventArgs) Handles buttonSaveAs.Click
        Export(True)
    End Sub
End Class
