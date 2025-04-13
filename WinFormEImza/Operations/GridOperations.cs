using WinFormEImza.Objects;
using System;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

namespace WinFormEImza.Operations
{
    internal class GridOperations
    {
        public void ConfigureGrid(DataGridView dgv)
        {
            try
            {
                dgv.AllowUserToAddRows = false;
                DataGridViewButtonColumn btnViewPdf = new DataGridViewButtonColumn();
                btnViewPdf.Name = "btnViewPdf";
                btnViewPdf.Text = "View PDF";
                btnViewPdf.UseColumnTextForButtonValue = true;
                btnViewPdf.FlatStyle = FlatStyle.Popup;
                btnViewPdf.DefaultCellStyle.BackColor = Color.LightGoldenrodYellow;
                btnViewPdf.DefaultCellStyle.ForeColor = Color.DarkSlateGray;
                if (dgv.Columns["btnViewPdf"] == null)
                {
                    dgv.Columns.Insert(4, btnViewPdf);
                }

                dgv.Columns[2].Visible = false;
                dgv.Columns[3].Visible = false;

                dgv.CellClick += dgvDocuments_CellClick;
                dgv.Columns[0].HeaderText = "Select";
                dgv.Columns[1].HeaderText = "File Path";
                dgv.Columns[2].HeaderText = "";
                dgv.Columns[3].HeaderText = "";
                dgv.Columns[4].HeaderText = "View File";


                dgv.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                dgv.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                dgv.Columns[2].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                dgv.Columns[3].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                dgv.Columns[4].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            }
            catch (Exception ex)
            {
                GeneralOperations.LogWrite(" ERROR [ConfigureGrid] (" + ex.Message + ")");
            }
        }

        private void dgvDocuments_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            DataGridView dgv = (DataGridView)sender;
            if (e.ColumnIndex == dgv.Columns["btnViewPdf"].Index)
            {
                ////MessageBox.Show(dgv.Rows[e.RowIndex].Cells[1].Value.ToString());
                Process.Start(dgv.Rows[e.RowIndex].Cells[1].Value.ToString());
            }
        }

        public void FillGrid(DataGridView dgv)
        {
            try
            {
                dgv.DataSource = null;
                DataTable dataTable = new DataTable();
                dataTable.Columns.Add(new DataColumn("IsSelected", typeof(bool)));
                dataTable.Columns.Add(new DataColumn("FilePath", typeof(string)));
                dataTable.Columns.Add(new DataColumn("TargetUploadUrl", typeof(string)));
                dataTable.Columns.Add(new DataColumn("TargetUploadQueryString", typeof(string)));
                dgv.DataSource = dataTable;
                ////this.AddItemToGrid(dgv, new SignatureDocument() { IsSelected = false, FilePath = @"c:\WinFormEImza\TEMP\Deneme.pdf" });
                ////this.AddItemToGrid(dgv, new SignatureDocument() { IsSelected = false, FilePath = @"c:\WinFormEImza\TEMP\Deneme.pdf" });
                ////this.AddItemToGrid(dgv, new SignatureDocument() { IsSelected = false, FilePath = @"c:\WinFormEImza\TEMP\Deneme3.pdf" });
                ////this.AddItemToGrid(dgv, new SignatureDocument() { IsSelected = true, FilePath = @"c:\WinFormEImza\TEMP\Deneme4.pdf" });
                ////this.AddItemToGrid(dgv, new SignatureDocument() { IsSelected = false, FilePath = @"c:\WinFormEImza\TEMP\Deneme5.pdf" });
                ////this.AddItemToGrid(dgv, new SignatureDocument() { IsSelected = true, FilePath = @"c:\WinFormEImza\TEMP\Deneme6.pdf" });
            }
            catch (Exception ex)
            {
                GeneralOperations.LogWrite(" ERROR [FillGrid] (" + ex.Message + ")");
            }
        }


        public void AddItemToGrid(DataGridView dgv, SignatureDocument item)
        {
            DataTable dataTable = (DataTable)dgv.DataSource;
            if (dataTable == null)
            {
                dataTable = new DataTable();
                dataTable.Columns.Add(new DataColumn("IsSelected", typeof(bool)));
                dataTable.Columns.Add(new DataColumn("FilePath", typeof(string)));
                dataTable.Columns.Add(new DataColumn("TargetUploadUrl", typeof(string)));
                dataTable.Columns.Add(new DataColumn("TargetUploadQueryString", typeof(string)));
            }
            DataRow drToAdd = dataTable.NewRow();
            drToAdd["IsSelected"] = item.IsSelected;
            drToAdd["FilePath"] = item.FilePath;
            drToAdd["TargetUploadUrl"] = item.TargetUploadUrl;
            drToAdd["TargetUploadQueryString"] = item.TargetUploadQueryString;
            dataTable.Rows.Add(drToAdd);
            dataTable.AcceptChanges();
            dgv.DataSource = dataTable;
        }
    }
}
