using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Base.ViewInfo;
using DevExpress.Utils;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using DevExpress.XtraGrid.Views.Grid;

namespace WindowsApplication1
{
    public partial class Form1 : Form
    {
        private DataTable CreateTable(int RowCount)
        {
            DataTable tbl = new DataTable("Parent");
            tbl.Columns.Add("Name", typeof(string));
            tbl.Columns.Add("ID", typeof(int));
            tbl.Columns.Add("Number", typeof(int));
            tbl.Columns.Add("Date", typeof(DateTime));
            tbl.Columns.Add("DETAILID", typeof(int));
            for (int i = 0; i < RowCount; i++)
                tbl.Rows.Add(new object[] { String.Format("Name{0}", i), i, 3 - i, DateTime.Now.AddDays(i), i });
            return tbl;
        }

        private DataTable CreateDet1Table(int RowCount)
        {
            DataTable tbl = new DataTable("Det1");
            tbl.Columns.Add("Name", typeof(string));
            tbl.Columns.Add("ID", typeof(int));
            for (int j = 0; j < RowCount; j++)
                for (int i = 0; i < RowCount; i++)
                    tbl.Rows.Add(new object[] { String.Format("Detail1Name{0}", i), i });
            return tbl;
        }

        private DataTable CreateDet2Table(int RowCount)
        {
            DataTable tbl = new DataTable("Det2");
            tbl.Columns.Add("Name", typeof(string));
            tbl.Columns.Add("ID", typeof(int));
            for (int j = 0; j < RowCount; j++)
                for (int i = 0; i < RowCount; i++)
                    tbl.Rows.Add(new object[] { String.Format("Detail2Name{0}", i), i });
            return tbl;
        }


        private DataSet GetMasterDetail()
        {
            DataSet ds = new DataSet("TestDS");
            ds.Tables.Add(CreateTable(20));
            ds.Tables.Add(CreateDet1Table(20));
            ds.Tables.Add(CreateDet2Table(20));
            DataColumn parentColumn = ds.Tables["Parent"].Columns["DETAILID"];
            DataColumn childColumn = ds.Tables["Det1"].Columns["ID"];
            ds.Relations.Add(new DataRelation("relDet1", parentColumn, childColumn));
            childColumn = ds.Tables["Det2"].Columns["ID"];
            ds.Relations.Add(new DataRelation("relDet2", parentColumn, childColumn));
            return ds;
        }


        public Form1()
        {
            InitializeComponent();
            gridControl1.DataSource = GetMasterDetail();
            gridControl1.DataMember = "Parent";
        }

        public SuperToolTip GetToolTipByHitInfo(GridView view, GridHitInfo hi)
        {
            if (hi == null)
                return null;
            SuperToolTip toolTip = new SuperToolTip();
            string caption = view.GridControl.MainView.Equals(view) ? "Main View" : string.Format("DetailView: {0}", view.LevelName);
            toolTip.Items.AddTitle(caption);
            toolTip.Items.AddSeparator();
            if (!hi.InRowCell)
                return null;
            toolTip.Items.Add(String.Format("Cell at {0}:", hi.HitPoint));
            toolTip.Items.Add(string.Format("Column = {0}", hi.Column));
            toolTip.Items.Add(string.Format("RowHandle = {0}", hi.RowHandle));
            return toolTip;
        }




        private void defaultToolTipController1_DefaultController_GetActiveObjectInfo(object sender, DevExpress.Utils.ToolTipControllerGetActiveObjectInfoEventArgs e)
        {
            GridControl gridControl = e.SelectedControl as GridControl; 
            if (gridControl == null)
                return;
            GridView view = gridControl.GetViewAt(e.ControlMousePosition) as GridView;
            if (view == null)
                return;
            GridHitInfo hi = view.CalcHitInfo(e.ControlMousePosition);
            SuperToolTip toolTip = GetToolTipByHitInfo(view, hi);
            if (toolTip == null)
                return;
            e.Info = new ToolTipControlInfo();
            e.Info.Object = hi.Column;
            e.SelectedObject = hi.Column;
            e.Info.SuperTip = toolTip;
            e.Info.ToolTipType = ToolTipType.SuperTip;
            e.Info.ToolTipPosition = Control.MousePosition;
            e.Info.ImmediateToolTip = true;
        }
    }
}