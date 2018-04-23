Imports Microsoft.VisualBasic
Imports System
Imports System.Collections.Generic
Imports System.ComponentModel
Imports System.Data
Imports System.Drawing
Imports System.Text
Imports System.Windows.Forms
Imports DevExpress.XtraGrid
Imports DevExpress.XtraGrid.Views.Base
Imports DevExpress.XtraGrid.Views.Base.ViewInfo
Imports DevExpress.Utils
Imports DevExpress.XtraGrid.Views.Grid.ViewInfo
Imports DevExpress.XtraGrid.Views.Grid

Namespace WindowsApplication1
	Partial Public Class Form1
		Inherits Form
		Private Function CreateTable(ByVal RowCount As Integer) As DataTable
			Dim tbl As New DataTable("Parent")
			tbl.Columns.Add("Name", GetType(String))
			tbl.Columns.Add("ID", GetType(Integer))
			tbl.Columns.Add("Number", GetType(Integer))
			tbl.Columns.Add("Date", GetType(DateTime))
			tbl.Columns.Add("DETAILID", GetType(Integer))
			For i As Integer = 0 To RowCount - 1
				tbl.Rows.Add(New Object() { String.Format("Name{0}", i), i, 3 - i, DateTime.Now.AddDays(i), i })
			Next i
			Return tbl
		End Function

		Private Function CreateDet1Table(ByVal RowCount As Integer) As DataTable
			Dim tbl As New DataTable("Det1")
			tbl.Columns.Add("Name", GetType(String))
			tbl.Columns.Add("ID", GetType(Integer))
			For j As Integer = 0 To RowCount - 1
				For i As Integer = 0 To RowCount - 1
					tbl.Rows.Add(New Object() { String.Format("Detail1Name{0}", i), i })
				Next i
			Next j
			Return tbl
		End Function

		Private Function CreateDet2Table(ByVal RowCount As Integer) As DataTable
			Dim tbl As New DataTable("Det2")
			tbl.Columns.Add("Name", GetType(String))
			tbl.Columns.Add("ID", GetType(Integer))
			For j As Integer = 0 To RowCount - 1
				For i As Integer = 0 To RowCount - 1
					tbl.Rows.Add(New Object() { String.Format("Detail2Name{0}", i), i })
				Next i
			Next j
			Return tbl
		End Function


		Private Function GetMasterDetail() As DataSet
			Dim ds As New DataSet("TestDS")
			ds.Tables.Add(CreateTable(20))
			ds.Tables.Add(CreateDet1Table(20))
			ds.Tables.Add(CreateDet2Table(20))
			Dim parentColumn As DataColumn = ds.Tables("Parent").Columns("DETAILID")
			Dim childColumn As DataColumn = ds.Tables("Det1").Columns("ID")
			ds.Relations.Add(New DataRelation("relDet1", parentColumn, childColumn))
			childColumn = ds.Tables("Det2").Columns("ID")
			ds.Relations.Add(New DataRelation("relDet2", parentColumn, childColumn))
			Return ds
		End Function


		Public Sub New()
			InitializeComponent()
			gridControl1.DataSource = GetMasterDetail()
			gridControl1.DataMember = "Parent"
		End Sub

		Public Function GetToolTipByHitInfo(ByVal view As GridView, ByVal hi As GridHitInfo) As SuperToolTip
			If hi Is Nothing Then
				Return Nothing
			End If
			Dim toolTip As New SuperToolTip()
			Dim caption As String
			If view.GridControl.MainView.Equals(view) Then
				caption = "Main View"
			Else
				caption = String.Format("DetailView: {0}", view.LevelName)
			End If
			toolTip.Items.AddTitle(caption)
			toolTip.Items.AddSeparator()
			If (Not hi.InRowCell) Then
				Return Nothing
			End If
			toolTip.Items.Add(String.Format("Cell at {0}:", hi.HitPoint))
			toolTip.Items.Add(String.Format("Column = {0}", hi.Column))
			toolTip.Items.Add(String.Format("RowHandle = {0}", hi.RowHandle))
			Return toolTip
		End Function




		Private Sub defaultToolTipController1_DefaultController_GetActiveObjectInfo(ByVal sender As Object, ByVal e As DevExpress.Utils.ToolTipControllerGetActiveObjectInfoEventArgs) Handles defaultToolTipController1.DefaultController.GetActiveObjectInfo
			Dim gridControl As GridControl = TryCast(e.SelectedControl, GridControl)
			If gridControl Is Nothing Then
				Return
			End If
			Dim view As GridView = TryCast(gridControl.GetViewAt(e.ControlMousePosition), GridView)
			If view Is Nothing Then
				Return
			End If
			Dim hi As GridHitInfo = view.CalcHitInfo(e.ControlMousePosition)
			Dim toolTip As SuperToolTip = GetToolTipByHitInfo(view, hi)
			If toolTip Is Nothing Then
				Return
			End If
			e.Info = New ToolTipControlInfo()
			e.Info.Object = hi.Column
			e.SelectedObject = hi.Column
			e.Info.SuperTip = toolTip
			e.Info.ToolTipType = ToolTipType.SuperTip
			e.Info.ToolTipPosition = Control.MousePosition
			e.Info.ImmediateToolTip = True
		End Sub
	End Class
End Namespace