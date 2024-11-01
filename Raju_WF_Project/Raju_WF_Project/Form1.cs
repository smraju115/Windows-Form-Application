using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Raju_WF_Project
{
    public partial class Form1 : Form
    {
        BindingSource bsP = new BindingSource();
        BindingSource bsS = new BindingSource();
        DataSet ds;
        public Form1()
        {
            InitializeComponent();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            dataGridView1.AutoGenerateColumns = false;
            LoadDataBindingSources();
        }
        public void LoadDataBindingSources()
        {
            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["db"].ConnectionString))
            {
                using (SqlDataAdapter da = new SqlDataAdapter("SELECT * FROM mobiles ", con))
                {
                    ds = new DataSet();
                    da.Fill(ds, "mobiles");
                    da.SelectCommand.CommandText = @"SELECT      specifications.spid, specifications.specname, specifications.specvalue, specifications.mid
                                                    FROM            specifications INNER JOIN
                         mobiles ON mobiles.mid = specifications.spid";
                    da.Fill(ds, "specifications");
                    //Add image column
                    ds.Tables["mobiles"].Columns.Add(new DataColumn("image", typeof(byte[])));
                    for (var i = 0; i < ds.Tables["mobiles"].Rows.Count; i++)
                    {
                        ds.Tables["mobiles"].Rows[i]["image"] = File.ReadAllBytes($@"..\..\Pictures\{ds.Tables["mobiles"].Rows[i]["picture"]}");
                    }
                    DataRelation rel = new DataRelation("FK_P_S", ds.Tables["mobiles"].Columns["mid"], ds.Tables["specifications"].Columns["mid"]);
                    ds.Relations.Add(rel);
                    bsP.DataSource = ds;
                    bsP.DataMember = "mobiles";

                    bsS.DataSource = bsP;
                    bsS.DataMember = "FK_P_S";

                    dataGridView1.DataSource = bsS;
                    AddDataBindings();
                }
            }
        }
        private void AddDataBindings()
        {
            lblId.DataBindings.Clear();
            lblId.DataBindings.Add(new Binding("Text", bsP, "mid"));
            lblName.DataBindings.Clear();
            lblName.DataBindings.Add(new Binding("Text", bsP, "model"));
            lblPrice.DataBindings.Clear();
            Binding bp = new Binding("Text", bsP, "price", true);
            bp.Format += Bp_Format;
            lblPrice.DataBindings.Add(bp);
            pictureBox1.DataBindings.Clear();
            pictureBox1.DataBindings.Add(new Binding("Image", bsP, "image", true));
            Binding bm = new Binding("Text", bsP, "mfgdate", true);
            bm.Format += Bm_Format;
            lblMfg.DataBindings.Clear();
            lblMfg.DataBindings.Add(bm);
            checkBox1.DataBindings.Clear();
            checkBox1.DataBindings.Add("Checked", bsP, "marketavailable", true);

        }
        private void Bm_Format(object sender, ConvertEventArgs e)
        {
            DateTime d = (DateTime)e.Value;
            e.Value = d.ToString("yyyy-MM-dd");
        }

        private void Bp_Format(object sender, ConvertEventArgs e)
        {
            decimal d = (decimal)e.Value;
            e.Value = d.ToString("0.00");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (bsP.Position < bsP.Count - 1)
            {
                bsP.MoveNext();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (bsP.Position > 0)
            {
                bsP.MovePrevious();
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            bsP.MoveLast();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            bsP.MoveFirst();
        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            new AddForm { TheForm = this }.ShowDialog();
        }

        private void editDeleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new EditForm { TheForm = this }.ShowDialog();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            int v = int.Parse((bsP.Current as DataRowView).Row[0].ToString());
            new EditForm { TheForm = this, IdToEdit = v }.ShowDialog();
        }

        private void lblId_Click(object sender, EventArgs e)
        {

        }
    }
}
