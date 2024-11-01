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
    public partial class EditForm : Form
    {
        List<Specifications> specifications = new List<Specifications>();
        string currentFile = "";
        string oldFile = "";
        public EditForm()
        {
            InitializeComponent();
        }
        public Form1 TheForm { get; set; }
        public int IdToEdit { get; set; }
        private void EditForm_Load(object sender, EventArgs e)
        {
            dataGridView1.AutoGenerateColumns = false;
            LoadDataToForm();
        }
        private void LoadDataToForm()
        {

            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["db"].ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand("SELECT * FROM mobiles WHERE mid=@i", con))
                {
                    cmd.Parameters.AddWithValue("@i", IdToEdit);
                    con.Open();
                    SqlDataReader dr = cmd.ExecuteReader();
                    if (dr.Read())
                    {
                        textBox1.Text = dr.GetString(1);
                        numericUpDown1.Value = dr.GetDecimal(2);
                        dateTimePicker1.Value = dr.GetDateTime(3).Date;
                        checkBox1.Checked = dr.GetBoolean(4);
                        pictureBox1.Image = Image.FromFile(@"..\..\Pictures\" + dr.GetString(5));
                        oldFile = dr.GetString(5);
                    }
                    dr.Close();
                    cmd.CommandText = @"SELECT      specifications.spid, specifications.specname, specifications.specvalue,  specifications.mid
                                                    FROM            specifications INNER JOIN
                         mobiles ON mobiles.mid = specifications.spid WHERE specifications.mid=@i";
                    cmd.Parameters.Clear();
                    cmd.Parameters.AddWithValue("@i", IdToEdit);
                    SqlDataReader dr2 = cmd.ExecuteReader();
                    while (dr2.Read())
                    {
                        specifications.Add(new Specifications { specname = dr2.GetString(0),specvalue = dr2.GetString(1) });
                    }
                    SetDataSources();
                    con.Close();
                }
            }
        }
        private void SetDataSources()
        {
            dataGridView1.DataSource = null;
            dataGridView1.DataSource = specifications;
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 3)
            {
                specifications.RemoveAt(e.RowIndex);
                dataGridView1.DataSource = null;
                dataGridView1.DataSource = specifications;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            specifications.Add(new Specifications { specname = textBox1.Text, specvalue = textBox2.Text });
            dataGridView1.DataSource = null;
            dataGridView1.DataSource = specifications;
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
                        string f = oldFile;
                        if (currentFile != "")
                        {
                            string ext = Path.GetExtension(currentFile);
                            f = Path.GetFileNameWithoutExtension(Path.GetRandomFileName()) + ext;
                            string savePath = @"..\..\Pictures\" + f;
                            MemoryStream ms = new MemoryStream(File.ReadAllBytes(currentFile));
                            byte[] bytes = ms.ToArray();
                            FileStream fs = new FileStream(savePath, FileMode.Create);
                            fs.Write(bytes, 0, bytes.Length);
                            fs.Close();
                        }
                        cmd.CommandText = "UPDATE mobiles SET model=@m, mfgdate=@dt, price=@pr, picture=@pic, marketavailable=@mav, WHERE mid=@id";
                        cmd.Parameters.AddWithValue("@id", IdToEdit);
                        cmd.Parameters.AddWithValue("@m", textBox1.Text);
                        cmd.Parameters.AddWithValue("@dt", dateTimePicker1.Value);
                        cmd.Parameters.AddWithValue("@pr", numericUpDown1.Value);
                        cmd.Parameters.AddWithValue("@pic", f);
                        cmd.Parameters.AddWithValue("@mav", checkBox1.Checked);
                        try
                        {
                            //1
                            cmd.ExecuteNonQuery();
                            //2
                            cmd.CommandText = "DELETE FROM specifications WHERE mid = @id";
                            cmd.Parameters.Clear();
                            cmd.Parameters.AddWithValue("@id", IdToEdit);
                            cmd.ExecuteNonQuery();
                            foreach (var s in specifications)
                            {
                                cmd.CommandText = "INSERT INTO specifications (specname, specvalue) VALUES (@spn, @spv)";
                                cmd.Parameters.Clear();
                                cmd.Parameters.AddWithValue("@spn", s.specname);
                                cmd.Parameters.AddWithValue("@spv", s.specvalue);
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
                            
                        }
                        catch
                        {
                            trx.Rollback();
                        }

                    }
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                currentFile = openFileDialog1.FileName;
            }
        }
    }
    
}
