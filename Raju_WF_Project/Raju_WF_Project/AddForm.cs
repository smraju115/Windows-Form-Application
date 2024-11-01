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
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace Raju_WF_Project
{
    public partial class AddForm : Form
    {
        List<Specifications> specifications = new List<Specifications>();
        string currentFile = "";
        public AddForm()
        {
            InitializeComponent();
        }
        public Form1 TheForm {  get; set; }
        private void AddForm_Load(object sender, EventArgs e)
        {
            dataGridView1.AutoGenerateColumns = false;
            //LoadSpecificationsCombo();
        }

        //private void LoadSpecificationsCombo()
        //{
        //    using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["db"].ConnectionString))
        //    {
        //        using (SqlDataAdapter da = new SqlDataAdapter("SELECT * FROM sellers ", con))
        //        {
        //            DataTable dt = new DataTable();
        //            da.Fill(dt);
        //        }
        //    }
        //}

        private void textBox3_TextChanged(object sender, EventArgs e)
        {

        }

        private void groupBox3_Enter(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                currentFile = openFileDialog1.FileName;
                pictureBox1.Image = Image.FromFile(currentFile);
            }
        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            specifications.Add(new Specifications { specname=textBox3.Text, specvalue=textBox2.Text });
            dataGridView1.DataSource = null;
            dataGridView1.DataSource = specifications;
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 2)
            {
                specifications.RemoveAt(e.RowIndex);
                dataGridView1.DataSource = null;
                dataGridView1.DataSource = specifications;
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["db"].ConnectionString))
            {
                con.Open();
                using (SqlTransaction trx = con.BeginTransaction())
                {
                    using (SqlCommand cmd = new SqlCommand())
                    {
                        cmd.Connection = con;
                        cmd.Transaction = trx;
                        string ext = Path.GetExtension(currentFile);
                        string f = Path.GetFileNameWithoutExtension(Path.GetRandomFileName()) + ext;
                        string savePath = @"..\..\Pictures\" + f;
                        MemoryStream ms = new MemoryStream(File.ReadAllBytes(currentFile));
                        byte[] bytes = ms.ToArray();
                        FileStream fs = new FileStream(savePath, FileMode.Create);
                        fs.Write(bytes, 0, bytes.Length);
                        fs.Close();
                        cmd.CommandText = "INSERT INTO mobiles(model, mfgdate, price, marketavailable, picture) VALUES(@m, @d, @p, @ma, @pic); SELECT SCOPE_IDENTITY();";
                        cmd.Parameters.AddWithValue("@m", textBox1.Text);
                        cmd.Parameters.AddWithValue("@d", dateTimePicker1.Value);
                        cmd.Parameters.AddWithValue("@p", numericUpDown1.Value);
                        cmd.Parameters.AddWithValue("@ma", checkBox1.Checked);
                        cmd.Parameters.AddWithValue("@pic", f);
                        try
                        {
                            var mid = cmd.ExecuteScalar();
                            foreach (var s in specifications)
                            {
                                cmd.CommandText = "INSERT INTO specifications (specname, specvalue, mid) VALUES (@spn, @spv, @i)";
                                cmd.Parameters.Clear();
                                cmd.Parameters.AddWithValue("@spn", s.specname);
                                cmd.Parameters.AddWithValue("@spv", s.specvalue);
                                cmd.Parameters.AddWithValue("@i", mid);
                                cmd.ExecuteNonQuery();
                            }
                            trx.Commit();
                            TheForm.LoadDataBindingSources();
                            MessageBox.Show("Data saved", "Success");
                            specifications.Clear();
                            dataGridView1.DataSource = null;
                            dataGridView1.DataSource = specifications;
                            textBox1.Clear();
                            numericUpDown1.Value = 0;
                            dateTimePicker1.Value = DateTime.Now;
                            checkBox1.Checked = false;
                            pictureBox1.Image = Image.FromFile(@"..\..\Pictures\f.png");
                            //dateTimePicker2.Value = DateTime.Now;
                            //numericUpDown2.Value = 0;
                        }
                        catch
                        {
                            trx.Rollback();
                        }

                    }
                }
            }
        }
    }
    public class Specifications
    {
        public string specname { get; set; }
        public string specvalue { get; set; }

    }

}
